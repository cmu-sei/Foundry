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
using Foundry.Portal.Data.Entities;
using Foundry.Portal.Services;
using Foundry.Portal.ViewModels;
using Stack.Http.Exceptions;
using Stack.Patterns.Service.Models;
using System.Linq;
using Xunit;

namespace Foundry.Portal.Tests.Services
{
    [Collection("AutoMapper")]
    public class DiscussionServiceTests : ServiceTests
    {
        public DiscussionServiceTests()
        : base() { }

        [Fact]
        public void AddContentReviewDiscussion_Succeeds()
        {
            using (var init = CreateTestContext())
            {
                Profile jack = init.TestDataFactory.AddProfileAndSetContextProfile("jack@this.test");

                var content = init.TestDataFactory.AddContent("Test");
                var discussion = init.TestDataFactory.AddContentReviewDiscussion(content.Id);
            }

            using (var test = CreateTestContext())
            {
                Assert.True(test.DbContext.Discussions.Count() > 0);
            }
        }

        [Fact]
        public void AddCommentAddVote_Succeeds()
        {
            int discussionId = 0;
            int commentId = 0;
            using (var init = CreateTestContext())
            {
                Profile jack = init.TestDataFactory.AddProfileAndSetContextProfile("jack@this.test");

                var content = init.TestDataFactory.AddContent("test");
                var discussion = init.TestDataFactory.AddContentReviewDiscussion(content.Id);
                discussionId = discussion.Id;
                DiscussionDetailComment comment = init.TestDataFactory.AddDiscussionComment(discussion);
                init.TestDataFactory.AddDiscussionComment(discussion);

                commentId = comment.Id;

                Profile jill = init.TestDataFactory.AddProfileAndSetContextProfile("jill@this.test");
                init.TestDataFactory.AddDiscussionComment(discussion);
                init.TestDataFactory.VoteOnComment(comment, 1);

                init.TestDataFactory.AddProfileAndSetContextProfile("joe@this.test");
                init.TestDataFactory.VoteOnComment(comment, 1);
            }

            using (var test = CreateTestContext())
            {
                test.TestDataFactory.AddProfileAndSetContextProfile("jack@this.test");

                var search = new CommentDataFilter();

                var result = new PagedResult<Comment, DiscussionDetailComment>();
                var mgr = test.GetService<DiscussionService>();
                result = mgr.GetAllCommentsByDiscussionId(discussionId, search).Result;

                var comment = result.Results.Single(c => c.Id == commentId);

                Assert.True(result.Total == 3, "Total count is " + result.Total);
                Assert.True(comment.Votes == 2, "First comment has " + comment.Votes + " votes");
            }
        }

        [Fact]
        public void Update_PreventDiscussionUpdateByNonOwners()
        {
            using (var test = CreateTestContext())
            {
                Profile jack = test.TestDataFactory.AddProfileAndSetContextProfile("jack@this.test");

                var content = test.TestDataFactory.AddContent("Test");
                var discussion = test.TestDataFactory.AddContentReviewDiscussion(content.Id);

                var mgr = test.GetService<DiscussionService>();
                var result = mgr.Update(new DiscussionUpdate()
                {
                    Id = discussion.Id,
                    Name = jack.Name,
                    ContentId = discussion.ContentId,
                    Description = discussion.Description,
                    GlobalId = discussion.GlobalId,
                    Status = discussion.Status,
                    Type = discussion.Type
                 }).Result;

                Assert.True(result.Name == jack.Name);

                Profile jill = test.TestDataFactory.AddProfileAndSetContextProfile("jill@this.test");

                mgr = test.GetService<DiscussionService>();
                Assert.ThrowsAsync<EntityPermissionException>(async() => await mgr.Update(new DiscussionUpdate()
                {
                    Id = discussion.Id,
                    Name = jill.Name,
                    ContentId = discussion.ContentId,
                    Description = discussion.Description,
                    GlobalId = discussion.GlobalId,
                    Status = discussion.Status,
                    Type = discussion.Type
                })).Wait();

            }
        }

    }
}
