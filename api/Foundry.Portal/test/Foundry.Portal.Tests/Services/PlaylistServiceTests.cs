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
using Stack.Http.Exceptions;
using System.Linq;
using Xunit;

namespace Foundry.Portal.Tests.Services
{
    [Collection("AutoMapper")]
    public class PlaylistServiceTests : ServiceTests
    {
        [Fact]
        public async void AddContent_UserCanAddContentToPlaylist()
        {
            using (var init = CreateTestContext())
            {
                init.TestDataFactory.AddProfileAndSetContextProfile("jack@this.test");

                var content1 = init.TestDataFactory.AddContent("one");
                var content2 = init.TestDataFactory.AddContent("two");

                var playlistService = init.GetService<PlaylistService>();
                var playlist = await playlistService.Add(new PlaylistCreate() { Name = "Playlist", LogoUrl = "http://logo.com/url" });

                await playlistService.AddContent(playlist.Id, content1.Id);
                await playlistService.AddContent(playlist.Id, content2.Id);

                var contentService = init.GetService<ContentService>();
                var contents = await contentService.GetAllByPlaylistId(playlist.Id, new ContentDataFilter());

                Assert.Equal(2, contents.Total);
            }
        }

        [Fact]
        public async void AddContent_UserCanAddContentToDefaultPlaylist()
        {
            using (var init = CreateTestContext())
            {
                init.TestDataFactory.AddProfileAndSetContextProfile("jack@this.test");

                var content1 = init.TestDataFactory.AddContent("one");
                var content2 = init.TestDataFactory.AddContent("two");
                var content3 = init.TestDataFactory.AddContent("three");

                var playlistService = init.GetService<PlaylistService>();
                var playlist = await playlistService.Add(new PlaylistCreate() { Name = "Default Playlist", IsDefault = true, LogoUrl = "http://logo.com/url" });

                await playlistService.AddContentToDefaultPlaylist(content1.Id);
                await playlistService.AddContentToDefaultPlaylist(content2.Id);
                await playlistService.AddContentToDefaultPlaylist(content3.Id);

                var contentService = init.GetService<ContentService>();
                var contents = await contentService.GetAllByPlaylistId(playlist.Id, new ContentDataFilter());

                Assert.Equal(3, contents.Total);
            }
        }

        [Fact]
        public async void GetAll_OnlyReturnsPublicPlaylists()
        {
            using (var init = CreateTestContext())
            {
                init.TestDataFactory.AddProfileAndSetContextProfile("owner@this.test");
                var ownerService = init.GetService<PlaylistService>();

                await ownerService.Add(new PlaylistCreate() { Name = "Public 1", IsDefault = true, IsPublic = true, LogoUrl = "http://logo.com/url" });
                await ownerService.Add(new PlaylistCreate() { Name = "Private 1", LogoUrl = "http://logo.com/url" });
                await ownerService.Add(new PlaylistCreate() { Name = "Public 2", IsPublic = true, LogoUrl = "http://logo.com/url" });
                await ownerService.Add(new PlaylistCreate() { Name = "Public 3", IsPublic = true, LogoUrl = "http://logo.com/url" });
                await ownerService.Add(new PlaylistCreate() { Name = "Private 2", LogoUrl = "http://logo.com/url" });

                init.TestDataFactory.AddProfileAndSetContextProfile("observer@this.test");
                var observerService = init.GetService<PlaylistService>();

                var publicPlaylists = await observerService.GetAll(new PlaylistDataFilter());

                Assert.Equal(3, publicPlaylists.Total);
            }
        }

        [Fact]
        public async void Follow_ObserverCanFollowPublicPlaylist()
        {
            using (var init = CreateTestContext())
            {
                init.TestDataFactory.AddProfileAndSetContextProfile("owner@this.test");
                var ownerService = init.GetService<PlaylistService>();

                await ownerService.Add(new PlaylistCreate() { Name = "Public 1", IsDefault = true, IsPublic = true, LogoUrl = "http://logo.com/url" });
                await ownerService.Add(new PlaylistCreate() { Name = "Private 1", LogoUrl = "http://logo.com/url" });
                await ownerService.Add(new PlaylistCreate() { Name = "Public 2", IsPublic = true, LogoUrl = "http://logo.com/url" });
                await ownerService.Add(new PlaylistCreate() { Name = "Public 3", IsPublic = true, LogoUrl = "http://logo.com/url" });
                await ownerService.Add(new PlaylistCreate() { Name = "Private 2", LogoUrl = "http://logo.com/url" });

                init.TestDataFactory.AddProfileAndSetContextProfile("observer@this.test");
                var observerService = init.GetService<PlaylistService>();

                var initialPublicPlaylists = await observerService.GetAll(new PlaylistDataFilter());
                Assert.Equal(3, initialPublicPlaylists.Results.Count(p => p.CanFollow));

                await observerService.Follow(initialPublicPlaylists.Results.First().Id);

                var afterFollowingPublicPlaylists = await observerService.GetAll(new PlaylistDataFilter());
                Assert.Equal(2, afterFollowingPublicPlaylists.Results.Count(p => p.CanFollow));
            }
        }

