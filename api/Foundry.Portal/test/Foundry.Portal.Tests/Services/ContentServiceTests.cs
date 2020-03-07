/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.EntityFrameworkCore;
using Foundry.Portal.Data;
using Foundry.Portal.Data.Entities;
using Foundry.Portal.Services;
using Foundry.Portal.ViewModels;
using Stack.Http.Exceptions;
using Stack.Patterns.Service.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Foundry.Portal.Tests.Services
{
    [Collection("AutoMapper")]
    public class ContentServiceTests : ServiceTests
    {

        /// <summary>
        /// test star and end date content data filter
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetAll_DataFilterStartAndEndDate()
        {
            var contents = new PagedResult<Content, ContentSummary>();
            ContentDetail contentOne, contentTwo, contentThree;

            string tag = "test";
            using (var context = CreateTestContext())
            {
                var contentService = context.GetService<ContentService>();

                contentOne = contentService.Add(new ContentCreate
                {
                    Name = "Event One",
                    Type = ContentType.Event,
                    Description = "event one",
                    LogoUrl = "http://logo.com/url",
                    Tags = new string[] { tag },
                    Url = "http://stepfwd.cert.org",
                    Settings = "",
                    Order = 1,
                    StartDate = "1/1/2018",
                    StartTime = "12:00PM"
                }).Result;

                contentTwo = contentService.Add(new ContentCreate
                {
                    Name = "Event Two",
                    Type = ContentType.Event,
                    Description = "event two",
                    LogoUrl = "http://logo.com/url",
                    Tags = new string[] { tag },
                    Url = "http://stepfwd.cert.org",
                    Settings = "",
                    Order = 2,
                    StartDate = "2/1/2018",
                    StartTime = "12:00PM"
                }).Result;

                contentThree = contentService.Add(new ContentCreate
                {
                    Name = "Event Three",
                    Type = ContentType.Event,
                    Description = "event three",
                    LogoUrl = "http://logo.com/url",
                    Tags = new string[] { tag },
                    Url = "http://stepfwd.cert.org",
                    Settings = "",
                    Order = 2,
                    StartDate = "3/1/2018",
                    StartTime = "12:00PM"
                }).Result;
            }

            using (var context = CreateTestContext())
            {
                contents = await context.GetService<ContentService>().GetAll(new ContentDataFilter() { Filter = "startdate=1/1/2018|enddate=1/31/2018" });
            }

            Assert.True(contents.Total == 1, contents.Total + " results in January");

            using (var context = CreateTestContext())
            {
                contents = await context.GetService<ContentService>().GetAll(new ContentDataFilter() { Filter = "startdate=2/1/2018|enddate=2/28/2018" });
            }

            Assert.True(contents.Total == 1, contents.Total + " results in February");

            using (var context = CreateTestContext())
            {
                contents = await context.GetService<ContentService>().GetAll(new ContentDataFilter() { Filter = "startdate=3/1/2018|enddate=3/31/2018" });
            }

            Assert.True(contents.Total == 1, contents.Total + " results in March");

            using (var context = CreateTestContext())
            {
                contents = await context.GetService<ContentService>().GetAll(new ContentDataFilter() { Filter = "startdate=1/1/2018|enddate=2/28/2018" });
            }

            Assert.True(contents.Total == 2, contents.Total + " results in January and February");

            using (var context = CreateTestContext())
            {
                contents = await context.GetService<ContentService>().GetAll(new ContentDataFilter() { Filter = "startdate=2/1/2018|enddate=3/31/2018" });
            }

            Assert.True(contents.Total == 2, contents.Total + " results in February and March");

            using (var context = CreateTestContext())
            {
                contents = await context.GetService<ContentService>().GetAll(new ContentDataFilter() { Filter = "startdate=1/1/2018|enddate=3/31/2018" });
            }

            Assert.True(contents.Total == 3, contents.Total + " results in January, February, and March");

            using (var context = CreateTestContext())
            {
                contents = await context.GetService<ContentService>().GetAll(new ContentDataFilter() { Filter = "startdate=4/1/2018|enddate=4/30/2018" });
            }

            Assert.True(contents.Total == 0, contents.Total + " results in April");
        }

        [Fact]
        public void Add_ContentIsAddedSuccessfully()
        {
            ContentDetail target = null;
            Content match = null;

            using (var context = CreateTestContext())
            {
                var contentService = context.GetService<ContentService>();

                target = contentService.Add(new ContentCreate
                {
                    Name = "A Good Video",
                    Type = ContentType.Video,
                    LogoUrl = "http://logo.com/url",
                    Description = "a good video",
                    Tags = new string[] { "test video" },
                    Url = "http://stepfwd.cert.org",
                    Settings = "settings",
                    Order = 1
                }).Result;
            }

            using (var context = CreateTestContext())
            {
                match = context.DbContext.Contents.Where(c => c.Id == target.Id).First();
            }

            Assert.NotNull(target);
            Assert.NotNull(match);
            Assert.Equal(target.Id, match.Id);
        }

        [Fact]
        public void Update_UpdateSavesSuccessfully()
        {
            ContentDetail match = null;
            ContentUpdate target = null;
            string name = Guid.NewGuid().ToString();

            using (var context = CreateTestContext())
            {
                var contentService = context.GetService<ContentService>();

                var content = context.TestDataFactory.AddContent("test");

                target = contentService.Map<ContentUpdate>(content);
                target.Name = name;
                contentService.Update(target).Wait();
            }

            using (var context = CreateTestContext())
            {
                var service = context.GetService<ContentService>();
                match = service.GetById(target.Id).Result;
            }

            Assert.NotNull(target);
            Assert.NotNull(match);
            Assert.Equal(match.Name, name);
        }

        /// <summary>
        /// test that content can be added and is deleted successfully
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Add_ContentIsAddedAndDeletedSuccessfully()
        {
            ContentDetail target = null;
            Content match = null;

            using (var context = CreateTestContext())
            {
                var contentService = context.GetService<ContentService>();

                var contentCreate = new ContentCreate
                {
                    Name = "Marked for deletion",
                    Type = ContentType.Lab,
                    Description = "marked for deletion",
                    Tags = new string[] { "delete me" },
                    LogoUrl = "http://logo.com/url",
                    Url = "http://stepfwd.cert.org",
                    Settings = "settings",
                    Order = 1
                };

                target = await contentService.Add(contentCreate);
            }

            using (var context = CreateTestContext())
            {
                match = context.DbContext.Contents.Where(c => c.Id == target.Id).First();
            }

            using (var context = CreateTestContext())
            {
                var contentService = context.GetService<ContentService>();

                await contentService.DeleteById(target.Id);

                await Assert.ThrowsAsync<EntityNotFoundException>(() => contentService.GetById(target.Id));
            }
        }

        [Fact]
        public async void SetRating_OneHundred()
        {
            using (var test = CreateTestContext())
            {
                var target = Guid.NewGuid().ToString();

                var content = test.TestDataFactory.AddContent(target);

                var contentId = content.Id;

                Rating rating = Rating.Poor;

                for (int i = 1; i <= 100; i++)
                {
                    test.TestDataFactory.AddProfileAndSetContextProfile(i + "@cert.org");
                    await test.GetService<ContentService>().AddOrUpdateRating(contentId, rating);
                    if (rating == Rating.Great)
                    {
                        rating = Rating.Poor;
                    }
                    else
                    {
                        rating++;
                    }
                }

                content = await test.GetService<ContentService>().GetById(contentId);

                Assert.Equal(25, content.Rating.Great);
                Assert.Equal(25, content.Rating.Good);
                Assert.Equal(25, content.Rating.Fair);
                Assert.Equal(25, content.Rating.Poor);
            }
        }

        [Fact]
        public async void SetRating_SucceedsAndSetsViewModel()
        {
            int contentId = 0;
            var rating = Rating.Great;

            using (var test = CreateTestContext())
            {
                var content = test.TestDataFactory.AddContent(Guid.NewGuid().ToString());

                contentId = content.Id;

                var contentService = test.GetService<ContentService>();
                await contentService.AddOrUpdateRating(contentId, rating);
            }

            using (var test = CreateTestContext())
            {
                var contentService = test.GetService<ContentService>();
                var contentInstance = await test.DbContext.ProfileContents
                    .SingleOrDefaultAsync(cis => cis.ProfileId == test.Profile.Id && cis.ContentId == contentId);

                Assert.NotNull(contentInstance);
                Assert.Equal(contentInstance.Rating, rating);

                var content = await contentService.GetById(contentId);

                Assert.Equal(rating, content.UserRating);
            }
        }

        [Fact]
        public async void SetDifficulty_SucceedsAndSetsViewModel()
        {
            int contentId = 0;
            var difficulty = Difficulty.Advanced;

            using (var test = CreateTestContext())
            {
                var content = test.TestDataFactory.AddContent(Guid.NewGuid().ToString());

                contentId = content.Id;

                var contentService = test.GetService<ContentService>();
                await contentService.AddOrUpdateDifficulty(contentId, difficulty);
            }

            using (var test = CreateTestContext())
            {
                var contentService = test.GetService<ContentService>();
                var contentInstance = test.DbContext.ProfileContents
                    .FirstOrDefault(cis => cis.ProfileId == test.Profile.Id && cis.ContentId == contentId);

                Assert.NotNull(contentInstance);
                Assert.Equal(contentInstance.Difficulty, difficulty);

                var content = await contentService.GetById(contentId);

                Assert.Equal(difficulty, content.UserDifficulty);
            }
        }
    }
}
