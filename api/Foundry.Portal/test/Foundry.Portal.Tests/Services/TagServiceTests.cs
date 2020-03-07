/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Foundry.Portal.Data;
using Foundry.Portal.Services;
using Foundry.Portal.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Foundry.Portal.Tests.Services
{
    [Collection("AutoMapper")]
    public class TagServiceTests : ServiceTests
    {
        [Fact]
        public void AddContentTags()
        {
            using (var test = CreateTestContext())
            {
                test.TestDataFactory.AddProfileAndSetContextProfile("tags@channel.tests");
                var content = test.TestDataFactory.AddContent("tags");

                var tagService = test.GetService<TagService>();
                var contentService = test.GetService<ContentService>();

                var check1Tags = new string[] { "TAG 1", "tag 2", "Tag 3" };

                tagService.SetContentTags(content.Id, check1Tags).Wait();

                var check1TagsFetch = contentService.GetById(content.Id).Result.Tags;

                Assert.Equal(3, check1TagsFetch.Count);
            }
        }

        [Fact]
        public async Task AddPlaylistTags()
        {
            using (var test = CreateTestContext())
            {
                test.TestDataFactory.AddProfileAndSetContextProfile("tags@channel.tests");
                var tagService = test.GetService<TagService>();
                var playlistService = test.GetService<PlaylistService>();

                var playlist = await playlistService.Add(new ViewModels.PlaylistCreate() { Name = "tags test", IsDefault = true, IsPublic = true, LogoUrl = "http://logo.url" });

                var check1Tags = new string[] { "TAG 1", "tag 2", "Tag 3", "tag 4", "tag 5" };

                await tagService.SetPlaylistTags(playlist.Id, check1Tags);

                var check1TagsFetch = playlistService.GetById(playlist.Id).Result.Tags;

                Assert.Equal(5, check1TagsFetch.Count);
            }
        }

        [Fact]
        public async Task AddPlaylistAndContentTagsAndDeleteTag()
        {
            using (var test = CreateTestContext())
            {
                test.TestDataFactory.AddProfileAndSetContextProfile("tags@channel.tests", SystemPermissions.PowerUser);

                var tagService = test.GetService<TagService>();
                var playlistService = test.GetService<PlaylistService>();
                var contentService = test.GetService<ContentService>();

                var content = test.TestDataFactory.AddContent("pttags");
                var playlist = await playlistService.Add(new PlaylistCreate() { Name = "tags test", IsDefault = true, IsPublic = true, LogoUrl = "http://logo.url" });

                await tagService.SetContentTags(content.Id, "aaaaa", "bbbbb", "delete", "ccccc", "ddddd", "eeeee");
                content = await contentService.GetById(content.Id);
                Assert.Equal(6, content.Tags.Count);
                Assert.True(content.Tags.Any(t => t.Name == "delete"), "content does not contains 'delete' tag.");

                await tagService.SetPlaylistTags(playlist.Id, "aaaaa", "bbbbb", "delete", "ccccc", "ddddd");
                playlist = await playlistService.GetById(playlist.Id);
                Assert.Equal(5, playlist.Tags.Count);
                Assert.True(playlist.Tags.Any(t => t.Name == "delete"), "playlist does not contains 'delete' tag.");

                await tagService.Delete("delete");

                content = await contentService.GetById(content.Id);
                Assert.Equal(5, content.Tags.Count);
                Assert.False(content.Tags.Any(t => t.Name == "delete"), "content contains 'delete' tag.");

                playlist = await playlistService.GetById(playlist.Id);
                Assert.Equal(4, playlist.Tags.Count);
                Assert.False(playlist.Tags.Any(t => t.Name == "delete"), "playlist contains 'delete' tag.");
            }
        }


        [Fact]
        public void AddAndUpdateContentTags()
        {
            using (var test = CreateTestContext())
            {
                test.TestDataFactory.AddProfileAndSetContextProfile("tags@channel.tests");
                var content = test.TestDataFactory.AddContent("tags");

                var tagService = test.GetService<TagService>();
                var contentService = test.GetService<ContentService>();

                var check1Tags = new string[] { "TAG 1" };

                tagService.SetContentTags(content.Id, check1Tags).Wait();

                var check1Content = contentService.GetById(content.Id).Result;

                Assert.Equal(check1Tags.Length, check1Content.Tags.Count);

                var check2Tags = check1Content.Tags.Select(t => t.Slug).ToList();

                check2Tags.Add("new tag");

                tagService.SetContentTags(content.Id, check2Tags.ToArray()).Wait();

                var check2TagsFetch = contentService.GetById(content.Id).Result.Tags;

                Assert.Equal(2, check2TagsFetch.Count);
            }
        }

        [Fact]
        public void AddAndRemoveContentTags()
        {
            using (var test = CreateTestContext())
            {
                test.TestDataFactory.AddProfileAndSetContextProfile("tags@channel.tests");
                var content = test.TestDataFactory.AddContent("remove content");

                var tagService = test.GetService<TagService>();
                var contentService = test.GetService<ContentService>();

                var check1Tags = new string[] { "TAG 1", "tAg 2", "TaG 3" };

                tagService.SetContentTags(content.Id, check1Tags).Wait();

                var check1Content = contentService.GetById(content.Id).Result;

                Assert.Equal(check1Tags.Length, check1Content.Tags.Count);

                var check2Tags = new string[] { "new tag" };

                tagService.SetContentTags(content.Id, check2Tags.ToArray()).Wait();

                var check2TagsFetch = contentService.GetById(content.Id).Result.Tags;

                Assert.Single(check2TagsFetch);
            }
        }

        [Fact]
        public void AddAndRemoveAllContentTags()
        {
            using (var test = CreateTestContext())
            {
                test.TestDataFactory.AddProfileAndSetContextProfile("tags@channel.tests");
                var content = test.TestDataFactory.AddContent("content tags");

                var tagService = test.GetService<TagService>();
                var contentService = test.GetService<ContentService>();

                var check1Tags = new string[] { "TAG 1", "tAg 2", "TaG 3" };

                tagService.SetContentTags(content.Id, check1Tags).Wait();

                var check1Content = contentService.GetById(content.Id).Result;

                Assert.Equal(check1Tags.Length, check1Content.Tags.Count);

                var check2Tags = new string[] { };

                tagService.SetContentTags(content.Id, check2Tags.ToArray()).Wait();

                var check2TagsFetch = contentService.GetById(content.Id).Result.Tags;

                Assert.Empty(check2TagsFetch);
            }
        }

        [Fact]
        public void VerifyTagsFetchedAndBuilt()
        {
            using (var test = CreateTestContext())
            {
                test.TestDataFactory.AddProfileAndSetContextProfile("tags@channel.tests");
                var content = test.TestDataFactory.AddContent("fetched");

                var tagService = test.GetService<TagService>();
                var contentService = test.GetService<ContentService>();

                var tags = new string[] { "TAG 1", "tAg 2", "TaG 3" };

                tagService.SetContentTags(content.Id, tags).Wait();

                var check1Content = contentService.GetById(content.Id).Result;

                var tagCheck = new List<string>(tags)
                {
                    "id is zero",
                    "id is zero, too"
                };

                var tagsFromNames = tagService.GetAndCreateTagsFromTagNames(tagCheck.ToArray());

                Assert.Equal(2, tagsFromNames.Count(t => t.Id == 0));
            }
        }
    }
}
