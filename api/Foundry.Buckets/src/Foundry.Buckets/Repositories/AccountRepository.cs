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
using Foundry.Buckets.Data;
using Foundry.Buckets.Data.Entities;
using Foundry.Buckets.Data.Repositories;
using Stack.Patterns.Repository;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Buckets.Repositories
{
    /// <summary>
    /// account repository
    /// </summary>
    public class AccountRepository : Repository<BucketsDbContext, Account>, IAccountRepository
    {
        /// <summary>
        /// create an instance of account repository
        /// </summary>
        /// <param name="db"></param>
        public AccountRepository(BucketsDbContext db)
            : base(db) { }

        /// <summary>
        /// get all
        /// </summary>
        /// <returns></returns>
        public override IQueryable<Account> GetAll()
        {
            return DbContext.Accounts
                .Include(a => a.BucketAccounts)
                .Include("BucketAccounts.Account")
                .Include("BucketAccounts.Bucket");
        }

        /// <summary>
        /// get by globalId
        /// </summary>
        /// <param name="globalId"></param>
        /// <returns></returns>
        public async Task<Account> GetByGlobalId(string globalId)
        {
            return await DbContext.Accounts
                .Include(a => a.BucketAccounts)
                .Include("BucketAccounts.Account")
                .Include("BucketAccounts.Bucket")
                .SingleOrDefaultAsync(a => a.GlobalId == globalId);
        }
    }
}

