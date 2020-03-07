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
    public class PlaylistService : DispatchService<Playlist>
    {
        public IPlaylistRepository _playlistRepository;
        public ITagRepository _tagRepository;
        public IValidationHandler _validationHandler;

        public PlaylistService(IPlaylistRepository playlistRepository, ITagRepository tagRepository, IValidationHandler validationHandler, CoreOptions options, IStackIdentityResolver userResolver, ILoggerFactory loggerFactory, IDomainEventDispatcher domainEventDispatcher, IMapper mapper)
            : base(domainEventDispatcher, options, userResolver, loggerFactory, mapper)
        {
            _playlistRepository = playlistRepository ?? throw new ArgumentNullException("playlistRepository");
            _validationHandler = validationHandler ?? throw new ArgumentNullException("validationHandler");
            _tagRepository = tagRepository ?? throw new ArgumentNullException("tagRepository");
        }

        /// <summary>
        /// get playlist by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PlaylistDetail> GetById(int id)
        {
            var playlist = await _playlistRepository.GetById(id);

            var model = Map<PlaylistDetail>(playlist);

            return model;
        }

        /// <summary>
        /// get all playlists created by profile. if not current identity then only return public playlists
        /// </summary>
        /// <param name="profileId"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<PagedResult<Playlist, PlaylistSummary>> GetAllByProfileId(int profileId, PlaylistDataFilter search)
        {
            var query = _playlistRepository.GetAllByProfileId(profileId);

            return await PagedResult<Playlist, PlaylistSummary>(query, search);
        }

        public async Task<PagedResult<Content, ContentSummary>> GetPlaylistContents(int playlistId, ContentDataFilter filter)
        {
            var query = _playlistRepository.DbContext.Contents
                // .Include(c => c.Publisher)
                .Include(c => c.Author)
                .Include(c => c.ContentTags)
                .Where(c => c.SectionContents.Any(sc => sc.Section.PlaylistId == playlistId));

            if (!Identity.Permissions.Contains(SystemPermissions.PowerUser))
            {
                query = query.Where(p => !p.IsDisabled);
            }

            return await PagedResult<Content, ContentSummary>(query, filter);
        }

        public async Task<PagedResult<Playlist, PlaylistSummary>> GetAllGroupPlaylists(string groupId, PlaylistDataFilter search)
        {
            var query = _playlistRepository.GetAllByGroupId(groupId);
            return await PagedResult<Playlist, PlaylistSummary>(query, search);
        }

        /// <summary>
        /// get all playlists by rating
        /// </summary>
        /// <param name="minimumRatingAverage"></param>
        /// <param name="minimumRatingTotal"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<PagedResult<Playlist, PlaylistSummary>> GetAllByRating(Rating minimumRatingAverage, int minimumRatingTotal, PlaylistDataFilter search)
        {
            var rating = (double)minimumRatingAverage;

            var query = _playlistRepository.GetAll()
                .Where(p => p.RatingAverage >= rating && p.RatingTotal >= minimumRatingTotal);

            if (string.IsNullOrWhiteSpace(search.Sort))
            {
                search.Sort = "-rating";
            }

            return await PagedResult<Playlist, PlaylistSummary>(query, search);
        }

        public async Task<PlaylistDetail> Add(PlaylistCreate model, bool imported = false)
        {
            await _validationHandler.ValidateRulesFor(model);

            var playlist = new Playlist
            {
                GlobalId = model.GlobalId,
                ProfileId = Identity.GetId(),
                Name = model.Name,
                Description = model.Description,
                Summary = model.Summary,
                LogoUrl = model.LogoUrl,
                TrailerUrl = model.TrailerUrl,
                IsPublic = model.IsPublic,
                IsDefault = model.IsDefault,
                IsRecommended = model.IsRecommended,
                IsFeatured = model.IsFeatured,
                Copyright = model.Copyright,
                PublisherId = model.PublisherId,
                PublisherName = model.PublisherName,
                PublisherSlug = model.PublisherSlug
            };

            playlist.ProfileFollowers.Add(new ProfileFollower() { ProfileId = Identity.GetId() });

            if (imported)
            {
                playlist.Imported = DateTime.UtcNow;
                playlist.ImportedBy = Identity.Id;
            }

            var tagService = new TagService(_tagRepository, Options, IdentityResolver, LoggerFactory, Mapper);

            playlist.Tags = tagService.ConvertTagsToDelmitedString(model.Tags);

            var result = await _playlistRepository.Add(playlist);

            await tagService.SetPlaylistTags(playlist.Id, model.Tags);

            Dispatch(new DomainEvent(result.GlobalId, result.Name, DomainEventType.PlaylistAdd));

            return await GetById(result.Id);
        }

        public async Task<PlaylistDetail> Update(PlaylistUpdate model, bool imported = false)
        {
            await _validationHandler.ValidateRulesFor(model);

            var playlist = await _playlistRepository.GetById(model.Id);

            if (!playlist.CanEdit(Identity))
                throw new EntityPermissionException("Playlist edit requires elevated permissions.");

            playlist.Name = model.Name;
            playlist.Description = model.Description;
            playlist.Summary = model.Summary;
            playlist.LogoUrl = model.LogoUrl;
            playlist.TrailerUrl = model.TrailerUrl;
            playlist.IsPublic = model.IsPublic;
            playlist.IsDefault = model.IsDefault;
            playlist.IsFeatured = model.IsFeatured;
            playlist.IsRecommended = model.IsRecommended;
            playlist.FeaturedOrder = model.FeaturedOrder;
            playlist.Copyright = model.Copyright;
            playlist.PublisherId = model.PublisherId;
            playlist.PublisherName = model.PublisherName;
            playlist.PublisherSlug = model.PublisherSlug;

            if (imported)
            {
                playlist.Imported = DateTime.UtcNow;
                playlist.ImportedBy = Identity.Id;
            }

            var tagService = new TagService(_tagRepository, Options, IdentityResolver, LoggerFactory, Mapper);

            playlist.Tags = tagService.ConvertTagsToDelmitedString(model.Tags);

            var result = await _playlistRepository.Update(playlist);

            await tagService.SetPlaylistTags(playlist.Id, model.Tags);

            Dispatch(new DomainEvent(result.GlobalId, result.Name, DomainEventType.PlaylistUpdate));

            return await GetById(result.Id);
        }

        public async Task<PagedResult<Playlist, PlaylistSummary>> GetAll(PlaylistDataFilter search)
        {
            var query = _playlistRepository.GetAll();

            return await PagedResult<Playlist, PlaylistSummary>(query, search);
        }

        public async Task<PlaylistSummary> Unfollow(int id)
        {
            var playlist = await _playlistRepository.GetById(id);

            if (playlist == null)
                throw new EntityNotFoundException("Playlist was not found.");

            var followers = playlist.ProfileFollowers.Where(f => f.ProfileId == Identity.GetId());

            if (!followers.Any())
                throw new EntityPermissionException("User does not follow this playlist.");

            _playlistRepository.DbContext.ProfileFollowers.RemoveRange(followers);

            await _playlistRepository.DbContext.SaveChangesAsync();

            Dispatch(new DomainEvent(playlist.GlobalId, playlist.Name, DomainEventType.PlaylistUnfollow));

            return Map<PlaylistSummary>(await _playlistRepository.GetById(playlist.Id));
        }

        public async Task<PlaylistSummary> Unfollow(int id, string groupId)
        {
            var playlist = await _playlistRepository.GetById(id);

            if (playlist == null)
                throw new EntityNotFoundException("Playlist was not found.");

            var followers = playlist.PlaylistGroups.Where(f => f.GroupId == groupId);

            if (!followers.Any())
                throw new EntityPermissionException("Group does not follow this playlist.");

            _playlistRepository.DbContext.PlaylistGroups.RemoveRange(followers);

            await _playlistRepository.DbContext.SaveChangesAsync();

            Dispatch(new DomainEvent(playlist.GlobalId, playlist.Name, DomainEventType.PlaylistGroupUnfollow));

            return Map<PlaylistSummary>(await _playlistRepository.GetById(playlist.Id));
        }

        public async Task<PlaylistSummary> Follow(int id)
        {
            var playlist = await _playlistRepository.GetById(id);

            if (playlist == null)
                throw new EntityNotFoundException("Playlist was not found.");

            if (playlist.ProfileId == Identity.GetId())
                throw new InvalidIdentityException("You cannot follow your own playlist.");

            if (!playlist.IsPublic)
                throw new EntityPermissionException("Playlist is not public and cannot be followed.");

            var followers = playlist.ProfileFollowers.Where(f => f.ProfileId == Identity.GetId());

            if (followers.Any())
                throw new EntityPermissionException("User already follows this Playlist.");

            _playlistRepository.DbContext.ProfileFollowers.Add(new ProfileFollower() { PlaylistId = playlist.Id, ProfileId = Identity.GetId() });

            await _playlistRepository.DbContext.SaveChangesAsync();

            Dispatch(new DomainEvent(playlist.GlobalId, playlist.Name, DomainEventType.PlaylistFollow));

            return Map<PlaylistSummary>(await _playlistRepository.GetById(playlist.Id));
        }

        public async Task<PlaylistSummary> Follow(int id, string groupId)
        {
            var playlist = await _playlistRepository.GetById(id);

            if (playlist == null)
                throw new EntityNotFoundException("Playlist was not found.");

            if (!playlist.IsPublic)
                throw new EntityPermissionException("Playlist is not public and cannot be followed.");

            var playlistGroup = playlist.PlaylistGroups.Where(f => f.GroupId == groupId);

            if (playlistGroup.Any())
                throw new EntityPermissionException("Group already follows this Playlist.");

            _playlistRepository.DbContext.PlaylistGroups.Add(new PlaylistGroup { PlaylistId = playlist.Id, GroupId = groupId });

            await _playlistRepository.DbContext.SaveChangesAsync();
            Dispatch(new DomainEvent(playlist.GlobalId, playlist.Name, DomainEventType.PlaylistGroupFollow));

            return Map<PlaylistSummary>(await _playlistRepository.GetById(playlist.Id));
        }

        public async Task<bool> Delete(int id)
        {
            var playlist = await _playlistRepository.DbContext.Playlists.SingleOrDefaultAsync(p => p.Id == id);

            if (playlist == null)
                throw new EntityNotFoundException("Playlist was not found.");

            if (!playlist.CanEdit(Identity))
                throw new EntityPermissionException("Playlist does not belong to current Profile.");

            _playlistRepository.DbContext.Playlists.Remove(playlist);
            await _playlistRepository.DbContext.SaveChangesAsync();

            Dispatch(new DomainEvent(playlist.GlobalId, playlist.Name, DomainEventType.PlaylistDelete));

            return true;
        }

        public async Task<bool> SetAsDefault(int playlistId)
        {
            if (playlistId <= 0)
                throw new InvalidIdentityException("Playlist ID is invalid.");

            var playlists = await _playlistRepository.GetAllByProfileId(Identity.GetId()).ToListAsync();

            if (!playlists.Any(p => p.Id == playlistId))
                throw new EntityNotFoundException("Playlist was not found.");

            foreach (var playlist in playlists)
            {
                if (!playlist.CanEdit(Identity))
                    throw new EntityPermissionException("User does not have access to manage this playlist.");

                playlist.IsDefault = (playlist.Id == playlistId);
            }

            await _playlistRepository.DbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AddContentToDefaultPlaylist(int contentId)
        {
            var playlist = await _playlistRepository.GetDefaultPlaylist(Identity.GetId());
            return await AddContent(playlist.Id, contentId);
        }

        public async Task<bool> AddContent(int playlistId, int contentId)
        {
            var playlist = await _playlistRepository.GetAll().SingleOrDefaultAsync(p => p.Id == playlistId);

            if (playlist == null)
                throw new EntityNotFoundException("Playlist was not found.");

            if (!playlist.CanEdit(Identity))
                throw new EntityPermissionException("User does not have access to manage this playlist.");

            if (_playlistRepository.DbContext.Contents.Any(c => c.SectionContents.Any(pc => pc.ContentId == contentId && pc.Section.PlaylistId == playlistId)))
                throw new EntityPermissionException("Content has already been added to playlist.");

            var section = await GetDefaultSection(playlistId, true);

            section.SectionContents.Add(new SectionContent
            {
                SectionId = section.Id,
                ContentId = contentId
            });

            await _playlistRepository.DbContext.SaveChangesAsync();

            var content = await _playlistRepository.DbContext.Contents.SingleOrDefaultAsync(c => c.Id == contentId);

            Dispatch(new DomainEvent(content.GlobalId, content.Name, playlist.GlobalId, DomainEventType.PlaylistContentAdd));

            return true;
        }

        public async Task<Section> GetDefaultSection(int playlistId, bool create = false)
        {
            var section = await _playlistRepository.DbContext.Sections.Where(s => s.PlaylistId == playlistId).OrderBy(s => s.Order).FirstOrDefaultAsync();

            if (section == null && create)
            {
                var playlist = _playlistRepository.DbContext.Playlists.Single(p => p.Id == playlistId);

                section = new Section
                {
                    Name = "",
                    Order = 0,
                    PlaylistId = playlistId
                };

                playlist.Sections.Add(section);

                await _playlistRepository.DbContext.SaveChangesAsync();
            }

            return section;
        }

        public async Task<bool> RemoveContent(int playlistId, int contentId)
        {
            var id = Identity.GetId();
            var playlist = await _playlistRepository.GetAllByProfileId(id).SingleOrDefaultAsync(p => p.Id == playlistId);

            if (playlist == null)
                throw new EntityNotFoundException("Playlist was not found.");

            if (!playlist.CanEdit(Identity))
                throw new EntityPermissionException("User does not have access to manage this playlist.");

            var sectionContent = await _playlistRepository.DbContext.SectionContents
                .Include(pc => pc.Content)
                .FirstOrDefaultAsync(pc => pc.ContentId == contentId && pc.Section.PlaylistId == playlistId);

            if (sectionContent == null)
                throw new EntityPermissionException("Content is not available.");

            var content = sectionContent.Content;

            _playlistRepository.DbContext.SectionContents.Remove(sectionContent);

            Dispatch(new DomainEvent(content.GlobalId, content.Name, playlist.GlobalId, DomainEventType.PlaylistContentDelete));

            await _playlistRepository.DbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Organize(int playlistId, List<PlaylistSectionUpdate> sections)
        {
            var playlist = await _playlistRepository.DbContext.Playlists
                .Include(p => p.Sections)
                .Include("Sections.SectionContents")
                .SingleOrDefaultAsync(p => p.Id == playlistId);

            if (playlist == null)
                throw new EntityNotFoundException("Playlist was not found.");

            if (!playlist.CanEdit(Identity))
                throw new EntityPermissionException("Playlist does not belong to current Profile.");

            var duplicates = sections.GroupBy(s => s.Name.Trim()).Where(s => s.Count() > 1);

            if (duplicates.Any())
            {
                var invalid = string.Join(", ", duplicates.Select(d => d.Key));
                throw new EntityDuplicateException("Section name '" + invalid + "' must be unique.");
            }

            playlist.Sections.Clear();

            int sectionOrder = 0;
            int contentOrder = 0;
            foreach (var s in sections)
            {
                var section = new Section()
                {
                    Name = s.Name.Trim(),
                    Order = sectionOrder
                };

                foreach (var c in s.ContentIds)
                {
                    var sectionContent = new SectionContent()
                    {
                        ContentId = c,
                        Order = contentOrder
                    };

                    section.SectionContents.Add(sectionContent);

                    contentOrder++;
                }

                sectionOrder++;
                contentOrder = 0;

                playlist.Sections.Add(section);
            }

            await _playlistRepository.DbContext.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Allows a user to rate a playlist
        /// </summary>
        /// <param name="playlistId">The id of the playlist to be rated</param>
        /// <param name="rating">The available playlist rating values</param>
        /// <returns>A PlaylistInstanceSummary</returns>
        public async Task<PlaylistDetail> SetRating(int playlistId, Rating rating)
        {
            var _db = _playlistRepository.DbContext;
            var playlist = await _playlistRepository.GetById(playlistId);

            if (playlist == null)
                throw new InvalidModelException("Playlist ID is invalid.");

            if (!playlist.IsPublic)
                throw new EntityPermissionException("Only public playlists can be rated.");

            var id = Identity.GetId();
            var profileFollower = await _playlistRepository.DbContext.ProfileFollowers.Where(f => f.PlaylistId == playlistId && f.ProfileId == id).SingleOrDefaultAsync();

            if (profileFollower == null)
                throw new EntityPermissionException("You must be following a playlist to rate it.");

            profileFollower.Rating = rating;
            _db.ProfileFollowers.Update(profileFollower);
            await _db.SaveChangesAsync();
            await UpdateRating(playlistId);

            Dispatch(new DomainEvent(playlist.GlobalId, playlist.Name, DomainEventType.PlaylistRate));

            return await GetById(playlistId);
        }

        async Task UpdateRating(int playlistId)
        {
            var query = _playlistRepository.DbContext.ProfileFollowers
                .Where(pc => pc.PlaylistId == playlistId && pc.Rating != Rating.Unrated)
                .OrderBy(pc => pc.Rating);

            int total = query.Count();
            double average = 0;
            Rating median = Rating.Unrated;

            if (total > 0)
            {
                average = query.Average(pp => (long?)pp.Rating) ?? 0;
                median = query.Skip(query.Count() / 2).FirstOrDefault().Rating;
            }

            var playlist = await _playlistRepository.DbContext.Playlists.SingleOrDefaultAsync(c => c.Id == playlistId);

            playlist.RatingAverage = average;
            playlist.RatingMedian = median;
            playlist.RatingTotal = total;

            await _playlistRepository.Update(playlist);

            Dispatch(new DomainEvent(playlist.GlobalId, playlist.Name, DomainEventType.PlaylistRate));
        }

        public static RatingMetricDetail CalculateRatingMetric(Playlist playlist)
        {
            return new RatingMetricDetail()
            {
                Average = playlist.RatingAverage,
                Median = playlist.RatingMedian,
                Poor = playlist.ProfileFollowers.Count(pf => pf.Rating == Rating.Poor),
                Fair = playlist.ProfileFollowers.Count(pf => pf.Rating == Rating.Fair),
                Good = playlist.ProfileFollowers.Count(pf => pf.Rating == Rating.Good),
                Great = playlist.ProfileFollowers.Count(pf => pf.Rating == Rating.Great),
                Total = playlist.RatingTotal
            };
        }

        public async Task<ExportResult> Export(ExportSettings export)
        {
            if (!Identity.Permissions.Contains(SystemPermissions.PowerUser))
                throw new EntityPermissionException("Action requires elevated permissions.");

            IExport strategy = null;

            if (export.Type == ExportType.Csv)
            {
                strategy = new PlaylistExportCsv(_playlistRepository.DbContext, Options.Content, Mapper);
            }

            if (export.Type == ExportType.Zip)
            {
                strategy = new PlaylistExportZip(_playlistRepository.DbContext, Options.Content, Mapper);
            }

            return await strategy.Export(export.Ids);
        }

        public async Task<IEnumerable<ImportResult>> Import(string token, IFormFile file)
        {
            string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"').ToLower();

            IImport import = null;

            //var groupService = new GroupService(new GroupRepository(this._playlistRepository.DbContext), this._validationHandler, DomainEventDispatcher, this.Options, this.IdentityResolver, this.LoggerFactory, Mapper);
            var contentService = new ContentService(
                new ContentRepository(this._playlistRepository.DbContext, new Security.ContentPermissionMediator(IdentityResolver)),
                new TagRepository(this._playlistRepository.DbContext),
                new DiscussionRepository(this._playlistRepository.DbContext),
                //new GroupRepository(this._playlistRepository.DbContext),
                new ProfileRepository(this._playlistRepository.DbContext, new Security.ProfilePermissionMediator(IdentityResolver)),
                this._validationHandler, DomainEventDispatcher, this.Options, this.IdentityResolver, this.LoggerFactory, Mapper);

            if (fileName.EndsWith("csv"))
            {
                import = new PlaylistImportCsv(Options.Content, contentService, this);
            }

            if (fileName.EndsWith("zip"))
            {
                import = new PlaylistImportZip(token, Options.Content, contentService, this);
            }

            if (import == null)
            {
                throw new EntityImportException("Invalid file type.");
            }

            return await import.Import(await import.Convert(file));
        }

        public async Task<PlaylistDetail> Import(string name, string description, string globalId, string logoUrl, string trailerUrl, string summary, string publisherGlobalId)
        {
            var playlist = await _playlistRepository.GetByGlobalId(globalId)
                ?? await _playlistRepository.GetByName(name);

            //Group group = null;
            //if (!string.IsNullOrWhiteSpace(publisherGlobalId))
            //{
            //    group = await _playlistRepository.DbContext.Groups.FirstOrDefaultAsync(g => g.GlobalId.ToLower() == publisherGlobalId.ToLower());
            //}

            if (playlist == null)
            {
                var add = new PlaylistCreate
                {
                    Description = description,
                    Name = name,
                    LogoUrl = logoUrl,
                    GlobalId = globalId,
                    Summary = summary,
                    TrailerUrl = trailerUrl,
                    PublisherId = publisherGlobalId
                    // PublisherId = group == null ? null : (int?)group.Id
                };

                return await Add(add, true);
            }

            var update = new PlaylistUpdate
            {
                Id = playlist.Id,
                Description = description,
                Name = name,
                LogoUrl = logoUrl,
                Summary = summary,
                TrailerUrl = trailerUrl,
                // PublisherId = group == null ? null : (int?)group.Id
            };

            return await Update(update, true);
        }
    }
}

