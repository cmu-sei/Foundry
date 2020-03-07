/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using Foundry.Portal.Api.Controllers;
//using Foundry.Portal.Data;
//using Foundry.Portal.Data.Entities;
//using Foundry.Portal.Services;
//using Foundry.Portal.TestBed;
//using Foundry.Portal.ViewModels;
//using Stack.Patterns.Service.Models;
//using System.Threading.Tasks;
//using Xunit;

//namespace Foundry.Portal.Api.Tests.Controllers
//{
//    [Collection("AutoMapper")]
//    public class ContentControllerTests : ControllerTests
//    {
//        ContentController GetContentController(TestContext context)
//        {
//            var contentService = context.GetService<ContentService>();
//            var discussionService = context.GetService<DiscussionService>();
//            var tagService = context.GetService<TagService>();

//            return new ContentController(new CoreOptions(), new LoggerFactory(), contentService, discussionService, tagService);
//        }

//        ChannelController GetChannelController(TestContext context)
//        {
//            return new ChannelController(new CoreOptions(), new LoggerFactory(), context.GetService<ChannelService>(), context.GetService<ProfileService>());
//        }

//        private int CreateBasicChannel(TestContext context)
//        {
//            var channel = context.TestDataFactory.AddChannelWithContent();
//            return channel.Id;
//        }

//        [Fact]
//        public async Task Add_ReturnsCreated()
//        {
//            using (var context = CreateTestContext())
//            {
//                var channelId = CreateBasicChannel(context);
//                var contentController = GetContentController(context);

//                var model = new ContentCreate()
//                {
//                    Name = "ReturnsCreated",
//                    ChannelId = channelId,
//                    Type = ContentType.Course,
//                    LogoUrl = "http://content.url/logo",
//                    Tags = new string[] { "tag" }
//                };

//                Assert.IsType<CreatedResult>(await contentController.Add(model));
//            }
//        }

//        [Fact]
//        public async Task Delete_ReturnsDeleted()
//        {
//            using (var context = CreateTestContext())
//            {
//                var contentController = GetContentController(context);
//                var channelController = GetChannelController(context);
//                var channel = context.TestDataFactory.AddChannelWithContent();
//                var result = await contentController.GetAllContentByChannel(channel.Id, new ContentDataFilter());
//                var contentResults = ((PagedResult<Content, ContentSummary>)((OkObjectResult)result).Value).Results;
//                int count = contentResults.Length;

//                Assert.IsType<OkResult>(await contentController.Delete(contentResults[0].Id));

//                result = await contentController.GetAllContentByChannel(channel.Id, new ContentDataFilter());
//                contentResults = ((PagedResult<Content, ContentSummary>)((OkObjectResult)result).Value).Results;
//                int newCount = contentResults.Length;
//                Assert.Equal(count.ToString(), (newCount + 1).ToString());
//            }
//        }
//    }
//}

