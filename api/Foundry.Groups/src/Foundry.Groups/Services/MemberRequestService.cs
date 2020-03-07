/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stack.DomainEvents;
using Foundry.Groups.Data;
using Foundry.Groups.Data.Repositories;
using Foundry.Groups.Notifications;
using Foundry.Groups.ViewModels;
using Stack.Http.Identity;
using Stack.Patterns.Service.Models;
using Stack.Validation.Handlers;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Groups.Services
{
    /// <summary>
    /// member request service
    /// </summary>
    public class MemberRequestService : DispatchService<IMemberRequestRepository, MemberRequest>
    {
        /// <summary>
        /// create an instance of member request service
        /// </summary>
        /// <param name="domainEventDispatcher"></param>
        /// <param name="identityResolver"></param>
        /// <param name="memberRequestRepository"></param>
        /// <param name="mapper"></param>
        /// <param name="validationHandler"></param>
        /// <param name="logger"></param>
        public MemberRequestService(
            IDomainEventDispatcher domainEventDispatcher,
            IStackIdentityResolver identityResolver,
            IMemberRequestRepository memberRequestRepository,
            IMapper mapper,
            IValidationHandler validationHandler,
            ILogger<MemberRequestService> logger)
            : base(domainEventDispatcher, identityResolver, memberRequestRepository, mapper, validationHandler, logger) { }

        /// <summary>
        /// get all member requests
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<PagedResult<MemberRequest, MemberRequestDetail>> GetAll(MemberRequestDataFilter search = null)
        {
            return await PagedResult<MemberRequest, MemberRequestDetail>(Repository.GetAll(), search);
        }

        /// <summary>
        /// get all member requests
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<PagedResult<MemberRequest, MemberRequestDetail>> GetAllByGroupId(string groupId, MemberRequestDataFilter search = null)
        {
            var query = Repository.GetAllByGroupId(groupId);

            return await PagedResult<MemberRequest, MemberRequestDetail>(query, search);
        }

        /// <summary>
        /// get by ids
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task<MemberRequestDetail> GetByIds(string groupId, string accountId)
        {
            var request = await Repository.GetByIds(groupId, accountId);

            return Map<MemberRequestDetail>(request);
        }

        /// <summary>
        /// update member request
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<MemberRequestDetail> Update(MemberRequestUpdate model)
        {
            await ValidationHandler.ValidateRulesFor(model);

            var request = await Repository.GetByIds(model.GroupId, model.AccountId);

            request.Status = model.Status;
            await DbContext.SaveChangesAsync();

            if (model.Status == MemberRequestStatus.Approved)
            {
                var create = new MemberCreate { AccountId = model.AccountId, GroupId = model.GroupId };

                var member = MemberService.Add(create, DbContext);

                await DispatchAsync(new DomainEvent(
                        MemberRequestNotificationModel.ToModel("accept", member.Group, member.Account.Id, member.Account.Name),
                        member.Group.Id, member.Group.Name, "memberrequest"));

                DbContext.MemberRequests.Remove(request);
                await DbContext.SaveChangesAsync();
            }

            if (model.Status == MemberRequestStatus.Denied)
            {
                await DispatchAsync(new DomainEvent(
                        MemberRequestNotificationModel.ToModel("reject", request.Group, request.Account.Id, request.Account.Name),
                        request.Group.Id, request.Group.Name, "memberrequest"));
            }

            return await GetByIds(model.GroupId, model.AccountId);
        }

        /// <summary>
        /// create a member request
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<MemberRequestDetail> Add(MemberRequestCreate model)
        {
            await ValidationHandler.ValidateRulesFor(model);

            var db = Repository.DbContext;

            var account = db.Accounts.SingleOrDefault(a => a.Id == model.AccountId);

            if (account == null)
            {
                db.Accounts.Add(new Account { Name = model.AccountName, Id = model.AccountId });
                db.SaveChanges();
            }

            var request = new MemberRequest
            {
                GroupId = model.GroupId,
                AccountId = model.AccountId,
                Status = MemberRequestStatus.Pending
            };

            var saved = await Repository.Add(request);

            var group = db.Groups
                .Include(g => g.Members)
                .SingleOrDefault(g => g.Id == model.GroupId);

            await DispatchAsync(new DomainEvent(
                MemberRequestNotificationModel.ToModel("add", group, account.Id, account.Name),
                group.Id, group.Name, "memberrequest"));

            return await GetByIds(model.GroupId, model.AccountId);
        }

        /// <summary>
        /// delete member request
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> Delete(MemberRequestDelete model)
        {
            await ValidationHandler.ValidateRulesFor(model);

            var request = await Repository.GetByIds(model.GroupId, model.AccountId);

            var accountId = request.AccountId;
            var accountName = request.Account.Name;

            var sendNotification = request.Status == MemberRequestStatus.Pending;

            DbContext.MemberRequests.Remove(request);
            await DbContext.SaveChangesAsync();

            if (sendNotification)
            {
                var group = DbContext.Groups
                    .Include(g => g.Members)
                    .SingleOrDefault(g => g.Id == model.GroupId);

                await DispatchAsync(new DomainEvent(
                    MemberRequestNotificationModel.ToModel("reject", group, accountId, accountName),
                    group.Id, group.Name, "memberrequest"));
            }

            return true;
        }
    }
}

