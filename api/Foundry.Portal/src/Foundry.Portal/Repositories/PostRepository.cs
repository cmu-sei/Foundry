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
using Foundry.Portal.Data;
using Foundry.Portal.Data.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Portal.Repositories
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        public PostRepository(SketchDbContext dbContext)
            : base(dbContext) { }

        public override async Task<Post> GetById(int id)
        {
            return await DbContext.Posts
                .Include(p => p.Attachments)
                .Include(p => p.Votes)
                .Include(p => p.Profile)
                .SingleOrDefaultAsync(p => p.Id == id);
        }

        public override IQueryable<Post> GetAll()
        {
            return DbContext.Posts
                .Include(p => p.Attachments)
                .Include(p => p.Votes)
                .Include(p => p.Profile);
        }

        /// <summary>
        /// delete post and children
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override async Task Delete(Post entity)
        {
            var children = await DbContext.Posts.Where(p => p.ParentId.HasValue && p.ParentId == entity.Id).ToListAsync();
            if (children.Any())
                DbContext.Posts.RemoveRange(children);

            await base.Delete(entity);
        }
    }
}

