/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Stack;
using Foundry.Portal.Services;
using Foundry.Portal.ViewModels;
using System;
using Xunit;
using Stack.Http.Exceptions;

namespace Foundry.Portal.Tests.Services
{
    [Collection("AutoMapper")]
    public class ProfileServiceTests : ServiceTests
    {
        [Fact]
        public async void GetByName_ReturnsCorrectProfile()
        {
            var guid = Guid.NewGuid().ToString();

            using (var context = CreateTestContext())
            {
                var profile = context.TestDataFactory.AddProfileAndSetContextProfile(guid);
                var profileService = context.GetService<ProfileService>();
                var fetched = await profileService.GetByName(guid);

                Assert.Equal(fetched.Id, profile.Id);
            }
        }

        [Fact]
        public async void GetByGlobalId_ReturnsCorrectProfile()
        {
            var guid = Guid.NewGuid().ToString();

            using (var context = CreateTestContext())
            {
                var profile = context.TestDataFactory.AddProfileAndSetContextProfile(guid);
                var profileService = context.GetService<ProfileService>();
                var fetched = await profileService.GetByGlobalId(profile.GlobalId);

                Assert.Equal(fetched.Id, profile.Id);
            }
        }

        [Fact]
        public async void Update_AllowsProfileToUpdateItsOwnRecord()
        {
            var guid = Guid.NewGuid().ToString();

            var updatedName = Guid.NewGuid().ToString();

            using (var context = CreateTestContext())
            {
                var profile = context.TestDataFactory.AddProfileAndSetContextProfile(guid);
                var profileService = context.GetService<ProfileService>();

                var model = new ProfileUpdate
                {
                    Id = profile.Id,
                    Name = updatedName
                };

                var updated = await profileService.Update(model);
                var fetched = await profileService.GetByName(updatedName);

                Assert.Equal(profile.Id, fetched.Id);
            }
        }

        [Fact]
        public void Update_DisallowsProfileBeUpdatedByAnotherProfile()
        {
            var goodGuy = Guid.NewGuid().ToString();
            var badGuy = Guid.NewGuid().ToString();

            using (var context = CreateTestContext())
            {
                var profile = context.TestDataFactory.AddProfile(goodGuy);

                context.TestDataFactory.AddProfileAndSetContextProfile(badGuy);

                var profileService = context.GetService<ProfileService>();

                var model = new ProfileUpdate
                {
                    Id = profile.Id,
                    Name = "I changed your name"
                };

                Assert.ThrowsAsync<EntityPermissionException>(async () => await profileService.Update(model)).Wait();
            }
        }
    }
}
