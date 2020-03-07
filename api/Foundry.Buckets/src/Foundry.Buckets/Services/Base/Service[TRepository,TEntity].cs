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
using Foundry.Buckets.Identity;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Patterns.Repository;
using Stack.Patterns.Service;
using Stack.Patterns.Service.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Buckets.Services
{
    /// <summary>
    /// base service class to expose common methods
    /// </summary>
    /// <typeparam name="TRepository"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class Service<TRepository, TEntity> : StackIdentityRepositoryService<BucketsDbContext, TRepository, TEntity>
        where TRepository : class, IRepository<BucketsDbContext, TEntity>
        where TEntity : class, new()
    {
        /// <summary>
        /// injected Mapper configuration
        /// </summary>
        protected IMapper Mapper { get; }

        IBucketsIdentity _identity;
        public new IBucketsIdentity Identity
        {
            get
            {
                if (_identity == null)
                {
                    _identity = base.Identity as IBucketsIdentity;
                }

                return _identity;
            }
        }

        /// <summary>
        /// is current user an administrator
        /// </summary>
        protected bool IsAdministrator
        {
            get
            {
                if (Identity == null)
                    return false;

                return Identity.Permissions.Contains("administrator");
            }
        }

        /// <summary>
        /// creates an instance of the base service class
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="repository"></param>
        /// <param name="mapper"></param>
        public Service(IStackIdentityResolver identityResolver, TRepository repository, IMapper mapper)
            : base(identityResolver, repository, mapper)
        {
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// check if identity has access type
        /// </summary>
        /// <param name="bucketId"></param>
        /// <param name="bucketAccessType"></param>
        /// <returns></returns>
        protected async Task<bool> HasBucketAccessType(int bucketId, params BucketAccessType[] bucketAccessType)
        {
            if (IsAdministrator)
                return true;

            if (bucketAccessType == null || !bucketAccessType.Any())
                return false;

            return await DbContext.Buckets.AnyAsync(b =>
                b.Id == bucketId &&
                b.BucketAccounts.Any(ba => ba.AccountId.ToLower() == Identity.Id.ToLower() && bucketAccessType.Contains(ba.BucketAccessType)));
        }

        /// <summary>
        /// get bucket ids for identity with specified access types
        /// </summary>
        /// <param name="bucketAccessType"></param>
        /// <returns></returns>
        protected async Task<int[]> GetBucketIdsByAccessType(params BucketAccessType[] bucketAccessType)
        {
            int[] ids = new int[] { };
            if (bucketAccessType != null && bucketAccessType.Any())
            {
                ids = await DbContext.Buckets.Where(b => b.BucketAccounts.Any(ba =>
                    ba.AccountId.ToLower() == Identity.Id.ToLower() &&
                    bucketAccessType.Contains(ba.BucketAccessType))).Select(b => b.Id).ToArrayAsync();
            }

            return ids;
        }


        /// <summary>
        /// checks if identity has access to bucket
        /// </summary>
        /// <param name="bucketId"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task<bool> HasBucketAccount(int bucketId, string accountId)
        {
            if (Identity == null)
                throw new InvalidIdentityException("Identity not found.");

            return await DbContext.BucketAccounts.AnyAsync(bs => bs.BucketId == bucketId && bs.AccountId.ToLower() == accountId.ToLower());
        }

        public TType Map<TType>(object source)
        {
            return Map<TType>(source, opts => {
                var identity = Identity;
                if (identity != null)
                {
                    if (identity is ProfileIdentity)
                    {
                        opts.Items["Profile"] = identity;
                    }

                    if (identity is ClientIdentity)
                    {
                        opts.Items["Client"] = identity;
                    }
                }
            });
        }

        TType Map<TType>(object source, Action<IMappingOperationOptions> opts)
        {
            try
            {
                if (opts == null)
                    return Mapper.Map<TType>(source);

                return Mapper.Map<TType>(source, opts);
            }
            catch (AutoMapperMappingException ex)
            {
                string message = string.Format("Could not map '{0}' to '{1}'",
                    source.GetType().Name, typeof(TType).Name);

                throw new AutoMapperMappingException(message, ex);
            }
        }

        /// <summary>
        /// get a paged result by generic type and always add ProfileId for automapper options
        /// </summary>
        /// <typeparam name="TEntityModel"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="query"></param>
        /// <param name="dataFilter"></param>
        /// <returns></returns>
        public async Task<PagedResult<TEntityModel, TViewModel>> PagedResult<TEntityModel, TViewModel>(
            IQueryable<TEntityModel> query,
            IDataFilter<TEntityModel> dataFilter)
            where TEntityModel : class
            where TViewModel : class
        {
            return await PagedResultFactory.Execute<TEntityModel, TViewModel>(query, dataFilter, Identity);
        }
    }
}
