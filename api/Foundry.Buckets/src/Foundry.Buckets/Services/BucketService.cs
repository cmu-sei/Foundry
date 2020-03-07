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
    /// service to manage buckets
    /// </summary>
    public class BucketService : Service<IBucketRepository, Bucket>
    {
        BucketPermissionMediator BucketPermissionMediator { get; }

        /// <summary>
        /// create an instance of
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="bucketRepository"></param>
        /// <param name="mapper"></param>
        /// <param name="bucketPermissionMediator"></param>
        public BucketService(IStackIdentityResolver identityResolver, IBucketRepository bucketRepository, IMapper mapper, BucketPermissionMediator bucketPermissionMediator)
            : base(identityResolver, bucketRepository, mapper)
        {
            BucketPermissionMediator = bucketPermissionMediator ?? throw new ArgumentNullException(nameof(bucketPermissionMediator));
        }

        IQueryable<Bucket> Query()
        {
            var query = DbContext.Buckets
                .Include(b => b.BucketAccounts)
                .Include("BucketAccounts.Account")
                .Include(b => b.Files)
                .Include("Files.FileVersions");

            return BucketPermissionMediator.Process(query);
        }

        /// <summary>
        /// get all buckets
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<PagedResult<Bucket, BucketSummary>> GetAll(BucketDataFilter search = null)
        {
            return await PagedResult<Bucket, BucketSummary>(Query(), search);
        }

        /// <summary>
        /// get bucket by global id
        /// </summary>
        /// <param name="globalId"></param>
        /// <returns></returns>
        public async Task<BucketDetail> GetByGlobalId(string globalId)
        {
            return Map<BucketDetail>(await Query().SingleOrDefaultAsync(b => b.GlobalId.ToLower() == globalId.ToLower()));
        }

        /// <summary>
        /// get bucket by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<BucketDetail> GetById(int id)
        {
            return Map<BucketDetail>(await Query().SingleOrDefaultAsync(b => b.Id == id));
        }

        /// <summary>
        /// get default bucket for identity
        /// </summary>
        /// <returns></returns>
        public async Task<BucketDetail> GetDefaultBucket()
        {
            if (Identity == null)
                throw new InvalidIdentityException("Identity not found.");

            Bucket bucket = await Query().SingleOrDefaultAsync(b => b.BucketAccounts.Any(bs => bs.IsDefault && bs.AccountId.ToLower() == Identity.Id.ToLower()));

            if (bucket == null)
                return await Add(new BucketCreate { IsDefault = true, Name = Identity.Name, BucketSharingType = BucketSharingType.Public, Description = "Created on demand" });

            return Map<BucketDetail>(bucket);
        }

        /// <summary>
        /// determine the bucket to use based on the Identity
        /// if the ClientId relates to a Source that is configured as IsUploadOwner, this Sources default Bucket will be used
        /// </summary>
        /// <returns></returns>
        public async Task<BucketDetail> GetBucketForRequest()
        {
            var identity = Identity as ProfileIdentity;
            var clientId = identity.ClientId.ToLower();

            var applicationAccount = await DbContext.Accounts.SingleOrDefaultAsync(a => a.GlobalId.ToLower() == clientId && a.IsApplication);

            if (identity != null && applicationAccount != null && applicationAccount.IsUploadOwner)
            {
                var bucket = await DbContext.Buckets
                    .Include(b => b.BucketAccounts)
                    .Include("BucketAccounts.Account")
                    .Include(b => b.Files)
                    .Include("Files.FileVersions")
                    .SingleOrDefaultAsync(b => b.BucketAccounts.Any(ba => ba.IsDefault && ba.AccountId == applicationAccount.GlobalId));

                if (bucket == null)
                {
                    var model = new BucketCreate
                    {
                        BucketSharingType = BucketSharingType.Public,
                        IsDefault = true,
                        Name = applicationAccount.Name
                    };

                    bucket = await Add(applicationAccount, model);
                }

                var globalId = identity.Id.ToLower().Trim();

                if (!bucket.BucketAccounts.Any(ba => ba.AccountId.ToLower() == globalId))
                {
                    await AddBucketAccount(bucket.Id, new BucketAccountCreate { AccountId = globalId, BucketAccessType = BucketAccessType.Member, BucketId = bucket.Id });
                }

                return Map<BucketDetail>(bucket);
            }

            return await GetDefaultBucket();
        }

        /// <summary>
        /// create new bucket for identity
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<BucketDetail> Add(BucketCreate model)
        {
            var identity = Identity;
            var globalId = identity.Id.ToLower().Trim();

            if (identity == null)
                throw new EntityPermissionException("Not authenticated.");

            Bucket bucket = null;

            var account = await DbContext.Accounts.SingleOrDefaultAsync(t => t.GlobalId.ToLower() == globalId);

            if (account == null)
            {
                account = new Account { Name = identity.Name, GlobalId = globalId.ToLower() };

                if (identity is ClientIdentity)
                {
                    account.IsUploadOwner = true;
                }

                await DbContext.Accounts.AddAsync(account);
                await DbContext.SaveChangesAsync();
            }

            bucket = await Add(account, model);

            return Map<BucketDetail>(bucket);
        }

        async Task<Bucket> Add(Account account, BucketCreate model)
        {
            var globalId = Guid.NewGuid().ToString().ToLower();
            if (account.IsAdministrator && !string.IsNullOrWhiteSpace(model.GlobalId))
            {
                globalId = model.GlobalId;
            }

            var bucket = new Bucket
            {
                GlobalId = globalId,
                Name = model.Name,
                BucketSharingType = model.BucketSharingType,
                RestrictedKey = Guid.NewGuid().ToString().ToLower().Replace("-", ""),
                CreatedById = account.GlobalId
            };

            bucket.Description = Identity.ClientId;

            await DbContext.Buckets.AddAsync(bucket);
            await DbContext.SaveChangesAsync();

            var bucketAccountCreate = new BucketAccountCreate
            {
                AccountId = account.GlobalId,
                BucketId = bucket.Id,
                BucketAccessType = BucketAccessType.Owner,
                IsDefault = model.IsDefault };

            await AddBucketAccount(bucket.Id, bucketAccountCreate);

            return bucket;
        }

        /// <summary>
        /// add account to bucket
        /// </summary>
        /// <param name="bucketId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<BucketDetail> AddBucketAccount(int bucketId, BucketAccountCreate model)
        {
            if (bucketId != model.BucketId)
                throw new InvalidModelException("Bucket id mismatch.");

            if (DbContext.BucketAccounts.Any(ba => ba.BucketId == bucketId && ba.AccountId == model.AccountId))
                throw new EntityDuplicateException("Account is already a member of this Bucket.");

            var bucketAccount = new BucketAccount
            {
                BucketId = bucketId,
                IsDefault = model.IsDefault,
                AccountId = model.AccountId,
                BucketAccessType = model.BucketAccessType
            };

            await DbContext.BucketAccounts.AddAsync(bucketAccount);
            await DbContext.SaveChangesAsync();

            if (model.IsDefault)
            {
                await UpdateDefaultBucket(DbContext, model.AccountId, bucketId);
            }

            return await GetById(bucketId);
        }

        /// <summary>
        /// update bucket
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<BucketDetail> Update(BucketUpdate model)
        {
            if (Identity == null)
                throw new EntityPermissionException("Not authenticated.");

            var bucket = await Query().SingleOrDefaultAsync(b => b.Id == model.Id);

            if (bucket == null)
                throw new EntityNotFoundException("Bucket was not found.");

            if (!BucketPermissionMediator.CanPerformAction(bucket, ActionType.Update))
                throw new EntityPermissionException("Account does not have access to this bucket.");

            bucket.Name = model.Name;
            bucket.BucketSharingType = model.BucketSharingType;
            bucket.Description = model.Description;

            if (string.IsNullOrEmpty(bucket.RestrictedKey))
            {
                bucket.RestrictedKey = Guid.NewGuid().ToString().ToLower().Replace("-", "");
            }

            await DbContext.SaveChangesAsync();

            return Map<BucketDetail>(bucket);
        }

        /// <summary>
        /// set default bucket for current user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<BucketDetail> SetDefaultBucket(int id)
        {
            var identity = Identity;
            if (identity == null)
                throw new EntityPermissionException("Not authenticated.");

            var globalId = identity.Id.ToLower().Trim();

            var bucket = await Query().SingleOrDefaultAsync(b => b.Id == id);

            if (bucket == null)
                throw new EntityNotFoundException("Bucket was not found.");

            var bucketAccount = bucket.BucketAccounts.SingleOrDefault(ba => ba.AccountId.ToLower() == globalId);

            if (bucketAccount == null)
            {
                if (bucket.BucketSharingType == BucketSharingType.Public || BucketPermissionMediator.CanPerformAction(bucket, ActionType.Update))
                {
                    bucketAccount = new BucketAccount
                    {
                        AccountId = globalId,
                        BucketAccessType = BucketAccessType.Member,
                        BucketId = bucket.Id
                    };

                    bucket.BucketAccounts.Add(bucketAccount);
                }
                else
                {
                    throw new EntityPermissionException("Account does not have access to this bucket.");
                }
            }

            bucketAccount.IsDefault = true;

            await DbContext.SaveChangesAsync();

            await UpdateDefaultBucket(DbContext, globalId, bucket.Id);

            return Map<BucketDetail>(bucket);
        }

        /// <summary>
        /// ensure that identity only has one default bucket
        /// </summary>
        /// <param name="DbContext"></param>
        /// <param name="accountGlobalId"></param>
        /// <param name="currentDefaultBucketId"></param>
        /// <returns></returns>
        public static async Task UpdateDefaultBucket(BucketsDbContext DbContext, string accountGlobalId, int currentDefaultBucketId)
        {
            var globalId = accountGlobalId.ToLower().Trim();

            var bucketAccounts = DbContext.BucketAccounts.Where(bt => bt.AccountId == globalId && bt.BucketId != currentDefaultBucketId);
            await bucketAccounts.ForEachAsync(bt => bt.IsDefault = false);

            await DbContext.SaveChangesAsync();
        }
    }
}

