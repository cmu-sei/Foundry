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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Portal.Repositories
{
    public class TagRepository : Repository<Tag>, ITagRepository
    {
        public TagRepository(SketchDbContext dbContext)
            : base(dbContext) { }

        public override IQueryable<Tag> GetAll()
        {
            return DbContext.Tags
                .Include(t => t.ContentTags);
        }

        public async Task AddContentTags(IEnumerable<ContentTag> contentTags)
        {
            var contentIds = contentTags.Select(ct => ct.ContentId).ToArray().Distinct();
            var remove = await DbContext.ContentTags.Where(ct => contentIds.Contains(ct.ContentId)).ToListAsync();

            DbContext.ContentTags.RemoveRange(remove);

            await DbContext.ContentTags.AddRangeAsync(contentTags);

            await DbContext.SaveChangesAsync();
        }

        public async Task AddPlaylistTags(IEnumerable<PlaylistTag> playlistTags)
        {
            var playlistIds = playlistTags.Select(ct => ct.PlaylistId).ToArray().Distinct();
            var remove = await DbContext.PlaylistTags.Where(ct => playlistIds.Contains(ct.PlaylistId)).ToListAsync();

            DbContext.PlaylistTags.RemoveRange(remove);

            await DbContext.PlaylistTags.AddRangeAsync(playlistTags);

            await DbContext.SaveChangesAsync();
        }

        public async Task<Tag> GetByName(string name)
        {
            return await DbContext.Tags
                .Include(t => t.ContentTags)
                .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());
        }

        public override async Task<Tag> GetById(int id)
        {
            return await DbContext.Tags
                .Include(t => t.ContentTags)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public IQueryable<Tag> GetAllByName(string[] tags)
        {
            var value = tags.Select(t => t.ToLower()).ToArray();

            var query = DbContext.Tags
                .Include(t => t.ContentTags)
                .Where(t => value.Contains(t.Name.ToLower()));

            return query;
        }
    }
}
