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
    /// account repository
    /// </summary>
    public class AccountRepository : Repository<GroupsDbContext, Account>, IAccountRepository
    {
        AccountPermissionMediator PermissionMediator { get; }

        public AccountRepository(GroupsDbContext db, AccountPermissionMediator permissionMediator)
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
            return DbContext.Accounts.Any(g => g.Id == id);
        }

        /// <summary>
        /// get all accounts
        /// </summary>
        /// <returns></returns>
        public override IQueryable<Account> GetAll()
        {
            var query = DbContext.Accounts;

            return PermissionMediator.Process(query);
        }

        /// <summary>
        /// get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Account> GetById(string id)
        {
            var query = DbContext.Accounts;

            return await PermissionMediator.Process(query).SingleOrDefaultAsync(g => g.Id == id);
        }

        /// <summary>
        /// delete
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override async Task Delete(Account entity)
        {
            if (!PermissionMediator.CanDelete(entity))
                throw new EntityPermissionException("Account delete requires elevated permissions.");

            await base.Delete(entity);
        }

        /// <summary>
        /// update
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override async Task<Account> Update(Account entity)
        {
            if (!PermissionMediator.CanUpdate(entity))
                throw new EntityPermissionException("Account update requires elevated permissions.");

            return await base.Update(entity);
        }
    }
}

