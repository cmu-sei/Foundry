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
using Foundry.Portal.Data.Entities;
using Foundry.Portal.Validation.ValidationRules;
using Foundry.Portal.ViewModels;
using System;
using System.Threading.Tasks;
using Xunit;
using Stack.Http.Exceptions;

namespace Foundry.Portal.Tests.Validation
{
    public class PlaylistValidationRuleTests : ValidationRuleTests
    {
        [Fact]
        public async Task Update_PlaylistIdIsValid()
        {
            var model = new PlaylistUpdate()
            {
                Id = 0,
                Name = Guid.NewGuid().ToString(),
                LogoUrl = Guid.NewGuid().ToString()
            };

            await Assert.ThrowsAsync<InvalidModelException>(async () => await _validationHandler.ValidateRule<PlaylistIdIsValid, PlaylistUpdate>(model));
        }

        [Fact]
        public async Task Update_PlaylistLogoUrlIsRequired()
        {
            var model = new PlaylistUpdate()
            {
                Id = 1,
                Name = Guid.NewGuid().ToString(),
                LogoUrl = null
            };

            await Assert.ThrowsAsync<InvalidModelException>(async () => await _validationHandler.ValidateRule<PlaylistLogoUrlIsRequired, PlaylistUpdate>(model));
        }

        [Fact]
        public async Task Update_PlaylistNameIsRequired()
        {
            var model = new PlaylistUpdate()
            {
                Id = 1,
                Name = null,
                LogoUrl = Guid.NewGuid().ToString()
            };

            await Assert.ThrowsAsync<InvalidModelException>(async () => await _validationHandler.ValidateRule<PlaylistNameIsRequired, PlaylistUpdate>(model));
        }

        [Fact]
        public async Task Update_PlaylistNameIsUnique()
        {
            var one = new Playlist() { Name = "Unique" };
            _dbContext.Playlists.Add(one);
            await _dbContext.SaveChangesAsync();

            var two = new Playlist() { Name = "Once Unique" };
            _dbContext.Playlists.Add(two);
            await _dbContext.SaveChangesAsync();

            var model = new PlaylistUpdate()
            {
                Id = two.Id,
                Name = one.Name
            };

            await Assert.ThrowsAsync<InvalidModelException>(async () => await _validationHandler.ValidateRule<PlaylistNameIsUnique, PlaylistUpdate>(model));
        }

        [Fact]
        public async Task Create_PlaylistLogoUrlIsRequired()
        {
            var model = new PlaylistCreate()
            {
                Name = Guid.NewGuid().ToString(),
                LogoUrl = null
            };

            await Assert.ThrowsAsync<InvalidModelException>(async () => await _validationHandler.ValidateRule<PlaylistLogoUrlIsRequired, PlaylistCreate>(model));
        }

        [Fact]
        public async Task Create_PlaylistNameIsRequired()
        {
            var model = new PlaylistCreate()
            {
                Name = null,
                LogoUrl = Guid.NewGuid().ToString()
            };

            await Assert.ThrowsAsync<InvalidModelException>(async () => await _validationHandler.ValidateRule<PlaylistNameIsRequired, PlaylistCreate>(model));
        }

        [Fact]
        public async Task Create_PlaylistNameIsUnique()
        {
            var playlist = new Playlist() { Name = "Not Unique" };
            _dbContext.Playlists.Add(playlist);
            await _dbContext.SaveChangesAsync();

            var model = new PlaylistCreate()
            {
                Name = playlist.Name,
                LogoUrl = Guid.NewGuid().ToString()
            };

            await Assert.ThrowsAsync<InvalidModelException>(async () => await _validationHandler.ValidateRule<PlaylistNameIsUnique, PlaylistCreate>(model));
        }
    }
}
