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
    /// member request repository
    /// </summary>
    public class MemberRequestRepository : Repository<GroupsDbContext, MemberRequest>, IMemberRequestRepository
    {
        MemberRequestPermissionMediator PermissionMediator { get; }

        public MemberRequestRepository(GroupsDbContext db, MemberRequestPermissionMediator permissionMediator)
            : base(db)
        {
            PermissionMediator = permissionMediator;
        }

        /// <summary>
        /// get all member requests
        /// </summary>
        /// <returns></returns>
        public override IQueryable<MemberRequest> GetAll()
        {
            var query = DbContext.MemberRequests
                .Include(gr => gr.Account)
                .Include(gr => gr.Group)
                .Include(gr => gr.Group.Members);

            return PermissionMediator.Process(query);
        }

        /// <summary>
        /// get member request
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task<MemberRequest> GetByIds(string groupId, string accountId)
        {
            var request = await DbContext.MemberRequests
                .Include(gr => gr.Account)
                .Include(gr => gr.Group)
                .Include(gr => gr.Group.Members)
                .SingleOrDefaultAsync(gr => gr.GroupId == groupId && gr.AccountId == accountId);

            return request;
        }

        /// <summary>
        /// get all by group id
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public IQueryable<MemberRequest> GetAllByGroupId(string groupId)
        {
            return GetAll()
                .Where(mr => mr.GroupId == groupId);
        }

        /// <summary>
        /// get all by account id
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public IQueryable<MemberRequest> GetAllByAccountId(string accountId)
        {
            return GetAll()
                .Where(mr => mr.AccountId == accountId);
        }

        public override Task<MemberRequest> Update(MemberRequest entity)
        {
            if (!PermissionMediator.CanUpdate(entity))
                throw new EntityPermissionException("Member request update requires elevated permissions.");

            return base.Update(entity);
        }

        public override Task Delete(MemberRequest entity)
        {
            if (!PermissionMediator.CanDelete(entity))
                throw new EntityPermissionException("Member request update requires elevated permissions.");

            return base.Delete(entity);
        }
    }
}

