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
using Microsoft.Extensions.Logging;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Patterns.Service;
using Stack.Patterns.Service.Models;
using Foundry.Portal.Data;
using Foundry.Portal.Data.Entities;
using Foundry.Portal.Extensions;
using Foundry.Portal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundry.Portal.Security;

namespace Foundry.Portal.Services
{
    public class TagService : Service<Tag>
    {
        ITagRepository _tagRepository;
        ContentPermissionMediator _contentPermissionMediator;

        public TagService(ITagRepository tagRepository, CoreOptions options, IStackIdentityResolver userResolver, ILoggerFactory loggerFactory, IMapper mapper)
            : base(options, userResolver, loggerFactory, mapper)
        {
            _tagRepository = tagRepository ?? throw new ArgumentNullException("tagRepository");
            _contentPermissionMediator = new ContentPermissionMediator(userResolver);
        }

        /// <summary>
        /// move to a database setting
        /// </summary>
        public bool AllowUserDefinedTags { get; set; } = false;

        public async Task<TagDetail> Add(TagCreate model)
        {
            var tag = new Tag
            {
                Name = model.Name,
                Description = model.Description,
                TagType = model.TagType,
                TagSubType = model.TagSubType
            };

            var result = await _tagRepository.Add(tag);
            return Map<TagDetail>(result);
        }

        public async Task<TagDetail> Update(int id, TagUpdate model)
        {
            if (!Identity.Permissions.Contains(SystemPermissions.PowerUser))
                throw new EntityPermissionException("Action requires elevated permissions.");

            var tag = await _tagRepository.GetById(id);

            if (tag == null)
                throw new EntityNotFoundException("Tag was not found.");

            var updateWarehousedTags = tag.Name != model.Name;

            tag.Name = model.Name;
            tag.TagType = model.TagType;
            tag.TagSubType = model.TagSubType;
            tag.Description = model.Description;

            var result = await _tagRepository.Update(tag);

            if (updateWarehousedTags)
            {
                // update all associated content warehoused tag name strings
                var contents = await _tagRepository.DbContext.Contents
                    .Include(c => c.ContentTags)
                    .Include("ContentTags.Tag")
                    .Where(c => c.ContentTags.Any(ct => ct.TagId == id)).ToListAsync();

                foreach (var content in contents)
                {
                    var tags = content.ContentTags.Select(ct => ct.Tag.Name).ToArray();
                    content.Tags = ConvertTagsToDelmitedString(tags);
                }
            }

            return Map<TagDetail>(result);
        }

        public async Task<TagDetail> Update(string name, TagUpdate model)
        {
            if (!Identity.Permissions.Contains(SystemPermissions.PowerUser))
                throw new EntityPermissionException("Action requires elevated permissions.");

            var tag = await _tagRepository.GetByName(name);

            if (tag == null)
                throw new EntityNotFoundException("Tag was not found.");

            return await Update(tag.Id, model);
        }

        public async Task<PagedResult<Tag, TagSummary>> GetAll(TagDataFilter search)
        {
            return await PagedResult<Tag, TagSummary>(_tagRepository.GetAll(), search);
        }

        public async Task<TagDetail> GetByName(string name)
        {
            var tag = await _tagRepository.GetByName(name);

            return Map<TagDetail>(tag);
        }

        public string ConvertTagsToDelmitedString(params string[] tags)
        {
            if (tags == null || !tags.Any())
                return string.Empty;

            return string.Join(Extensions.StringExtensions.TagDelimiter, tags.Select(t => t.ToUrlString()));
        }

        public IEnumerable<ContentTag> BuildContentTags(int contentId, string[] tags)
        {
            if (tags == null || !tags.Any())
                return new List<ContentTag>();

            var entityTags = GetAndCreateTagsFromTagNames(tags);

            return entityTags.Select(t => new ContentTag { Tag = t, ContentId = contentId });
        }

        public IEnumerable<PlaylistTag> BuildPlaylistTags(int playlistId, string[] tags)
        {
            if (tags == null || !tags.Any())
                return new List<PlaylistTag>();

            var entityTags = GetAndCreateTagsFromTagNames(tags);

            return entityTags.Select(t => new PlaylistTag { Tag = t, PlaylistId = playlistId });
        }

        public IEnumerable<Tag> GetAndCreateTagsFromTagNames(string[] tags)
        {
            if (tags == null || !tags.Any())
                return new List<Tag>();

            var found = _tagRepository.GetAllByName(tags);

            var notFound = tags.Select(t => t.ToLower())
                .Except(found.Select(t => t.Name.ToLower()));

            var result = new List<Tag>();

            result.AddRange(found);

            foreach (var create in notFound)
            {
                result.Add(new Tag() { Name = create });
            }

            return result;
        }

