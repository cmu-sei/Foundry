/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.Extensions.Logging;
using Foundry.Portal.Data.Entities;
using Foundry.Portal.Events;
using Foundry.Portal.Repositories;
using Foundry.Portal.Security;
using Foundry.Portal.Services;
using Foundry.Portal.ViewModels;
using Stack.Validation.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Foundry.Portal.Data.Generator.Templates
{
    public class ExternalDataTemplate
    {
        public ExternalDataTemplate(CoreOptions options, SketchDbContext dbContext, ILoggerFactory loggerFactory, IValidationHandler validationHandler)
        {
            _options = options;
            _dbContext = dbContext;
            _loggerFactory = loggerFactory;
            _validationHandler = validationHandler;
        }

        CoreOptions _options;
        SketchDbContext _dbContext;
        readonly ILoggerFactory _loggerFactory;
        readonly IValidationHandler _validationHandler;

        ProfileService profileService;
        ContentService contentService;
        PlaylistService playlistService;

        private void SetActor(string globalId)
        {
            var profileCache = new DataProfileCache();
            var validationHandler = new StrictValidationHandler(_dbContext);
            var extensionResolver = new DataExtensionResolver(_options.Extension);
            var webHookHandler = new DataWebHookHandler();
            var del = new DataDomainEventDelegator(webHookHandler);
            var dispatcher = new DomainEventDispatcher(del);

            var profile = _dbContext.Profiles.SingleOrDefault(p => p.GlobalId.ToLower() == globalId.ToLower());

            var identityResolver = new DataFactoryIdentityResolver(profile);

            var profileRepo = new ProfileRepository(_dbContext, new ProfilePermissionMediator(identityResolver));
            var contentRepo = new ContentRepository(_dbContext, new ContentPermissionMediator(identityResolver));
            var tagRepo = new TagRepository(_dbContext);
            var discussionRepo = new DiscussionRepository(_dbContext);

            profileService = new ProfileService(profileRepo, dispatcher, _options, identityResolver, _loggerFactory, null, profileCache);
            contentService = new ContentService(contentRepo, tagRepo, discussionRepo, profileRepo, validationHandler, dispatcher, _options, identityResolver, _loggerFactory, null);
            playlistService = new PlaylistService(new PlaylistRepository(_dbContext, new PlaylistPermissionMediator(identityResolver)), tagRepo, validationHandler, _options, identityResolver, _loggerFactory, dispatcher, null);
        }

        public void Run()
        {
            DateTime dtNow = DateTime.UtcNow;
            List<Seed.Profile> profiles = new List<Seed.Profile>();
            List<Seed.Channel> channels = new List<Seed.Channel>();
            List<Seed.Subscription> subscriptions = new List<Seed.Subscription>();
            List<Seed.Membership> memberships = new List<Seed.Membership>();
            List<Seed.Content> contents = new List<Seed.Content>();
            List<Seed.Playlist> playlists = new List<Seed.Playlist>();
            List<Seed.ItemLink> playlistContents = new List<Seed.ItemLink>();

            string filename = "db-export.data";
            string[] lines = File.ReadAllLines(filename);
            foreach (string line in lines)
            {
                if (line.Trim().StartsWith("#"))
                    continue;

                string[] fields = line.Split('|');
                switch (fields[0])
                {
                    case "Profile":
                        profiles.Add(new Seed.Profile { Id = Int32.Parse(fields[1]), Name = fields[2], GlobalId = fields[3] });
                        break;

                    case "Channel":
                        channels.Add(new Seed.Channel
                        {
                            Id = Int32.Parse(fields[1]),
                            Name = fields[2],
                            Description = fields[3].Replace("&&", "\n"),
                            ProfileId = Int32.Parse(fields[4]),
                            LogoUrl = fields[5],
                            Access = Int32.Parse(fields[6]),
                            GlobalId = fields[7]
                        });
                        break;

                    case "Subscription":
                        subscriptions.Add(new Seed.Subscription
                        {
                            Id = Int32.Parse(fields[1]),
                            ChannelId = Int32.Parse(fields[2]),
                            ProfileId = Int32.Parse(fields[3]),
                            Permissions = Int32.Parse(fields[4])

                        });
                        break;

                    case "Playlist":
                        playlists.Add(new Seed.Playlist
                        {
                            Id = Int32.Parse(fields[1]),
                            Name = fields[2],
                            Description = fields[3].Replace("&&", "\n"),
                            ProfileId = Int32.Parse(fields[4]),
                            LogoUrl = fields[5],
                            IsPublic = Int32.Parse(fields[6]) > 0,
                            GlobalId = fields[7]
                        });
                        break;

                    case "Membership":
                        memberships.Add(new Seed.Membership
                        {
                            Id = Int32.Parse(fields[1]),
                            GroupId = Int32.Parse(fields[2]),
                            ProfileId = Int32.Parse(fields[3]),
                            Permissions = Int32.Parse(fields[4])

                        });
                        break;

                    case "Content":
                        contents.Add(new Seed.Content
                        {
                            Id = Int32.Parse(fields[1]),
                            ChannelId = Int32.Parse(fields[2]),
                            ProfileId = channels.Where(c => c.Id == Int32.Parse(fields[2])).Select(c => c.ProfileId).FirstOrDefault(), //Int32.Parse(fields[3]),
                            GroupId = (String.IsNullOrWhiteSpace(fields[4]) ? (int?)null : Int32.Parse(fields[4])),
                            Name = fields[5],
                            Description = fields[6].Replace("&&", "\n"),
                            LogoUrl = fields[7],
                            HoverUrl = fields[8],
                            ThumbnailUrl = fields[9],
                            GlobalId = fields[10],
                            Tags = fields[11].Replace(" ", "|"),
                            Url = fields[12]
                        });
                        break;

                    case "PlaylistContent":
                        playlistContents.Add(new Seed.ItemLink
                        {
                            OwnerId = Int32.Parse(fields[1]),
                            ItemId = Int32.Parse(fields[2])
                        });
                        break;
                }
            }

            Dictionary<int, int> profileMap = new Dictionary<int, int>();
            Dictionary<int, int> channelMap = new Dictionary<int, int>();
            Dictionary<int, int> playlistMap = new Dictionary<int, int>();
            Dictionary<int, int> contentMap = new Dictionary<int, int>();

            //pass 1: create primary entities
            foreach (var profile in profiles)
            {
                Profile p = new Profile() { Name = profile.Name, GlobalId = profile.GlobalId };
                _dbContext.Profiles.Add(p);
                _dbContext.SaveChanges();
                profileMap.Add(profile.Id, p.Id);

                SetActor(p.GlobalId);

                foreach (var playlist in playlists.Where(g => g.ProfileId == profile.Id))
                {
                    var result = playlistService.Add(new PlaylistCreate
                    {
                        Name = playlist.Name,
                        LogoUrl = playlist.LogoUrl,
                        IsPublic = playlist.IsPublic
                    }).Result;
                    playlistMap.Add(playlist.Id, result.Id);
                }
            }

            //pass 2: link up subscriptions,memberships
            foreach (var profile in profiles)
            {
                SetActor(profile.GlobalId);
            }

            //pass 3: approve restricted subscriptions
            foreach (var channel in channels.Where(c => c.Access > 0))
            {
                SetActor(profiles.Where(p => p.Id == channel.ProfileId).Select(p => p.GlobalId).Single());
            }

            //pass 5: add content
            foreach (var profile in profiles)
            {
                var items = contents.Where(c => c.ProfileId == profile.Id).ToArray();
                if (items.Any())
                {
                    SetActor(profile.GlobalId);
                    foreach (var item in items)
                    {
                        var result = contentService.Add(new ContentCreate
                        {
                            Name = item.Name,
                            Description = item.Description,
                            Url = item.Url,
                            LogoUrl = item.LogoUrl,
                            HoverUrl = item.HoverUrl,
                            ThumbnailUrl = item.ThumbnailUrl,
                            Tags = item.Tags.Split('|'),
                            Type = (ContentType)Enum.Parse(typeof(ContentType), item.ContentType, true)
                        }).Result;
                        contentMap.Add(item.Id, result.Id);
                    }
                }
            }

            //pass 6: add playlist content
            foreach (var pset in playlistContents.GroupBy(p => p.OwnerId))
            {
                var playlist = playlists.Where(p => p.Id == pset.Key).SingleOrDefault();
                if (playlist != null)
                {
                    var profile = profiles.Where(p => p.Id == playlist.ProfileId).Single();
                    SetActor(profile.GlobalId);
                    foreach (var p in pset)
                        if (playlistMap.ContainsKey(p.OwnerId) && contentMap.ContainsKey(p.ItemId))
                            playlistService.AddContent(playlistMap[p.OwnerId], contentMap[p.ItemId]).Wait();
                }
            }

            //pass 8: random ratings
            Random rand = new Random();
            List<int> skiplist = new List<int>();
            foreach (var profile in profiles)
            {
                Console.WriteLine("{0} {1}", profile.Id, profile.Name);
                SetActor(profile.GlobalId);
                byte fi = Convert.ToByte(profile.Name.ToLower().ToCharArray().First());
                int affinity = ((fi - 97) / 8) + 1;
                int v = 0;
                bool rateit;
                foreach (var content in contents)
                {
                    if (skiplist.Contains(content.Id))
                        continue;

                    rateit = rand.Next(10) < 7;
                    if (rateit)
                    {
                        try
                        {
                            v = rand.Next(affinity + 1, 5);
                            contentService.AddOrUpdateRating(contentMap[content.Id], (Rating)v).Wait();
                            contentService.AddOrUpdateDifficulty(contentMap[content.Id], (Difficulty)affinity).Wait();
                        }
                        catch (Exception ex)
                        {
                            skiplist.Add(content.Id);
                            Console.WriteLine("{0} can't rate {1} {2}", profile.Id, content.Id, ex.Message);
                        }
                    }
                }
            }
        }
    }
}

