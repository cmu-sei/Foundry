/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Foundry.Portal.Services;
using Foundry.Portal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Portal.Import
{
    public abstract class ImportBase : IImport
    {
        public ContentService _contentService;
        public PlaylistService _playlistService;
        public ContentOptions _contentOptions;
        public ImportBase(ContentOptions contentOptions, ContentService contentService, PlaylistService playlistService)
        {
            _contentOptions = contentOptions;
            _contentService = contentService;
            _playlistService = playlistService;
        }

        async Task<List<PlaylistKey>> ImportPlaylists(IEnumerable<ContentExport> contents)
        {
            /// look up all groups for content add
            var imported = new List<PlaylistKey>();

            var playlists = contents.Where(c =>
                !string.IsNullOrWhiteSpace(c.PlaylistGlobalId) || !string.IsNullOrWhiteSpace(c.PlaylistName))
                .Select(c => new { Name = c.PlaylistName, Description = c.PlaylistDescription, LogoUrl = c.PlaylistLogoUrl, GlobalId = c.PlaylistGlobalId, Summary = c.PlaylistSummary, TrailerUrl = c.PlaylistTrailerUrl, PublisherGlobalId = c.PlaylistPublisherGlobalId });

            foreach (var pl in playlists)
            {
                if (!imported.Any(g => g.GlobalId.ToLower() == pl.GlobalId.ToLower()))
                {
                    var import = await _playlistService.Import(pl.Name, pl.Description, pl.GlobalId, pl.LogoUrl, pl.TrailerUrl, pl.Summary, pl.PublisherGlobalId);

                    imported.Add(new PlaylistKey { Id = import.Id, GlobalId = import.GlobalId.ToLower(), Name = import.Name });
                }
            }

            return imported;
        }

        public string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();

            if (ext == "csv")
                mimeType = "text/csv";

            if (ext == "zip")
                mimeType = "application/zip";

            return mimeType;
        }

        public abstract Task<IEnumerable<ContentExport>> Convert(IFormFile file);

        public class ContentKey
        {
            public int Id { get; set; }
            public string GlobalId { get; set; }
        }

        public class PlaylistKey
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string GlobalId { get; set; }
        }

        public virtual async Task<IEnumerable<ImportResult>> Import(IEnumerable<ContentExport> importContents)
        {
            var result = new List<ImportResult>();

            // import all playlists
            var importedPlaylists = await ImportPlaylists(importContents);

            var db = _contentService.DbContext;

            var organize = new Dictionary<int, Dictionary<string, List<int>>>();
            var orderedContents = importContents.OrderBy(c => c.SectionOrder).ThenBy(c => c.SectionContentOrder);

            var importContentGlobalIds = importContents.Select(ic => ic.GlobalId.ToLower()).Distinct();

            var contents = await db.Contents.Where(c => importContentGlobalIds.Contains(c.GlobalId.ToLower()))
                .Select(c => new ContentKey { Id = c.Id, GlobalId = c.GlobalId.ToLower() }).ToListAsync();

            foreach (var importContent in orderedContents)
            {
                var importResult = new ImportResult();

                int contentId = 0;

                if (!string.IsNullOrWhiteSpace(importContent.GlobalId))
                {
                    // lookup existing entity
                    var contentKey = contents.FirstOrDefault(ck => ck.GlobalId == importContent.GlobalId.ToLower());
                    contentId = contentKey == null ? 0 : contentKey.Id;
                }

                // pull playlist from lookup
                var playlistKey = importedPlaylists.FirstOrDefault(p => p.GlobalId == importContent.PlaylistGlobalId.ToLower())
                    ?? importedPlaylists.FirstOrDefault(p => p.Name == importContent.PlaylistName);

                ContentDetail saved = null;

                try
                {
                    if (contentId == 0)
                    {
                        var add = new ContentCreate
                        {
                            Name = importContent.Name,
                            Description = importContent.Description,
                            StartDate = importContent.StartDate,
                            StartTime = importContent.StartTime,
                            GlobalId = importContent.GlobalId,
                            EndDate = importContent.EndDate,
                            EndTime = importContent.EndTime,
                            LogoUrl = importContent.LogoUrl,
                            HoverUrl = importContent.HoverUrl,
                            ThumbnailUrl = importContent.ThumbnailUrl,
                            Tags = ConvertTags(importContent.Tags),
                            Settings = importContent.Settings,
                            Order = importContent.Order,
                            Type = importContent.Type,
                            Url = importContent.Url,
                            Summary = importContent.Summary,
                            TrailerUrl = importContent.TrailerUrl,
                            PublisherId = importContent.PublisherGlobalId,
                            PublisherName = importContent.PublisherName
                        };

                        saved = await _contentService.Add(add, true);

                        contents.Add(new ContentKey { Id = saved.Id, GlobalId = saved.GlobalId });
                    }
                    else
                    {
                        var update = new ContentUpdate
                        {
                            Id = contentId,
                            Name = importContent.Name,
                            Description = importContent.Description,
                            StartDate = importContent.StartDate,
                            StartTime = importContent.StartTime,
                            EndDate = importContent.EndDate,
                            EndTime = importContent.EndTime,
                            LogoUrl = importContent.LogoUrl,
                            HoverUrl = importContent.HoverUrl,
                            ThumbnailUrl = importContent.ThumbnailUrl,
                            Tags = ConvertTags(importContent.Tags),
                            Settings = importContent.Settings,
                            Order = importContent.Order,
                            Type = importContent.Type,
                            Url = importContent.Url,
                            Summary = importContent.Summary,
                            TrailerUrl = importContent.TrailerUrl,
                            PublisherId = importContent.PublisherGlobalId,
                            PublisherName = importContent.PublisherName
                        };

                        saved = await _contentService.Update(update, true);
                    }

                    importResult.Messages.Add(string.Format("Content '{0}' '{1}' imported.", saved.Name, saved.GlobalId));

                    if (playlistKey != null)
                    {
                        if (!organize.ContainsKey(playlistKey.Id))
                        {
                            organize.Add(playlistKey.Id, new Dictionary<string, List<int>>());
                        }
                        var sectionName = string.IsNullOrWhiteSpace(importContent.SectionName) ? "_" : importContent.SectionName;

                        if (!organize[playlistKey.Id].ContainsKey(sectionName))
                        {
                            organize[playlistKey.Id].Add(sectionName, new List<int>());
                        }

                        organize[playlistKey.Id][sectionName].Add(saved.Id);
                    }

                }
                catch (Exception ex)
                {
                    importResult.Messages.Add(string.Format("Content '{0}' '{1}' import failed.", importContent.Name, importContent.GlobalId));
                    importResult.Errors.Add(ex.Message);
                }

                result.Add(importResult);
            }

            // create playlist sections and content order
            foreach (var o in organize)
            {
                var playlistOrganizeResult = new ImportResult();

                var sections = new List<PlaylistSectionUpdate>();

                foreach (var sectionKeyValue in o.Value)
                {
                    sections.Add(new PlaylistSectionUpdate
                    {
                        Name = sectionKeyValue.Key,
                        ContentIds = sectionKeyValue.Value.Distinct().ToArray()
                    });
                }

                if (sections.Any())
                {
                    if (await _playlistService.Organize(o.Key, sections))
                    {
                        playlistOrganizeResult.Messages.Add(string.Format("Playlist '{0}' organized with '{1}' sections.", o.Key, sections.Count()));
                    }
                    else
                    {
                        playlistOrganizeResult.Messages.Add(string.Format("Playlist '{0}' organize failed.", o.Key));
                    }

                    result.Add(playlistOrganizeResult);
                }
            }

            return result;
        }

        string[] ConvertTags(string importTags)
        {
            string[] tags = new string[] { };

            if (!string.IsNullOrWhiteSpace(importTags))
            {
                tags = importTags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToArray();
            }

            return tags;
        }
    }
}

