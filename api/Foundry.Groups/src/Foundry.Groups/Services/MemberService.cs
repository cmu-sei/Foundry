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
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Patterns.Service.Models;
using Stack.Validation.Handlers;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Groups.Services
{
    /// <summary>
    /// member service
    /// </summary>
    public class MemberService : DispatchService<IMemberRepository, Member>
    {
        /// <summary>
        /// create an instance of member service
        /// </summary>
        /// <param name="domainEventDispatcher"></param>
        /// <param name="identityResolver"></param>
        /// <param name="memberRepository"></param>
        /// <param name="mapper"></param>
        /// <param name="validationHandler"></param>
        /// <param name="logger"></param>
        public MemberService(
            IDomainEventDispatcher domainEventDispatcher,
            IStackIdentityResolver identityResolver,
            IMemberRepository memberRepository,
            IMapper mapper,
            IValidationHandler validationHandler,
            ILogger<MemberService> logger)
            : base(domainEventDispatcher, identityResolver, memberRepository, mapper, validationHandler, logger)
        {
        }

        /// <summary>
        /// get member by group and account
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task<MemberDetail> Get(string groupId, string accountId)
        {
            var member = await Repository.Get(groupId, accountId);

            return Map<MemberDetail>(member);
        }

        /// <summary>
        /// get all members by group id
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<PagedResult<Member, MemberSummary>> GetAllByGroupId(string groupId, MemberDataFilter search = null)
        {
            var query = Repository.GetAll().Where(m => m.GroupId.ToLower() == groupId.ToLower());

            return await PagedResult<Member, MemberSummary>(query, search);
        }

        /// <summary>
        /// add member
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<MemberDetail> Add(MemberCreate model)
        {
            if (!IsAdministrator)
                throw new EntityPermissionException("Action requires elevated permissions.");

            await ValidationHandler.ValidateRulesFor(model);

            var member = Add(model, DbContext);

            await DispatchAsync(new DomainEvent(
                MemberNotificationModel.ToModel("add", member.Group, member.AccountId, member.Account.Name),
                member.Group.Id, member.Group.Name, "member"));

            return Map<MemberDetail>(member);
        }

        /// <summary>
        /// create member for accounts
        /// create account if it doesn't exist
        /// </summary>
        /// <param name="model"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        /// <remarks>referenced by GroupService</remarks>
        internal static Member Add(MemberCreate model, GroupsDbContext db)
        {
            var account = db.Accounts.SingleOrDefault(a => a.Id == model.AccountId);

            if (account == null)
            {
                db.Accounts.Add(new Account { Name = model.AccountName, Id = model.AccountId });
                db.SaveChanges();
            }

            var member = new Member
            {
                AccountId = model.AccountId,
                GroupId = model.GroupId,
                IsManager = model.IsManager,
                IsOwner = model.IsOwner
            };

            db.Members.Add(member);
            db.SaveChanges();

            return db.Members
                .Include(m => m.Account)
                .Include(m => m.Group)
                .Include("Group.Members")
                .SingleOrDefault(m => m.AccountId == model.AccountId && m.GroupId == model.GroupId);
        }


        /// <summary>
        /// update member
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<MemberDetail> Update(MemberUpdate model)
        {
            await ValidationHandler.ValidateRulesFor(model);

            var member = await Repository.DbContext.Members
                .Include(m => m.Group)
                .Include(m => m.Account)
                .Include("Group.Members")
                .SingleOrDefaultAsync(m => m.AccountId == model.AccountId && m.GroupId == model.GroupId);


            if (member.IsManager != model.IsManager || member.IsOwner != model.IsOwner)
            {
                var promote = (!member.IsManager && model.IsManager) || (!member.IsOwner && model.IsOwner);
                var demote = (member.IsManager && !model.IsManager && !model.IsOwner) || (member.IsOwner && !model.IsOwner);

                member.IsManager = model.IsManager;
                member.IsOwner = model.IsOwner;

                var saved = await Repository.Update(member);

                if (demote)
                {
                    await DispatchAsync(new DomainEvent(
                        MemberNotificationModel.ToModel("demote", member.Group, member.AccountId, member.Account.Name),
                        member.Group.Id, member.Group.Name, "member"));
                }
                else if (promote)
                {
                    await DispatchAsync(new DomainEvent(
                        MemberNotificationModel.ToModel("promote", member.Group, member.AccountId, member.Account.Name),
                        member.Group.Id, member.Group.Name, "member"));
                }
            }

            return await Get(member.GroupId, member.AccountId);
        }

        /// <summary>
        /// delete member
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> Delete(MemberDelete model)
        {
            await ValidationHandler.ValidateRulesFor(model);

            var member = await Repository.DbContext.Members
                .Include(m => m.Group)
                .Include(m => m.Account)
                .Include("Group.Members")
                .SingleOrDefaultAsync(m => m.AccountId == model.AccountId && m.GroupId == model.GroupId);

            var group = member.Group;
            var accountId = member.Account.Id;
            var memberName = member.Account.Name;

            await Repository.Delete(member);

            if (model.AccountId == Identity.Id)
            {
                await DispatchAsync(new DomainEvent(
                        MemberNotificationModel.ToModel("leave", member.Group, accountId, memberName),
                        member.Group.Id, member.Group.Name, "member"));
            }
            else
            {
                await DispatchAsync(new DomainEvent(
                        MemberNotificationModel.ToModel("delete", member.Group, accountId, memberName),
                        member.Group.Id, member.Group.Name, "member"));
            }

            return true;
        }
    }
}

