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
using Foundry.Portal.Security;
using Stack.Http.Exceptions;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Foundry.Portal.Repositories
{
    public class ContentRepository : Repository<Content>, IContentRepository
    {
        ContentPermissionMediator _contentPermissionMediator;

        public ContentRepository(SketchDbContext dbContext, ContentPermissionMediator contentPermissionMediator)
            : base(dbContext)
        {
            _contentPermissionMediator = contentPermissionMediator ?? throw new ArgumentNullException(nameof(contentPermissionMediator));
        }

        IQueryable<Content> GetAllQuery(Expression<Func<Content, bool>> expression)
        {
            return GetAllQuery().Where(expression);
        }

        IQueryable<Content> GetAllQuery()
        {
            var query = DbContext.Contents
                .Include(c => c.ContentTags)
                .Include("ContentTags.Tag")
                .Include(c => c.ProfileContents)
                .Include("ProfileContents.Profile")
                .Include(c => c.Author);

            return _contentPermissionMediator.Process(query);
        }

        async Task<Content> GetQuery(Expression<Func<Content, bool>> expression)
        {
            var query = DbContext.Contents
                .Include(c => c.ContentTags)
                .Include("ContentTags.Tag")
                .Include(c => c.ProfileContents)
                .Include("ProfileContents.Profile")
                .Include(c => c.SectionContents)
                .Include("SectionContents.Section")
                .Include("SectionContents.Section.Playlist")
                .Include(c => c.Discussions)
                .Include(c => c.Author)
                .Where(expression);

            return await _contentPermissionMediator.Process(query).SingleOrDefaultAsync();
        }

        public async override Task<Content> GetById(int id)
        {
            return await GetQuery(c => c.Id == id);
        }

        public async Task<Content> GetByGlobalId(string globalId)
        {
            return await GetQuery(c => c.GlobalId.ToLower() == globalId.ToLower());
        }

        public override IQueryable<Content> GetAll()
        {
            return GetAllQuery();
        }

        public IQueryable<Content> GetAllByProfileId(int id)
        {
            return GetAllQuery(c => c.AuthorId == id);
        }

        public IQueryable<Content> GetAllByPlaylistId(int id)
        {
            var query = DbContext.Contents
                .Include(c => c.SectionContents)
                .Include("SectionContents.Section")
                .Include("SectionContents.Section.Playlist")
                .Include(c => c.ContentTags)
                .Include("ContentTags.Tag")
                .Where(c => c.SectionContents.Any(pc => pc.Section.PlaylistId == id));

            return _contentPermissionMediator.Process(query);
        }

        public async Task<ProfileContent> GetProfileContent(int contentId, int profileId)
        {
            return await DbContext.ProfileContents
                .Include(pc => pc.Content)
                .Include(pc => pc.Profile)
                .SingleOrDefaultAsync(c => c.ContentId == contentId && c.ProfileId == profileId);
        }

        public async override Task DeleteById(int id)
        {
            var content = await DbContext.Contents.SingleOrDefaultAsync(c => c.Id == id);

            if (content == null)
                return;

            if (!_contentPermissionMediator.CanDelete(content))
            {
                throw new EntityPermissionException("You do not have permission to delete this content.");
            }

            var profileContents = DbContext.ProfileContents.Where(p => p.ContentId == id).ToArray();
            var discussions = DbContext.Discussions.Where(p => p.ContentId == id).ToArray();

            DbContext.ProfileContents.RemoveRange(profileContents);
            DbContext.Discussions.RemoveRange(discussions);
            DbContext.Remove(content);

            await DbContext.SaveChangesAsync();
        }

        public async Task<bool> IsDisabled(int id)
        {
            return await DbContext.Contents.AnyAsync(c => c.Id == id && c.IsDisabled);
        }

        public async override Task<Content> Update(Content content)
        {
            if (!_contentPermissionMediator.CanUpdate(content))
            {
                throw new EntityPermissionException("You do not have permission to update this content.");
            }

            return await base.Update(content);
        }

        public async override Task Delete(Content content)
        {
            await DeleteById(content.Id);
        }
    }
}

