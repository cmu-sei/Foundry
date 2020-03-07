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
    public class MemberRepository : Repository<GroupsDbContext, Member>, IMemberRepository
    {
        MemberPermissionMediator PermissionMediator { get; }

        public MemberRepository(GroupsDbContext db, MemberPermissionMediator permissionMediator)
            : base(db)
        {
            PermissionMediator = permissionMediator;
        }

        /// <summary>
        /// get
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task<Member> Get(string groupId, string accountId)
        {
            var query = DbContext.Members
                .Include(m => m.Account)
                .Include(m => m.Group);

            return await query.SingleOrDefaultAsync(m => m.AccountId == accountId && m.GroupId == groupId);
        }

        /// <summary>
        /// get all members
        /// </summary>
        /// <returns></returns>
        public override IQueryable<Member> GetAll()
        {
            var query = DbContext.Members
                .Include(m => m.Account)
                .Include(m => m.Group);

            return query;
        }

        /// <summary>
        /// delete
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override async Task Delete(Member entity)
        {
            if (!PermissionMediator.CanDelete(entity))
                throw new EntityPermissionException("Member delete requires elevated permissions.");

            await base.Delete(entity);
        }

        /// <summary>
        /// update
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override async Task<Member> Update(Member entity)
        {
            if (!PermissionMediator.CanUpdate(entity))
                throw new EntityPermissionException("Member update requires elevated permissions.");

            return await base.Update(entity);
        }
    }
}

