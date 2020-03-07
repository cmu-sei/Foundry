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
using Foundry.Groups.Data;
using Foundry.Groups.Data.Repositories;
using Foundry.Groups.ViewModels;
using Stack.Http.Identity;
using Stack.Patterns.Service.Models;
using Stack.Validation.Handlers;
using System.Threading.Tasks;

namespace Foundry.Groups.Services
{
    /// <summary>
    /// group request service
    /// </summary>
    public class GroupRequestService : Service<IGroupRequestRepository, GroupRequest>
    {
        public GroupRequestService(IStackIdentityResolver identityResolver, IGroupRequestRepository groupRepository, IMapper mapper, IValidationHandler validationHandler)
            : base(identityResolver, groupRepository, mapper, validationHandler) { }

        /// <summary>
        /// get all groups
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<PagedResult<GroupRequest, GroupRequestDetail>> GetAll(GroupRequestDataFilter search = null)
        {
            return await PagedResult<GroupRequest, GroupRequestDetail>(Repository.GetAll(), search);
        }

        /// <summary>
        /// get all group requests
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<PagedResult<GroupRequest, GroupRequestDetail>> GetAllByParentId(string parentId, GroupRequestDataFilter search = null)
        {
            var query = Repository.GetAllByParentId(parentId);

            return await PagedResult<GroupRequest, GroupRequestDetail>(query, search);
        }

        /// <summary>
        /// get by parent and child id
        /// </summary>
        /// <param name="parentGroupId"></param>
        /// <param name="childGroupId"></param>
        /// <returns></returns>
        public async Task<GroupRequestDetail> GetByIds(string parentGroupId, string childGroupId)
        {
            var request = await Repository.GetByIds(parentGroupId, childGroupId);

            return Map<GroupRequestDetail>(request);
        }

        /// <summary>
        /// update group request
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<GroupRequestDetail> Update(GroupRequestUpdate model)
        {
            await ValidationHandler.ValidateRulesFor(model);

            var request = await Repository.GetByIds(model.ParentGroupId, model.ChildGroupId);

            request.Status = model.Status;
            await Repository.DbContext.SaveChangesAsync();

            if (model.Status == GroupRequestStatus.Approved)
            {
                var group = await Repository.DbContext.Groups.Include(g => g.Parent)
                    .SingleOrDefaultAsync(g => g.Id == model.ChildGroupId);

                group.ParentId = request.ParentGroupId;
                await Repository.DbContext.SaveChangesAsync();

                await GroupService.UpdateKey(DbContext, group);
            }

            return await GetByIds(model.ParentGroupId, model.ChildGroupId);
        }

        /// <summary>
        /// add group request
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<GroupRequestDetail> Add(GroupRequestCreate model)
        {
            await ValidationHandler.ValidateRulesFor(model);

            var request = new GroupRequest
            {
                ChildGroupId = model.ChildGroupId,
                ParentGroupId = model.ParentGroupId,
                Status = GroupRequestStatus.Pending
            };

           DbContext.GroupRequests.Add(request);
           await DbContext.SaveChangesAsync();

            return await GetByIds(model.ParentGroupId, model.ChildGroupId);
        }

        /// <summary>
        /// delete group request
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> Delete(GroupRequestDelete model)
        {
            await ValidationHandler.ValidateRulesFor(model);

            var request = await Repository.GetByIds(model.ParentGroupId, model.ChildGroupId);

            Repository.DbContext.GroupRequests.Remove(request);

            await Repository.DbContext.SaveChangesAsync();

            return true;
        }
    }
}