        /// <summary>
        /// Updates a Content item's tags list
        /// </summary>
        /// <param name="contentId">The id of the content item</param>
        /// <param name="tags">The id's of the tags</param>
        /// <returns>True on success</returns>
        public async Task<bool> SetContentTags(int contentId, params string[] tags)
        {
            var content = await _tagRepository.DbContext.Contents.Where(c => c.Id == contentId).FirstOrDefaultAsync();

            if (content == null || !_contentPermissionMediator.CanUpdate(content))
            {
                throw new EntityPermissionException("Action requires elevated permissions.");
            }

            if (tags != null && tags.Any())
            {
                var contentTags = BuildContentTags(contentId, tags);

                await _tagRepository.AddContentTags(contentTags);
            }
            else
            {
                var contentTags = await _tagRepository.DbContext.ContentTags.Where(ct => ct.ContentId == contentId).ToListAsync();
                _tagRepository.DbContext.ContentTags.RemoveRange(contentTags);
                await _tagRepository.DbContext.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> SetPlaylistTags(int playlistId, params string[] tags)
        {
            if (tags != null && tags.Any())
            {
                var playlistTags = BuildPlaylistTags(playlistId, tags);

                await _tagRepository.AddPlaylistTags(playlistTags);
            }
            else
            {
                var playlistTags = await _tagRepository.DbContext.PlaylistTags.Where(ct => ct.PlaylistId == playlistId).ToListAsync();
                _tagRepository.DbContext.PlaylistTags.RemoveRange(playlistTags);
                await _tagRepository.DbContext.SaveChangesAsync();
            }

            return true;
        }

        public async Task<TagDetail[]> AddTags(string type, string[] tags)
        {
            if (!Identity.Permissions.Contains(SystemPermissions.PowerUser))
                throw new EntityPermissionException("Adding tags requires elevated permissions.");

            var _db = _tagRepository.DbContext;

            var result = new List<Tag>();
            foreach (string tag in tags)
            {
                if (tag.Length > 50)
                    continue;

                Tag found = await _db.Tags.Where(o => o.Name == tag && o.TagType.ToLower() == type.ToLower()).SingleOrDefaultAsync();
                if (found == null)
                {
                    found = new Tag { Name = tag, TagType = type.ToLower() };
                    _db.Tags.Add(found);
                }
                result.Add(found);
            }
            await _db.SaveChangesAsync();

            return result.Select(t => Map<TagDetail>(t)).ToArray();
        }

        /// <summary>
        /// delete tags by ids
        /// </summary>
        /// <param name="tagIds"></param>
        /// <returns></returns>
        public async Task<bool> Delete(int[] tagIds)
        {
            if (!Identity.Permissions.Contains(SystemPermissions.PowerUser))
                throw new EntityPermissionException("Deleting tags requires elevated permissions.");

            var tags = await _tagRepository.DbContext.Tags.Where(t => tagIds.Contains(t.Id)).ToListAsync();

            foreach (var tag in tags)
            {
                await Delete(tag);
            }

            return true;
        }

        /// <summary>
        /// delete tag regardless of it being used
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<bool> Delete(string name)
        {
            if (!Identity.Permissions.Contains(SystemPermissions.PowerUser))
                throw new EntityPermissionException("Deleting tags requires elevated permissions.");

            var tag = await _tagRepository.DbContext.Tags.SingleOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());

            if (tag == null)
                throw new EntityNotFoundException("Tag '" + name + "' was not found.");

            return await Delete(tag);
        }

        /// <summary>
        /// delete tag by id and reset content and playlist tags
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> Delete(Tag tag)
        {
            if (!Identity.Permissions.Contains(SystemPermissions.PowerUser))
                throw new EntityPermissionException("Deleting tags requires elevated permissions.");

            if (tag == null)
                throw new EntityNotFoundException("Tag was not found.");

            await DeleteContentTags(tag);
            await DeletePlaylistTags(tag);

            _tagRepository.DbContext.Tags.Remove(tag);

            await _tagRepository.DbContext.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// delete tag from contents and set tags
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        async Task DeleteContentTags(Tag tag)
        {
            var contents = await _tagRepository.DbContext.Contents
                .Include(c => c.ContentTags)
                .Include("ContentTags.Tag")
                .Where(c => c.ContentTags.Any(ct => ct.TagId == tag.Id))
                .ToListAsync();

            foreach (var content in contents)
            {
                var trimmedTags = content.ContentTags.Where(ct => ct.TagId != tag.Id).Select(ct => ct.Tag.Name).ToArray();
                await SetContentTags(content.Id, trimmedTags);
            }
        }

        /// <summary>
        /// delete tag from playlists and set tags
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        async Task DeletePlaylistTags(Tag tag)
        {
            var playlists = await _tagRepository.DbContext.Playlists
                .Include(c => c.PlaylistTags)
                .Include("PlaylistTags.Tag")
                .Where(c => c.PlaylistTags.Any(ct => ct.TagId == tag.Id)).ToListAsync();

            foreach (var playlist in playlists)
            {
                var trimmedTags = playlist.PlaylistTags.Where(pt => pt.TagId != tag.Id).Select(ct => ct.Tag.Name).ToArray();
                await SetPlaylistTags(playlist.Id, trimmedTags);
            }
        }
    }
}
