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
    public class GroupRepository : Repository<GroupsDbContext, Group>, IGroupRepository
    {
        GroupPermissionMediator PermissionMediator { get; }

        public GroupRepository(GroupsDbContext db, GroupPermissionMediator permissionMediator)
            : base(db)
        {
            PermissionMediator = permissionMediator;
        }

        /// <summary>
        /// check if id exists
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Exists(string id)
        {
            return DbContext.Groups.Any(g => g.Id == id);
        }

        /// <summary>
        /// get all groups
        /// </summary>
        /// <returns></returns>
        public override IQueryable<Group> GetAll()
        {
            var query = DbContext.Groups
                .Include(g => g.Parent)
                .Include(g => g.Children)
                .Include(g => g.Members)
                .Include(g => g.MemberRequests);

            return PermissionMediator.Process(query);
        }

        /// <summary>
        /// get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Group> GetById(string id)
        {
            var query = DbContext.Groups
                .Include(g => g.Parent)
                .Include(g => g.Children)
                .Include(g => g.Members)
                .Include(g => g.MemberRequests);

            return await PermissionMediator.Process(query).SingleOrDefaultAsync(g => g.Id == id);
        }

        /// <summary>
        /// is child group
        /// </summary>
        /// <param name="parentGroupId"></param>
        /// <param name="childGroupId"></param>
        /// <returns></returns>
        public async Task<bool> IsChildGroup(string parentGroupId, string childGroupId)
        {
            var query = DbContext.Groups
                .Include(g => g.Parent)
                .Include(g => g.Children);

            return await query.AnyAsync(g => g.ParentId == parentGroupId && g.Id == childGroupId);
        }

        /// <summary>
        /// delete
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override async Task Delete(Group entity)
        {
            if (!PermissionMediator.CanDelete(entity))
                throw new EntityPermissionException("Group delete requires elevated permissions.");

            await base.Delete(entity);
        }

        /// <summary>
        /// update
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override async Task<Group> Update(Group entity)
        {
            if (!PermissionMediator.CanUpdate(entity))
                throw new EntityPermissionException("Group update requires elevated permissions.");

            return await base.Update(entity);
        }
    }
}