        [Fact]
        public async void Follow_ObserverCannotFollowPlaylistAlreadyFollowed()
        {
            using (var init = CreateTestContext())
            {
                init.TestDataFactory.AddProfileAndSetContextProfile("owner@this.test");
                var ownerService = init.GetService<PlaylistService>();

                await ownerService.Add(new PlaylistCreate() { Name = "Public 1", IsDefault = true, IsPublic = true, LogoUrl = "http://logo.com/url" });
                await ownerService.Add(new PlaylistCreate() { Name = "Private 1", LogoUrl = "http://logo.com/url" });
                await ownerService.Add(new PlaylistCreate() { Name = "Public 2", IsPublic = true, LogoUrl = "http://logo.com/url" });
                await ownerService.Add(new PlaylistCreate() { Name = "Public 3", IsPublic = true, LogoUrl = "http://logo.com/url" });
                await ownerService.Add(new PlaylistCreate() { Name = "Private 2", LogoUrl = "http://logo.com/url" });

                init.TestDataFactory.AddProfileAndSetContextProfile("observer@this.test");
                var observerService = init.GetService<PlaylistService>();

                var initialPublicPlaylists = await observerService.GetAll(new PlaylistDataFilter());
                var playlistId = initialPublicPlaylists.Results.First().Id;
                await observerService.Follow(playlistId);
                var afterFollowingPublicPlaylists = await observerService.GetAll(new PlaylistDataFilter());

                Assert.ThrowsAsync<EntityPermissionException>(() => observerService.Follow(playlistId)).Wait();
            }
        }

        [Fact]
        public async void Follow_ObserverCannotFollowPrivatePlaylist()
        {
            using (var init = CreateTestContext())
            {
                init.TestDataFactory.AddProfileAndSetContextProfile("owner@this.test");
                var ownerService = init.GetService<PlaylistService>();

                await ownerService.Add(new PlaylistCreate() { Name = "Public 1", IsDefault = true, IsPublic = true, LogoUrl = "http://logo.com/url" });
                await ownerService.Add(new PlaylistCreate() { Name = "Private 1", LogoUrl = "http://logo.com/url" });

                var playlists = await ownerService.GetAllByProfileId(init.Profile.Id, new PlaylistDataFilter());
                var privatePlaylistId = playlists.Results.First(p => !p.IsPublic).Id;

                init.TestDataFactory.AddProfileAndSetContextProfile("observer@this.test");
                var observerService = init.GetService<PlaylistService>();

                Assert.ThrowsAsync<EntityNotFoundException>(() => observerService.Follow(privatePlaylistId)).Wait();
            }
        }

        [Fact]
        public async void Unfollow_ObserverCannotUnfollowPlaylistNotFollowed()
        {
            using (var init = CreateTestContext())
            {
                init.TestDataFactory.AddProfileAndSetContextProfile("owner@this.test");
                var ownerService = init.GetService<PlaylistService>();

                await ownerService.Add(new PlaylistCreate() { Name = "Public 1", IsDefault = true, IsPublic = true, LogoUrl = "http://logo.com/url" });
                await ownerService.Add(new PlaylistCreate() { Name = "Public 2", IsPublic = true, LogoUrl = "http://logo.com/url" });
                await ownerService.Add(new PlaylistCreate() { Name = "Public 3", IsPublic = true, LogoUrl = "http://logo.com/url" });

                init.TestDataFactory.AddProfileAndSetContextProfile("observer@this.test");
                var observerService = init.GetService<PlaylistService>();

                var initialPublicPlaylists = await observerService.GetAll(new PlaylistDataFilter());
                var playlistId = initialPublicPlaylists.Results.First().Id;

                Assert.ThrowsAsync<EntityPermissionException>(() => observerService.Unfollow(playlistId)).Wait();
            }
        }

        [Fact]
        public async void Delete_ObserverCannotDeletePlaylistTheyDoNotOwn()
        {
            using (var init = CreateTestContext())
            {
                init.TestDataFactory.AddProfileAndSetContextProfile("owner@this.test");
                var ownerService = init.GetService<PlaylistService>();

                await ownerService.Add(new PlaylistCreate() { Name = "Public 1", IsDefault = true, IsPublic = true, LogoUrl = "http://logo.com/url" });
                await ownerService.Add(new PlaylistCreate() { Name = "Private 1", LogoUrl = "http://logo.com/url" });
                await ownerService.Add(new PlaylistCreate() { Name = "Public 2", IsPublic = true, LogoUrl = "http://logo.com/url" });

                init.TestDataFactory.AddProfileAndSetContextProfile("observer@this.test");
                var observerService = init.GetService<PlaylistService>();

                var initialPublicPlaylists = await observerService.GetAll(new PlaylistDataFilter());

                var playlistId = initialPublicPlaylists.Results.First().Id;

                Assert.ThrowsAsync<EntityPermissionException>(() => observerService.Delete(playlistId)).Wait();
            }
        }

        [Fact]
        public async void Follow_OwnerCannotFollowOwnPlaylist()
        {
            using (var init = CreateTestContext())
            {
                init.TestDataFactory.AddProfileAndSetContextProfile("owner@this.test");
                var ownerService = init.GetService<PlaylistService>();

                await ownerService.Add(new PlaylistCreate() { Name = "Public 1", IsDefault = true, IsPublic = true, LogoUrl = "http://logo.com/url" });
                await ownerService.Add(new PlaylistCreate() { Name = "Private 1", LogoUrl = "http://logo.com/url" });
                await ownerService.Add(new PlaylistCreate() { Name = "Public 2", IsPublic = true, LogoUrl = "http://logo.com/url" });
                await ownerService.Add(new PlaylistCreate() { Name = "Public 3", IsPublic = true, LogoUrl = "http://logo.com/url" });
                await ownerService.Add(new PlaylistCreate() { Name = "Private 2", LogoUrl = "http://logo.com/url" });

                var playlists = await ownerService.GetAllByProfileId(init.Profile.Id, new PlaylistDataFilter());
                var playlistId = playlists.Results.First().Id;
                Assert.ThrowsAsync<InvalidIdentityException>(() => ownerService.Follow(playlistId)).Wait();
            }
        }
    }
}
