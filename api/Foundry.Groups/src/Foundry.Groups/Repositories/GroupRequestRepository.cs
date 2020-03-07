/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.EntityFrameworkCore;
using Foundry.Groups.Data;
using Foundry.Groups.Data.Repositories;
using Foundry.Groups.Security;
using Stack.Http.Exceptions;
using Stack.Patterns.Repository;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Groups.Repositories
{
    /// <summary>
    /// group repository
    /// </summary>
    public class GroupRequestRepository : Repository<GroupsDbContext, GroupRequest>, IGroupRequestRepository
    {
        GroupRequestPermissionMediator PermissionMediator { get; }

        public GroupRequestRepository(GroupsDbContext db, GroupRequestPermissionMediator permissionMediator)
            : base(db)
        {
            PermissionMediator = permissionMediator;
        }

        /// <summary>
        /// get all group requests
        /// </summary>
        /// <returns></returns>
        public override IQueryable<GroupRequest> GetAll()
        {
            var query = DbContext.GroupRequests
                .Include(gr => gr.ParentGroup)
                .Include(gr => gr.ParentGroup.Members)
                .Include(gr => gr.ChildGroup)
                .Include(gr => gr.ChildGroup.Members);

            return PermissionMediator.Process(query);
        }

        /// <summary>
        /// get group request
        /// </summary>
        /// <param name="parentGroupId"></param>
        /// <param name="childGroupId"></param>
        /// <returns></returns>
        public async Task<GroupRequest> GetByIds(string parentGroupId, string childGroupId)
        {
            var request = await GetAll()
                .SingleOrDefaultAsync(gr => gr.ParentGroupId == parentGroupId && gr.ChildGroupId == childGroupId);

            return request;
        }

        /// <summary>
        /// get all group requests by parent
        /// </summary>
        /// <param name="parentGroupId"></param>
        /// <returns></returns>
        public IQueryable<GroupRequest> GetAllByParentId(string parentGroupId)
        {
            return GetAll()
                .Where(gr => gr.ParentGroupId == parentGroupId);
        }

        /// <summary>
        /// get all group request by child
        /// </summary>
        /// <param name="childGroupId"></param>
        /// <returns></returns>
        public IQueryable<GroupRequest> GetAllByChildId(string childGroupId)
        {
            return GetAll()
                .Where(gr => gr.ChildGroupId == childGroupId);
        }

        /// <summary>
        /// delete
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override async Task Delete(GroupRequest entity)
        {
            if (!PermissionMediator.CanDelete(entity))
                throw new EntityPermissionException("Group request delete requires elevated permissions.");

            await base.Delete(entity);
        }

        /// <summary>
        /// update
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override async Task<GroupRequest> Update(GroupRequest entity)
        {
            if (!PermissionMediator.CanUpdate(entity))
                throw new EntityPermissionException("Member request update requires elevated permissions.");

            return await base.Update(entity);
        }
    }
}

