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
using System.Linq;
using System.Threading.Tasks;

namespace Stack.Patterns.Repository
{
    /// <summary>
    /// base repository class with override-able implementation based on a DbContext
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class Repository<TDbContext, TEntity> : IRepository<TDbContext, TEntity>
        where TDbContext: DbContext
        where TEntity : class
    {
        public TDbContext DbContext { get; private set; }

        public Repository(TDbContext db)
        {
            DbContext = db;
        }

        public virtual DbSet<TEntity> Set()
        {
            return DbContext.Set<TEntity>();
        }

        public virtual async Task Delete(TEntity entity)
        {
            Set().Remove(entity);
            await DbContext.SaveChangesAsync();
            return;
        }

        public virtual async Task DeleteById(int id)
        {
            var entity = await GetById(id);
            await Delete(entity);
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return Set();
        }

        public virtual async Task<TEntity> GetById(int id)
        {
            return await Set().FindAsync(id);
        }

        public virtual async Task<TEntity> Update(TEntity entity)
        {
            await DbContext.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<TEntity> Add(TEntity entity)
        {
            await Set().AddAsync(entity);
            await DbContext.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<bool> Exists(int id)
        {
            return (await Set().FindAsync(id)) != null;
        }
    }
}
