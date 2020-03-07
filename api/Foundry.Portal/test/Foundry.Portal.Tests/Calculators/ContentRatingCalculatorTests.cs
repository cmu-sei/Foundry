/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Foundry.Portal.Calculators;
using Foundry.Portal.Data;
using Foundry.Portal.Data.Entities;
using Foundry.Portal.Events;
using Foundry.Portal.Repositories;
using Foundry.Portal.Services;
using Foundry.Portal.TestBed;
using Foundry.Portal.ViewModels;
using Stack.Validation.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Foundry.Portal.Tests.Calculators
{
    public class ContentRatingCalculatorTests
    {
        DomainEventDelegator DomainEventDelegator()
        {
            return new DomainEventDelegator(new DomainEventDispatcherOptions(),
                new TestWebHookHandler(), new TestNotificationHandler(), new TestContentRatingCalculator(), new TestPlaylistRatingCalculator(), new TestContentDifficultyCalculator());
        }

        DomainEventDispatcher DomainEventDispatcher()
        {
            return new DomainEventDispatcher(DomainEventDelegator());
        }

        ContentService ContentService(TestContext<SketchDbContext> testContext)
        {
            var dbContext = testContext.DbContext;

            var identityResolver = new TestIdentityResolver(testContext.Profile);

            return new ContentService(
                new ContentRepository(dbContext, new Security.ContentPermissionMediator(identityResolver)),
                new TagRepository(dbContext),
                new DiscussionRepository(dbContext),
                new ProfileRepository(dbContext, new Security.ProfilePermissionMediator(identityResolver)),
                new StrictValidationHandler(testContext.DbContext),
                DomainEventDispatcher(),
                new CoreOptions(),
                identityResolver,
                new TestLoggerFactory(),
                testContext.Mapper);
        }

        ProfileService ProfileService(TestContext<SketchDbContext> testContext)
        {
            var dbContext = testContext.DbContext;

            var id = new TestIdentityResolver(testContext.Profile);
            return new ProfileService(
                new ProfileRepository(dbContext, new Security.ProfilePermissionMediator(id)),
                DomainEventDispatcher(),
                new CoreOptions(),
                id,
                new TestLoggerFactory(), testContext.Mapper, new TestProfileCache());
        }

        readonly Action<TestContext<SketchDbContext>> seed = (testContext) =>
        {
            var profile = new Profile { GlobalId = Guid.NewGuid().ToString(), Name = "test@cert.org" };

            testContext.DbContext.Profiles.Add(profile);
            testContext.DbContext.SaveChanges();

            testContext.Profile = profile;
        };

        [Fact]
        public async Task CalculateContentRating_CalculatesAccurately()
        {
            using (var context = new TestContext<SketchDbContext>((options) => { return new SketchDbContext(options); }, seed))
            {

                var guid = Guid.NewGuid().ToString().ToLower();
                var contentCreate = new ContentCreate()
                {
                    Name = guid,
                    GlobalId = guid,
                    Tags = new string[] { "test" }, LogoUrl = "http://logo.url", Type = ContentType.Course };
                var content = await ContentService(context).Add(contentCreate);

                var profiles = new List<ProfileDetail>();

                for (int i = 0; i < 11; i++)
                {
                    guid = Guid.NewGuid().ToString().ToLower();
                    var profileService = ProfileService(context);
                    var profile = await profileService.Add(new ProfileCreate() { Name = guid, GlobalId = guid, LogoUrl = "http://logo.url" });

                    context.Profile = new Profile { Id = profile.Id, GlobalId = profile.GlobalId };

                    profiles.Add(profile);
                }

                var calculator = new ContentRatingCalculator(null) { DbContextOptions = context.DbContextOptions };

                for (int i = 0; i < 11; i++)
                {
                    var profile = profiles[i];
                    context.Profile = new Profile { Id = profile.Id, GlobalId = profile.GlobalId };

                    var rating = Rating.Poor;

                    if (i == 0) rating = Rating.Good;
                    if (i > 5) rating = Rating.Great;

                    await ContentService(context).AddOrUpdateRating(content.Id, rating);
                }

                await calculator.CalculateContentRating(content.GlobalId);

                context.DbContext = new SketchDbContext(context.DbContextOptions);

                var rated = await ContentService(context).GetById(content.Id);

                Assert.Equal(5, rated.Rating.Poor);
                Assert.Equal(1, rated.Rating.Good);
                Assert.Equal(5, rated.Rating.Great);
                Assert.Equal(Rating.Good, rated.Rating.Median);

                double average = (
                        (rated.Rating.Poor * (double)Rating.Poor) +
                        (rated.Rating.Good * (double)Rating.Good) +
                        (rated.Rating.Great * (double)Rating.Great)
                    ) / profiles.Count();

                Assert.Equal(average, rated.Rating.Average);
            }
        }
    }
}
