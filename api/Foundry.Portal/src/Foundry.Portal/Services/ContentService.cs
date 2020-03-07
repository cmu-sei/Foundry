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
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Foundry.Portal.Data;
using Foundry.Portal.Data.Entities;
using Foundry.Portal.Events;
using Foundry.Portal.Export;
using Foundry.Portal.Extensions;
using Foundry.Portal.Import;
using Foundry.Portal.Repositories;
using Foundry.Portal.Security;
using Foundry.Portal.ViewModels;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Patterns.Service;
using Stack.Patterns.Service.Models;
using Stack.Validation.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Foundry.Portal.Services
{
    /// <summary>
    /// content service
    /// </summary>
    public class ContentService : DispatchService<Content>
    {
        readonly IContentRepository _contentRepository;
        readonly ITagRepository _tagRepository;
        readonly IDiscussionRepository _discussionRepository;
        readonly IProfileRepository _profileRepository;
        readonly ContentPermissionMediator _contentPermissionMediator;
        readonly string[] _reservedTags = new string[] { "_recommended", "_new", "_popular" };

        const ChannelPermission _unrestrictedPermissions = ChannelPermission.Browse_Content | ChannelPermission.Launch_Content;

        public IValidationHandler ValidationHandler { get; }
        public SketchDbContext DbContext { get { return _contentRepository.DbContext; } }

        /// <summary>
        /// create an instance of content servce
        /// </summary>
        /// <param name="contentRepository"></param>
        /// <param name="tagRepository"></param>
        /// <param name="discussionRepository"></param>
        /// <param name="profileRepository"></param>
        /// <param name="validationHandler"></param>
        /// <param name="domainEventDispatcher"></param>
        /// <param name="options"></param>
        /// <param name="identityResolver"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="mapper"></param>
        public ContentService(
            IContentRepository contentRepository,
            ITagRepository tagRepository,
            IDiscussionRepository discussionRepository,
            IProfileRepository profileRepository,
            IValidationHandler validationHandler,
            IDomainEventDispatcher domainEventDispatcher,
            CoreOptions options,
            IStackIdentityResolver identityResolver,
            ILoggerFactory loggerFactory,
            IMapper mapper)
            : base(domainEventDispatcher, options, identityResolver, loggerFactory, mapper)
        {
            _contentRepository = contentRepository ?? throw new ArgumentNullException("contentRepository");
            _tagRepository = tagRepository ?? throw new ArgumentNullException("tagRepository");
            _discussionRepository = discussionRepository ?? throw new ArgumentNullException("discussionRepository");
            _profileRepository = profileRepository ?? throw new ArgumentNullException("profileRepository");
            ValidationHandler = validationHandler ?? throw new ArgumentNullException("validationHandler");
            _contentPermissionMediator = new ContentPermissionMediator(IdentityResolver);
        }

        async Task<int> GetAuthor(string authorId)
        {
            int result = Identity.GetId();

            if (!string.IsNullOrWhiteSpace(authorId))
            {
                var author = await DbContext.Profiles.SingleOrDefaultAsync(p => p.GlobalId.ToLower() == authorId);
                if (author != null)
                {
                    result = author.Id;
                }
            }

            return result;
        }

        /// <summary>
        /// add content
        /// </summary>
        /// <param name="model"></param>
        /// <param name="imported"></param>
        /// <returns></returns>
        public async Task<ContentDetail> Add(ContentCreate model, bool imported = false)
        {
            await ValidationHandler.ValidateRulesFor(model);

            var tagService = new TagService(_tagRepository, Options, IdentityResolver, LoggerFactory, Mapper);

            Content entity = new Content
            {
                Name = model.Name,
                Description = model.Description,
                Summary = model.Summary,
                Copyright = model.Copyright,
                Type = model.Type,
                GlobalId = model.GlobalId,
                AuthorId = await GetAuthor(model.AuthorId),
                PublisherId = model.PublisherId,
                PublisherName = model.PublisherName,
                PublisherSlug = model.PublisherSlug,
                Order = model.Order,
                Url = model.Url,
                LogoUrl = model.LogoUrl,
                HoverUrl = model.HoverUrl,
                ThumbnailUrl = model.ThumbnailUrl,
                TrailerUrl = model.TrailerUrl,
                Settings = model.Settings,
                CreatedBy = Identity.Id,
            };

            if (imported)
            {
                entity.Imported = DateTime.UtcNow;
                entity.ImportedBy = Identity.Id;
            }

            entity.Start = entity.Start.Set(model.StartDate, model.StartTime);

            if (entity.Start.HasValue)
            {
                entity.End = entity.End.Set(model.EndDate, model.EndTime);

                if (!entity.End.HasValue)
                {
                    entity.End = entity.Start;
                }
            }
            else
            {
                entity.End = null;
            }

            entity.IsDisabled = model.IsDisabled;
            entity.FeaturedOrder = model.FeaturedOrder;

            if (Identity.Permissions.Contains(SystemPermissions.PowerUser))
            {
                entity.IsRecommended = model.IsRecommended;
                entity.IsFeatured = model.IsFeatured;
            }

            entity.Tags = tagService.ConvertTagsToDelmitedString(model.Tags);

            DiscussionService discussionService = new DiscussionService(_discussionRepository, Options, IdentityResolver, LoggerFactory, Mapper);

            discussionService.AddDiscussionToContent(entity);

            await _contentRepository.Add(entity);

            await tagService.SetContentTags(entity.Id, model.Tags);

            Dispatch(new DomainEvent(entity.GlobalId, entity.Name, Identity.Id, DomainEventType.ContentAdd));

            return await GetById(entity.Id);
        }

        public async Task<List<string>> GetDashboardTags()
        {
            var tags = new List<string>(_reservedTags);
            var popularTags = (await _contentRepository.DbContext.Tags.OrderByDescending(t => t.ContentTags.Count()).Take(10).ToListAsync()).Select(ct => ct.Name);
            tags.AddRange(popularTags);

            return tags.Select(t => t.Trim().ToLower()).Distinct().ToList();
        }

        public async Task<IEnumerable<DashboardValue>> GetDashboardValues()
        {
            List<DashboardValue> values = new List<DashboardValue>();

            var keys = new List<string>(_reservedTags);
            foreach (var key in _reservedTags)
            {
                values.Add(new DashboardValue() { Value = key, Type = "Key", Text = key.Replace("_", "").ToTitleCase() });
            }

            var contentTypes = Enum.GetValues(typeof(ContentType)).Cast<ContentType>()
                .Where(ct => ct != ContentType.NotSet && ct != ContentType.Exercise && ct != ContentType.Lab)
                .OrderBy(ct => ct.ToString())
                .ToList();

            contentTypes.Insert(0, ContentType.Lab);
            contentTypes.Insert(0, ContentType.Exercise);

            foreach (var contentType in contentTypes)
            {
                values.Add(new DashboardValue() { Value = contentType.ToString(), Type = "ContentType", Text = contentType.ToString().ToTitleCase() });
            }

            var tags = (await _contentRepository.DbContext.Tags.OrderByDescending(t => t.ContentTags.Count()).Take(10).ToListAsync()).Select(ct => ct.Name).Distinct();
            foreach (var tag in tags)
            {
                values.Add(new DashboardValue() { Value = tag, Type = "Tag", Text = tag.ToTitleCase() });
            }

            return values;
        }

        /// <summary>
        /// export content to file
        /// </summary>
        /// <param name="export"></param>
        /// <returns></returns>
        public async Task<ExportResult> Export(ExportSettings export)
        {
            if (!Identity.Permissions.Contains(SystemPermissions.PowerUser))
                throw new EntityPermissionException("Action requires elevated permissions.");

            IExport strategy = null;

            if (export.Type == ExportType.Csv)
            {
                strategy = new ContentExportCsv(_contentRepository.DbContext, Options.Content, Mapper);
            }

            if (export.Type == ExportType.Zip)
            {
                strategy = new ContentExportZip(_contentRepository.DbContext, Options.Content, Mapper);
            }

            return await strategy.Export(export.Ids);
        }

        /// <summary>
        /// import content from file
        /// </summary>
        /// <param name="token"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ImportResult>> Import(string token, IFormFile file)
        {
            string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"').ToLower();

            //var groupService = new GroupService(new GroupRepository(this._contentRepository.DbContext), this.ValidationHandler, DomainEventDispatcher, this.Options, this.IdentityResolver, this.LoggerFactory, Mapper);
            var playlistService = new PlaylistService(
                    new PlaylistRepository(this._contentRepository.DbContext, new PlaylistPermissionMediator(this.IdentityResolver)),
                    new TagRepository(this._contentRepository.DbContext),
                    this.ValidationHandler, this.Options, this.IdentityResolver, this.LoggerFactory, DomainEventDispatcher, Mapper);

            IImport import = null;

            if (fileName.EndsWith("csv"))
            {
                import = new ContentImportCsv(Options.Content, this, playlistService);
            }

            if (fileName.EndsWith("zip"))
            {
                import = new ContentImportZip(token, Options.Content, this, playlistService);
            }

            if (import == null)
                throw new EntityImportException("Invalid file type.");

            return await import.Import(await import.Convert(file));
        }

        /// <summary>
        /// disable content by ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<bool> Disable(int[] ids)
        {
            if (!Identity.Permissions.Contains(SystemPermissions.PowerUser))
                throw new EntityPermissionException("Action requires elevated permissions.");

            if (!ids.Any())
                throw new InvalidModelException("No content ids specified.");

            var contents = _contentRepository.GetAll().Where(c => ids.Contains(c.Id));

            await contents.ForEachAsync(c => c.IsDisabled = true);
            await _contentRepository.DbContext.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// enable content by ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<bool> Enable(int[] ids)
        {
            if (!Identity.Permissions.Contains(SystemPermissions.PowerUser))
                throw new EntityPermissionException("Action requires elevated permissions.");

            if (!ids.Any())
                throw new InvalidModelException("No content ids specified.");

            var contents = _contentRepository.GetAll().Where(c => ids.Contains(c.Id));

            await contents.ForEachAsync(c => c.IsDisabled = false);
            await _contentRepository.DbContext.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// reassign content sponsor
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="contentIds"></param>
        /// <returns></returns>
        public async Task<bool> UpdateSponsor( string groupId, int[] contentIds)
        {
            if (!Identity.Permissions.Contains(SystemPermissions.PowerUser))
                throw new EntityPermissionException("Action requires elevated permissions.");

            if (!contentIds.Any())
                throw new InvalidModelException("No content ids specified.");

            var contents = _contentRepository.GetAll().Where(c => contentIds.Contains(c.Id));

            // if (!_contentRepository.DbContext.Groups.Any(g => g.Id == groupId))
            //     throw new EntityNotFoundException("Group was not found.");

            await contents.ForEachAsync(c => c.PublisherId = groupId);
            await _contentRepository.DbContext.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// reassign content author
        /// </summary>
        /// <param name="authorId"></param>
        /// <param name="contentIds"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAuthor(int authorId, int[] contentIds)
        {
            if (!Identity.Permissions.Contains(SystemPermissions.PowerUser))
                throw new EntityPermissionException("Action requires elevated permissions.");

            if (!contentIds.Any())
                throw new InvalidModelException("No content ids specified.");

            var contents = _contentRepository.GetAll().Where(c => contentIds.Contains(c.Id));

            var id = Identity.GetId();
            if (!_contentRepository.DbContext.Profiles.Any(g => g.Id == id))
                throw new EntityNotFoundException("Profile was not found.");

            await contents.ForEachAsync(c => c.AuthorId = authorId);
            await _contentRepository.DbContext.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// TODO: refactor and rely on filter
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<PagedResult<Content, ContentSummary>> GetAllByTag(string tag, ContentDataFilter search = null)
        {
            search = search ?? new ContentDataFilter();
            var result = new PagedResult<Content, ContentSummary>();

            var t = tag.ToLower();

            if (t.StartsWith("_"))
            {
                t = t.Replace("_", "");
            }

            switch (t)
            {
                case "new":
                    search.Sort = "recent";
                    result = await GetAll(search);
                    break;
                case "recommended":
                    search.Filter = "recommended";
                    result = await GetAll(search);
                    break;
                case "popular":
                    result = await GetAllByRating(Rating.Good, 5, search);
                    break;
                default:
                    search.Filter = "tag=" + tag;
                    result = await GetAll(search);
                    break;
            }

            return result;
        }

        /// <summary>
        /// TODO: refactor and rely on filter
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dataFilter"></param>
        /// <returns></returns>
        public async Task<PagedResult<Content, ContentSummary>> GetAllByPlaylistId(int id, ContentDataFilter dataFilter)
        {
            IQueryable<Content> query = _contentRepository.GetAllByPlaylistId(id);
            return await PagedResult<Content, ContentSummary>(query, dataFilter);
        }

        public async Task AddBookmark(int id)
        {
            var _db = _contentRepository.DbContext;
            var profileContent = await _contentRepository.GetProfileContent(id, Identity.GetId());

            if (profileContent == null)
            {
                profileContent = new ProfileContent
                {
                    ContentId = id,
                    ProfileId = Identity.GetId()
                };

                _contentRepository.DbContext.ProfileContents.Add(profileContent);
            }

            profileContent.Bookmarked = DateTime.UtcNow;

            await _contentRepository.DbContext.SaveChangesAsync();
        }

        public async Task RemoveBookmark(int id)
        {
            var profileContent = await _contentRepository.GetProfileContent(id, Identity.GetId());

            if (profileContent == null)
            {
                return;
            }

            profileContent.Bookmarked = null;

            await _contentRepository.DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// update content
        /// </summary>
        /// <param name="model"></param>
        /// <param name="imported"></param>
        /// <returns></returns>
        public async Task<ContentDetail> Update(ContentUpdate model, bool imported = false)
        {
            await ValidationHandler.ValidateRulesFor(model);

            var entity = await _contentRepository.GetById(model.Id);

            if (entity == null)
                throw new EntityNotFoundException("Content was not found.");

            if (!_contentPermissionMediator.CanUpdate(entity))
                throw new EntityPermissionException("User does not have access to update this content.");

            var tagService = new TagService(_tagRepository, Options, IdentityResolver, LoggerFactory, Mapper);

            var originalName = entity.Name;

            entity.Name = model.Name;
            entity.Summary = model.Summary;
            entity.Description = model.Description;
            entity.Copyright = model.Copyright;
            entity.Type = model.Type;
            entity.PublisherId = model.PublisherId;
            entity.PublisherName = model.PublisherName;
            entity.PublisherSlug = model.PublisherSlug;
            entity.IsDisabled = model.IsDisabled;
            entity.Order = model.Order;
            entity.Url = model.Url;
            entity.LogoUrl = model.LogoUrl;
            entity.HoverUrl = model.HoverUrl;
            entity.ThumbnailUrl = model.ThumbnailUrl;
            entity.Settings = model.Settings;
            entity.Tags = tagService.ConvertTagsToDelmitedString(model.Tags);
            entity.Start = entity.Start.Set(model.StartDate, model.StartTime);
            entity.TrailerUrl = model.TrailerUrl;

            if (!string.IsNullOrWhiteSpace(model.AuthorId))
            {
                entity.AuthorId = await GetAuthor(model.AuthorId);
            }

            if (Identity.Permissions.Contains(SystemPermissions.PowerUser))
            {
                entity.IsRecommended = model.IsRecommended;
                entity.IsFeatured = model.IsFeatured;
                entity.FeaturedOrder = model.FeaturedOrder;
            }

            if (entity.Start.HasValue)
            {
                entity.End = entity.End.Set(model.EndDate, model.EndTime);
            }
            else
            {
                entity.End = null;
            }

            if (entity.AuthorId == 0) entity.AuthorId = Identity.GetId();

            var discussionService = new DiscussionService(_discussionRepository, Options, IdentityResolver, LoggerFactory, Mapper);

            discussionService.AddDiscussionToContent(entity);

            if (imported)
            {
                entity.Imported = DateTime.UtcNow;
                entity.ImportedBy = Identity.Id;
            }

            await _contentRepository.Update(entity);

            await tagService.SetContentTags(entity.Id, model.Tags);

            Dispatch(new DomainEvent(entity.GlobalId, originalName, Identity.Id, DomainEventType.ContentUpdate));

            return await GetById(entity.Id);
        }

        /// <summary>
        /// conditionally update properties based on value not being null
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ContentSummary> Patch(int id, ContentPatch model)
        {
            if (model == null)
                throw new InvalidModelException("The model is null.");

            var original = await GetById(id);

            if (original == null)
                throw new EntityNotFoundException("Content was not found.");

            var update = new ContentUpdate()
            {
                Id = id,
                Description = original.Description,
                IsDisabled = original.IsDisabled,
                LogoUrl = original.LogoUrl,
                HoverUrl = original.HoverUrl,
                ThumbnailUrl = original.ThumbnailUrl,
                Name = original.Name,
                Order = original.Order,
                PublisherId = original.PublisherId,
                PublisherName = original.PublisherName,
                PublisherSlug = original.PublisherSlug,
                Type = original.Type,
                Tags = original.Tags.Select(t => t.Name).ToArray(),
                Url = original.Url,
                TrailerUrl = original.TrailerUrl
            };

            if (model.Description != null) update.Description = model.Description;
            if (model.IsDisabled.HasValue) update.IsDisabled = model.IsDisabled.Value;
            if (model.Name != null) update.Name = model.Name;
            if (model.Order.HasValue) update.Order = model.Order.Value;
            if (model.Settings != null) update.Settings = model.Settings;
            if (model.PublisherId != null) update.PublisherId = model.PublisherId;
            if (model.PublisherName != null) update.PublisherName = model.PublisherName;
            if (model.PublisherSlug != null) update.PublisherSlug = model.PublisherSlug;
            if (model.LogoUrl != null) update.LogoUrl = model.LogoUrl;
            if (model.HoverUrl != null) update.HoverUrl = model.HoverUrl;
            if (model.ThumbnailUrl != null) update.ThumbnailUrl = model.ThumbnailUrl;
            if (model.TrailerUrl != null) update.TrailerUrl = model.TrailerUrl;
            if (model.Type.HasValue) update.Type = model.Type.Value;
            if (model.Url != null) update.Url = model.Url;
            if (model.Tags != null) update.Tags = model.Tags;

            if (Identity.Permissions.Contains(SystemPermissions.PowerUser))
            {
                if (model.IsRecommended.HasValue) update.IsRecommended = model.IsRecommended.Value;
                if (model.IsFeatured.HasValue) update.IsFeatured = model.IsFeatured.Value;
                if (model.FeaturedOrder.HasValue) update.FeaturedOrder = model.FeaturedOrder.Value;
            }

            var saved = await Update(update);

            Dispatch(new DomainEvent(original.GlobalId, original.Name, DomainEventType.ContentUpdate));

            return Map<ContentSummary>(await _contentRepository.GetById(saved.Id));
        }

        /// <summary>
        /// create a collection of content
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<int> SaveRangeAsync(int channelId, IEnumerable<ContentCreate> list)
        {
            int contentAdded = 0;

            foreach (var content in list)
            {
                ContentDetail contentDetail = await Add(content);

                if (contentDetail != null)
                {
                    contentAdded++;
                }
            }

            return contentAdded;
        }

        /// <summary>
        /// add flag to content
        /// </summary>
        /// <param name="id"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public async Task AddFlag(int id, string comment)
        {
            var content = await _contentRepository.GetById(id);

            if (content == null)
                throw new EntityNotFoundException("Content '" + id + " was not found.");

            var profileContent = await _contentRepository.GetProfileContent(id, Identity.GetId());

            if (profileContent == null)
            {
                profileContent = new ProfileContent
                {
                    ContentId = id,
                    ProfileId = Identity.GetId()
                };

                _contentRepository.DbContext.ProfileContents.Add(profileContent);
            }

            profileContent.Flagged = DateTime.UtcNow;
            profileContent.FlaggedComment = comment;

            await _contentRepository.DbContext.SaveChangesAsync();

            await CalculateFlagCount(id);

            Dispatch(new DomainEvent(content.GlobalId, content.Name, Identity.Id, DomainEventType.ContentFlagged));
        }

        /// <summary>
        /// user removed their own flag
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task RemoveFlag(int id)
        {
            var content = await _contentRepository.GetById(id);

            if (content == null)
                throw new EntityNotFoundException("Content '" + id + " was not found.");

            var profileContent = await _contentRepository.GetProfileContent(id, Identity.GetId());

            if (profileContent == null)
            {
                return;
            }

            profileContent.Flagged = null;
            profileContent.FlaggedComment = string.Empty;

            await _contentRepository.DbContext.SaveChangesAsync();

            await CalculateFlagCount(id);
        }

        /// <summary>
        /// accept flag
        /// </summary>
        /// <param name="id"></param>
        /// <param name="profileContentId"></param>
        /// <returns></returns>
        public async Task AcceptFlag(int id, int profileId)
        {
            await SetFlagStatus(id, profileId, FlagStatusType.Accepted);
        }

        /// <summary>
        /// reject flag
        /// </summary>
        /// <param name="id"></param>
        /// <param name="profileContentId"></param>
        /// <returns></returns>
        public async Task RejectFlag(int id, int profileId)
        {
            await SetFlagStatus(id, profileId, FlagStatusType.Rejected);
        }

        /// <summary>
        /// set flag status
        /// </summary>
        /// <param name="id"></param>
        /// <param name="profileContentId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task SetFlagStatus(int id, int profileId, FlagStatusType status)
        {
            if (!Identity.Permissions.Contains(SystemPermissions.PowerUser))
                throw new EntityPermissionException("Action requires elevated permissions.");

            var content = await _contentRepository.GetById(id);

            if (content == null)
                throw new EntityNotFoundException("Content '" + id + " was not found.");

            var profileContent = await _contentRepository.GetProfileContent(id, profileId);

            if (profileContent == null)
                throw new EntityNotFoundException("No flag found for this content.");

            profileContent.FlagStatus = status;

            if (status == FlagStatusType.Accepted)
            {
                content.IsDisabled = true;
            }

            await _contentRepository.DbContext.SaveChangesAsync();

            await CalculateFlagCount(id);
        }

        /// <summary>
        /// calculate flag count
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        async Task CalculateFlagCount(int id)
        {
            var content = await _contentRepository.GetById(id);

            content.FlagCount = await _contentRepository.DbContext
                .ProfileContents.CountAsync(pc => pc.ContentId == id && pc.Flagged.HasValue && pc.FlagStatus == FlagStatusType.Pending);

            await _contentRepository.DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves a content item that matches the speciifed search criteria. Only unrestricted content will be returned.
        /// </summary>
        /// <param name="dataFilter">The search parameters specified by the caller</param>
        /// <returns>A content search result that matches the speciifed search criteria</returns>
        public async Task<PagedResult<Content, ContentSummary>> GetAll(ContentDataFilter dataFilter)
        {
            var query = _contentRepository.GetAll();
            return await PagedResult<Content, ContentSummary>(query, dataFilter);
        }

        /// <summary>
        /// get all content by rating
        /// [TODO] refactor the data filter to handle 'min=x,max=y' and remove
        /// </summary>
        /// <param name="minimumRatingAverage"></param>
        /// <param name="minimumRatingTotal"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<PagedResult<Content, ContentSummary>> GetAllByRating(Rating minimumRatingAverage, int minimumRatingTotal, ContentDataFilter search)
        {
            var rating = (double)minimumRatingAverage;

            var query = _contentRepository.GetAll()
                .Where(c => c.RatingAverage >= rating && c.RatingTotal >= minimumRatingTotal);

            search.Sort = "-rating";

            return await PagedResult<Content, ContentSummary>(query, search);
        }

        //public async Task<PagedResult<Content, ContentSummary>> GetAllByChannelId(int id, ContentDataFilter search)
        //{
        //    IQueryable<Content> query = _contentRepository.GetAllByChannelId(id);

        //    search.Sort = "index";

        //    return await PagedResult<Content, ContentSummary>(query, search);
        //}

        /// <summary>
        /// Retrieves a list of content summary items that match the specified search criteria. Only summary items of accessible content will be returned.
        /// </summary>
        /// <param name="search">The search parameters specified by the caller</param>
        /// <returns>Returns a list of content summary items that match the search criteria</returns>
        public async Task<PagedResult<Content, ContentSummary>> GetAllAccessibleByProfileId(ContentDataFilter search)
        {
            IQueryable<Content> query = _contentRepository.GetAll();

            return await PagedResult<Content, ContentSummary>(query, search);
        }
                /// <summary>
        /// get all content by profile id
        /// </summary>
        /// <param name="profileId"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<PagedResult<Content, ContentSummary>> GetAllContentByProfileId(int profileId, ContentDataFilter search)
        {
            IQueryable<Content> query = _contentRepository.GetAllByProfileId(profileId);
            return await PagedResult<Content, ContentSummary>(query, search);
        }

        /// <summary>
        /// Retrieves a content detail item that matches the global id of the content. Specifying the global id of a restricted content item will result in an exception.
        /// </summary>
        /// <param name="globalId">The global id of the content to retrieve</param>
        /// <returns>A content detail item</returns>
        public async Task<ContentDetail> GetByGlobalId(string globalId)
        {
            var content = await _contentRepository.DbContext.Contents.SingleOrDefaultAsync(c => c.GlobalId.ToLower() == globalId.ToLower());

            if (content == null)
                throw new EntityNotFoundException("Content '" + globalId + "' was not found.");

            return await GetById(content.Id);
        }

        /// <summary>
        /// Retrieves a content detail item that matches the id of the content. Specifying the id of a restricted content item will result in an exception.
        /// </summary>
        /// <param name="id">The id of the content to retrieve</param>
        /// <returns>A content detail item</returns>
        public async Task<ContentDetail> GetById(int id)
        {
            if (!(await _contentRepository.Exists(id)))
                throw new EntityNotFoundException("Content '" + id + "' was not found.");

            var content = await _contentRepository.GetById(id);

            if (content == null)
            {
                throw new EntityPermissionException("Getting content requires elevated permissions");
            }

            if (content.IsDisabled && !_contentPermissionMediator.CanUpdate(content))
                throw new EntityNotFoundException("Content '" + id + "' was not found.");

            return Map<ContentDetail>(content);
        }

        /// <summary>
        /// Deletes a piece of content from a channel
        /// </summary>
        /// <param name="id">The id of the content to delete</param>
        /// <returns>True if the content is successfully deleted and false otherwise</returns>
        public async Task DeleteById(int id)
        {
            var content = await _contentRepository.GetById(id);

            if (content == null)
                throw new EntityNotFoundException("Content '" + id + "' was not found");

            await Delete(content);
            return;
        }

        /// <summary>
        /// Deletes a piece of content from a channel
        /// </summary>
        /// <param name="globalId">The global id of the content to delete</param>
        /// <returns>True if the content is successfully deleted and false otherwise</returns>
        public async Task DeleteByGlobalId(string globalId)
        {
            var content = await _contentRepository.GetByGlobalId(globalId);

            if (content == null)
                throw new EntityNotFoundException("Content '" + globalId + "' was not found");

            await _contentRepository.Delete(content);
        }

        async Task Delete(Content content)
        {
            if (!_contentPermissionMediator.CanDelete(content))
            {
                throw new EntityPermissionException("Content '" + content.Name + "' cannot be deleted. Permission denied.");
            }

            var globalId = content.GlobalId;
            var name = content.Name;

            await _contentRepository.DeleteById(content.Id);

            Dispatch(new DomainEvent(globalId, name, Identity.Id, DomainEventType.ContentDelete));
        }

        /// <summary>
        /// Deletes multiple pieces of content from a channel
        /// </summary>
        /// <param name="globalIds">A list of global ids of content to delete</param>
        /// <returns>True if the content is successfully deleted and false otherwise</returns>
        public async Task DeleteByGlobalIds(List<string> globalIds)
        {
            List<Content> contentItems = _contentRepository.DbContext.Contents.Where(c => globalIds.Contains(c.GlobalId)).ToList();

            // loop through range to hit business rules
            foreach (var content in contentItems)
            {
                await DeleteById(content.Id);
            }
        }

        /// <summary>
        /// Launches a piece of content and adds the ability for a content provider to award achievements for the specified piece of content
        /// </summary>
        /// <param name="contentGlobalId"></param>
        /// <param name="profileGlobalId"></param>
        /// <returns>A ProfileContent object containing details necessary for awarding achievements</returns>
        public async Task<ProfileContentSummary> Engage(string contentGlobalId, string profileGlobalId)
        {
            var content = await _contentRepository.GetByGlobalId(contentGlobalId);
            var profile = await _contentRepository.DbContext.Profiles.SingleOrDefaultAsync(p => p.GlobalId.ToLower() == profileGlobalId.ToLower());

            if (content == null)
                throw new EntityNotFoundException("Content '" + contentGlobalId + "' was not found.");

            var profileContent = await _contentRepository.GetProfileContent(content.Id, profile.Id);

            if (profileContent == null)
            {
                profileContent = new ProfileContent
                {
                    ContentId = content.Id,
                    ProfileId = Identity.GetId()
                };

                _contentRepository.DbContext.ProfileContents.Add(profileContent);
                await _contentRepository.DbContext.SaveChangesAsync();
            }

            return Map<ProfileContentSummary>(profileContent);
        }

        /// <summary>
        /// Allows a user to rate a piece of content
        /// </summary>
        /// <param name="contentId">The id of the content to be rated</param>
        /// <param name="rating">The available content rating values</param>
        /// <returns>A ContentDetail</returns>
        public async Task<ContentDetail> AddOrUpdateRating(int contentId, Rating rating)
        {
            var _db = _contentRepository.DbContext;
            var profileContent = await _contentRepository.GetProfileContent(contentId, Identity.GetId());

            if (profileContent == null)
            {
                profileContent = new ProfileContent
                {
                    ContentId = contentId,
                    ProfileId = Identity.GetId(),
                    Rating = rating
                };

                _db.ProfileContents.Add(profileContent);
            }
            else
            {
                if (profileContent.ProfileId != Identity.GetId())
                    throw new EntityPermissionException("Rating content requires elevated permissions.");

                profileContent.Rating = rating == profileContent.Rating ? Rating.Unrated : rating;

                _db.ProfileContents.Update(profileContent);
            }

            await _db.SaveChangesAsync();

            if (profileContent.Content == null)
            {
                profileContent = await _contentRepository.GetProfileContent(contentId, Identity.GetId());
            }

            Dispatch(new DomainEvent(profileContent.Content.GlobalId, profileContent.Content.Name, DomainEventType.ContentRate));

            return await GetById(contentId);
        }

        /// <summary>
        /// Allows a user to provide a difficulty rating for a piece of content
        /// </summary>
        /// <param name="contentId">The id of the content to be rated</param>
        /// <param name="level">he available content difficulty rating values</param>
        /// <returns>A ContentDetail</returns>
        public async Task<ContentDetail> AddOrUpdateDifficulty(int contentId, Difficulty level)
        {
            var profileContent = await _contentRepository.GetProfileContent(contentId, Identity.GetId());
            var db = _contentRepository.DbContext;

            if (profileContent == null)
            {
                profileContent = new ProfileContent
                {
                    ContentId = contentId,
                    ProfileId = Identity.GetId(),
                    Difficulty = level
                };

                db.ProfileContents.Add(profileContent);
            }
            else
            {
                if (profileContent.ProfileId != Identity.GetId())
                    throw new EntityPermissionException("Setting difficulty requires elevated permissions.");

                profileContent.Difficulty = level == profileContent.Difficulty ? Difficulty.Unrated : level;

                db.ProfileContents.Update(profileContent);
            }

            await db.SaveChangesAsync();

            if (profileContent.Content == null)
            {
                profileContent = await _contentRepository.GetProfileContent(contentId, Identity.GetId());
            }

            Dispatch(new DomainEvent(profileContent.Content.GlobalId, profileContent.Content.Name, DomainEventType.ContentLevel));

            return await GetById(contentId);
        }
    }
}

