/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Foundry.Portal.Api.Controllers;
using Foundry.Portal.Services;
using Foundry.Portal.TestBed;
using Foundry.Portal.ViewModels;
using Stack.Http.Exceptions;
using System.Threading.Tasks;
using Xunit;

namespace Foundry.Portal.Api.Tests.Controllers
{
    [Collection("AutoMapper")]
    public class ProfileControllerTests : ControllerTests
    {
        ProfileController GetController(TestContext context)
        {
            return new ProfileController(new CoreOptions(),
                new LoggerFactory(),
                context.GetService<ProfileService>(),
                context.GetService<PlaylistService>(),
                context.GetService<ContentService>());
        }

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            using (var context = CreateTestContext())
            {
                var controller = GetController(context);
                Assert.IsType<OkObjectResult>(await controller.GetAll());
            }
        }

        [Fact]
        public async Task Get_IfProfileExistsReturnsOk()
        {
            using (var context = CreateTestContext())
            {
                var profile = context.TestDataFactory.AddProfileAndSetContextProfile("Get_IfProfileExistsReturnsOk@this.ws");
                var controller = GetController(context);
                Assert.IsType<OkObjectResult>(await controller.GetById(profile.Id));
            }
        }

        [Fact]
        public async Task Get_ProfileWithPlaylistsReturnsOk()
        {
            using (var context = CreateTestContext())
            {
                var profile = context.TestDataFactory.AddProfileAndSetContextProfile("Get_ProfileWithPlaylistsReturnsOk@this.ws");
                var controller = GetController(context);
                var playlistService = context.GetService<PlaylistService>();

                string logoUrl = "http://logourl.com";

                await playlistService.Add(new PlaylistCreate() { Name = "Playlist1", IsDefault = true, IsPublic = true, LogoUrl = logoUrl });
                await playlistService.Add(new PlaylistCreate() { Name = "Playlist2", IsPublic = true, LogoUrl = logoUrl });
                await playlistService.Add(new PlaylistCreate() { Name = "Playlist3", LogoUrl = logoUrl });

                Assert.IsType<OkObjectResult>(await controller.GetById(profile.Id));
            }
        }

        [Fact]
        public void Get_IfProfileDoesNotExistReturnsNotFound()
        {
            using (var context = CreateTestContext())
            {
                var controller = GetController(context);
                Assert.ThrowsAsync<EntityNotFoundException>(async () => await controller.GetById(77777)).Wait();
            }
        }
    }
}
