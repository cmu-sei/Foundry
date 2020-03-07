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
using Foundry.Buckets.Data;
using Foundry.Buckets.Data.Entities;
using Foundry.Buckets.Data.Repositories;
using Foundry.Buckets.Identity;
using Foundry.Buckets.Security;
using Foundry.Buckets.ViewModels;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Patterns.Service.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Buckets.Services
{
    /// <summary>
    /// bucket source service
    /// </summary>
    public class BucketAccountService : Service<IBucketAccountRepository, BucketAccount>
    {
        BucketPermissionMediator BucketPermissionMediator { get; }

        /// <summary>
        /// creates an instance of bucket source service
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="bucketAccountRepository"></param>
        /// <param name="mapper"></param>
        /// <param name="bucketPermissionMediator"></param>
        public BucketAccountService(IStackIdentityResolver identityResolver, IBucketAccountRepository bucketAccountRepository, IMapper mapper, BucketPermissionMediator bucketPermissionMediator)
            : base(identityResolver, bucketAccountRepository, mapper)
        {
            BucketPermissionMediator = bucketPermissionMediator ?? throw new ArgumentNullException(nameof(bucketPermissionMediator));
        }

        IQueryable<BucketAccount> Query()
        {
            var query = DbContext.Buckets
                .Include(b => b.BucketAccounts)
                .Include("BucketAccounts.Account")
                .Include(b => b.Files)
                .Include("Files.FileVersions");

            return BucketPermissionMediator.Process(query)
                .SelectMany(b => b.BucketAccounts);
        }

        /// <summary>
        /// get all sources by bucket id
        /// </summary>
        /// <param name="bucketId"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<PagedResult<BucketAccount, BucketAccountSummary>> GetAll(int bucketId, BucketAccountDataFilter search = null)
        {
            return await PagedResult<BucketAccount, BucketAccountSummary>(Query(), search);
        }

        /// <summary>
        /// delete bucket account
        /// </summary>
        /// <param name="bucketId"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task<bool> Delete(int bucketId, string accountId)
        {
            if (Identity == null)
                throw new EntityPermissionException("Action requires elevated permissions.");

            if (bucketId <= 0)
                throw new InvalidModelException("Bucket ID is invalid.");

            if (string.IsNullOrWhiteSpace(accountId))
                throw new InvalidModelException("Account ID is invalid.");

            var query = Query();

            var bucketAccount = await Query().SingleOrDefaultAsync(ba => ba.BucketId == bucketId && ba.AccountId.ToLower() == accountId.ToLower());

            if (bucketAccount == null)
                throw new EntityNotFoundException("Bucket account was not found.");

            if (!(await HasBucketAccessType(bucketAccount.BucketId, BucketAccessType.Owner)))
                throw new EntityPermissionException("Action requires elevated permissions.");

            if (Identity is ClientIdentity)
            {
                if (bucketAccount.AccountId.ToLower() == Identity.Id.ToLower())
                    throw new EntityPermissionException("You cannot remove your own account from this bucket.");
            }

            DbContext.BucketAccounts.Remove(bucketAccount);
            await DbContext.SaveChangesAsync();

            return true;
        }
    }
}

