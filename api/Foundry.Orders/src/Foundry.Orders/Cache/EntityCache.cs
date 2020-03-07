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
using Foundry.Orders.Data;

namespace Foundry.Orders.Cache
{
    public class EntityCache<TEntity>
        where TEntity : class, IEntityGlobal
    {
        IMemoryCache _memoryCache;
        public EntityCache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public TEntity Get(string globalId)
        {
            if (_memoryCache.TryGetValue(globalId, out TEntity entity))
                return entity;

            return null;
        }

        public void Remove(string globalId)
        {
            if (_memoryCache.TryGetValue(globalId, out TEntity entity))
            {
                _memoryCache.Remove(globalId);
            }
        }

        public TEntity Set(TEntity entity)
        {
            if (entity == null)
                return null;

            return _memoryCache.Set(entity.GlobalId, entity);
        }
    }
}

