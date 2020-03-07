/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.Extensions.Caching.Memory;
using Foundry.Buckets.Data;
using System;

namespace Foundry.Buckets.Cache
{
    /// <summary>
    /// basic memory cache for entities
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class EntityCache<TEntity>
        where TEntity : class, IGlobal
    {
        IMemoryCache _memoryCache;
        BucketsDbContext _db;

        /// <summary>
        /// create an instance of EntityCache
        /// </summary>
        /// <param name="memoryCache"></param>
        /// <param name="db"></param>
        public EntityCache(IMemoryCache memoryCache, BucketsDbContext db)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        /// <summary>
        /// get cached entity by globalId
        /// </summary>
        /// <param name="globalId"></param>
        /// <returns></returns>
        public TEntity GetOrCreate(string globalId)
        {
            if (string.IsNullOrWhiteSpace(globalId))
                return null;

            if (_memoryCache.TryGetValue(globalId.ToLower(), out TEntity result))
                return result;

            var entity = _db.Set<TEntity>().Find(globalId);

            return GetOrCreate(entity);
        }

        /// <summary>
        /// get or create entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TEntity GetOrCreate(TEntity entity)
        {
            if (entity == null)
                return null;

            if (_memoryCache.TryGetValue(entity.GlobalId.ToLower(), out TEntity result))
                return result;

            _memoryCache.Set(entity.GlobalId.ToLower(), entity);

            return entity;
        }

        /// <summary>
        /// remove cached entity by globalId
        /// </summary>
        /// <param name="globalId"></param>
        public void Remove(string globalId)
        {
            if (string.IsNullOrWhiteSpace(globalId))
                return;

            if (_memoryCache.TryGetValue(globalId.ToLower(), out TEntity entity))
            {
                _memoryCache.Remove(globalId);
            }
        }
    }
}

