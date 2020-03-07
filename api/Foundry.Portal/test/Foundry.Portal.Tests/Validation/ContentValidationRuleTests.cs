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
using Foundry.Portal.Data;
using Foundry.Portal.Validation.ValidationRules;
using Foundry.Portal.ViewModels;
using System;
using System.Threading.Tasks;
using Xunit;
using Stack.Http.Exceptions;

namespace Foundry.Portal.Tests.Validation
{
    public class ContentValidationRuleTests : ValidationRuleTests
    {

        [Fact]
        public async Task Update_ContentIdIsValid()
        {
            var model = new ContentUpdate()
            {
                Id = 0,
                LogoUrl = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString(),
                Tags = new string[] { "tag" },
                Type = ContentType.Course
            };

            await Assert.ThrowsAsync<InvalidModelException>(async () => await _validationHandler.ValidateRule<ContentIdIsValid, ContentUpdate>(model));
        }

        [Fact]
        public async Task Update_ContentLogoUrlIsRequired()
        {
            var model = new ContentUpdate()
            {
                Id = 1,
                LogoUrl = null,
                Name = Guid.NewGuid().ToString(),
                Tags = new string[] { "tag" },
                Type = ContentType.Course
            };

            await Assert.ThrowsAsync<InvalidModelException>(async () => await _validationHandler.ValidateRule<ContentLogoUrlIsRequired, ContentUpdate>(model));
        }

        [Fact]
        public async Task Update_ContentNameIsRequired()
        {
            var model = new ContentUpdate()
            {
                Id = 1,
                LogoUrl = Guid.NewGuid().ToString(),
                Name = null,
                Tags = new string[] { "tag" },
                Type = ContentType.Course
            };

            await Assert.ThrowsAsync<InvalidModelException>(async () => await _validationHandler.ValidateRule<ContentNameIsRequired, ContentUpdate>(model));
        }

        [Fact]
        public async Task Update_ContentTagIsRequired()
        {
            var model = new ContentUpdate()
            {
                Id = 1,
                LogoUrl = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString(),
                Tags = null,
                Type = ContentType.Course
            };

            await Assert.ThrowsAsync<InvalidModelException>(async () => await _validationHandler.ValidateRule<ContentTagIsRequired, ContentUpdate>(model));
        }

        [Fact]
        public async Task Update_ContentTypeIsRequired()
        {
            var model = new ContentUpdate()
            {
                Id = 1,
                LogoUrl = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString(),
                Tags = new string[] { "tag" },
                Type = ContentType.NotSet
            };

            await Assert.ThrowsAsync<InvalidModelException>(async () => await _validationHandler.ValidateRule<ContentTypeIsRequired, ContentUpdate>(model));
        }

        [Fact]
        public async Task Create_ContentLogoUrlIsRequired()
        {
            var model = new ContentCreate()
            {
                LogoUrl = null,
                Name = Guid.NewGuid().ToString(),
                Tags = new string[] { "tag" },
                Type = ContentType.Course
            };

            await Assert.ThrowsAsync<InvalidModelException>(async () => await _validationHandler.ValidateRule<ContentLogoUrlIsRequired, ContentCreate>(model));
        }

        [Fact]
        public async Task Create_ContentNameIsRequired()
        {
            var model = new ContentCreate()
            {
                LogoUrl = Guid.NewGuid().ToString(),
                Name = null,
                Tags = new string[] { "tag" },
                Type = ContentType.Course
            };

            await Assert.ThrowsAsync<InvalidModelException>(async () => await _validationHandler.ValidateRule<ContentNameIsRequired, ContentCreate>(model));
        }

        [Fact]
        public async Task Create_ContentTagIsRequired()
        {
            var model = new ContentCreate()
            {
                LogoUrl = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString(),
                Tags = null,
                Type = ContentType.Course
            };

            await Assert.ThrowsAsync<InvalidModelException>(async () => await _validationHandler.ValidateRule<ContentTagIsRequired, ContentCreate>(model));
        }

        [Fact]
        public async Task Create_ContentTypeIsRequired()
        {
            var model = new ContentCreate()
            {
                LogoUrl = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString(),
                Tags = new string[] { "tag" },
                Type = ContentType.NotSet
            };

            await Assert.ThrowsAsync<InvalidModelException>(async () => await _validationHandler.ValidateRule<ContentTypeIsRequired, ContentCreate>(model));
        }
    }
}

