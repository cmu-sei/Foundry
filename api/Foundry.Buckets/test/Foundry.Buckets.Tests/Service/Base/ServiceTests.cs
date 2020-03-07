/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Foundry.Buckets.Data;
using Foundry.Buckets.Data.Entities;
using Foundry.Buckets.Identity;
using Foundry.Buckets.Repositories;
using Foundry.Buckets.Security;
using Foundry.Buckets.Services;
using System;

namespace Foundry.Buckets.Tests.Service
{
    public abstract class ServiceTests
    {
        protected FileService CreateFileService(TestContext<BucketsDbContext> context)
        {
            return new FileService(
                context.IdentityResolver,
                new FileRepository(context.DbContext),
                context.Mapper,
                context.HostingEnvironment,
                context.StorageProvider,
                new BucketPermissionMediator(context.DbContext, context.IdentityResolver));
        }

        protected BucketService CreateBucketService(TestContext<BucketsDbContext> context)
        {
            return new BucketService(
                context.IdentityResolver,
                new BucketRepository(context.DbContext),
                context.Mapper,
                new BucketPermissionMediator(context.DbContext, context.IdentityResolver));
        }

        public const string DefaultClientId = "test-client";

        public static void InitializeProfileIdentity(TestContext<BucketsDbContext> ctx, string clientId = DefaultClientId)
        {
            var globalId = Guid.NewGuid().ToString().ToLower();
            var target = new Account
            {
                GlobalId = globalId,
                Name = globalId
            };

            ctx.DbContext.Accounts.Add(target);
            ctx.DbContext.SaveChanges();

            ctx.Identity = new ProfileIdentity
            {
                Id = target.GlobalId,
                Name = target.Name,
                Permissions = new string[] { },
                ClientId = clientId,
                Subject = target.GlobalId
            };
        }

        public static Account CreateClientIdentity(TestContext<BucketsDbContext> ctx)
        {
            var source = new Account
            {
                Name = DefaultClientId,
                GlobalId = DefaultClientId,
                IsUploadOwner = true
            };
            ctx.DbContext.Accounts.Add(source);
            ctx.DbContext.SaveChanges();

            var bucket = new Bucket
            {
                BucketSharingType = BucketSharingType.Public,
                CreatedById = source.GlobalId.ToLower(),
                Name = DefaultClientId + "-bucket"
            };

            var bucketSource = new BucketAccount
            {
                BucketAccessType = BucketAccessType.Owner,
                IsDefault = true
            };

            bucket.BucketAccounts.Add(bucketSource);
            ctx.DbContext.Buckets.Add(bucket);
            return source;
        }
    }
}

