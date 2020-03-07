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
using System;
using System.Linq;

namespace Foundry.Portal.TestBed
{
    public class TestDataFactory
    {
        TestContext TestContext { get; set; }
        SketchDbContext DbContext { get; set; }

        public TestDataFactory(TestContext context)
        {
            TestContext = context;
            DbContext = context.DbContext;
        }

        public Profile AddProfileAndSetContextProfile(string name, params SystemPermissions[] permissions)
        {
            var profile = AddProfile(name, permissions);
            TestContext.Profile = profile;
            return profile;
        }

        public Profile AddProfile(string name, params SystemPermissions[] permissions)
        {
            permissions = permissions == null || !permissions.Any()
                ? new SystemPermissions[] { SystemPermissions.None }
                : permissions;

            var profile = DbContext.Profiles.SingleOrDefault(p => p.Name == name);

            SystemPermissions systemPermissions = SystemPermissions.None;

            if (permissions.Any())
            {
                foreach (var permission in permissions)
                {
                    systemPermissions |= permission;
                }
            }

            if (profile == null)
            {
                profile = new Profile
                {
                    GlobalId = Guid.NewGuid().ToString(),
                    Name = name,
                    Permissions = systemPermissions
                };

                DbContext.Profiles.Add(profile);
                DbContext.SaveChanges();
            }

            return profile;
        }

        public Profile GetProfile(string name)
        {
            return DbContext.Profiles.SingleOrDefault(p => p.Name == name);
        }

        public ContentDetail AddContent(string name = null)
        {
            name = name ?? Guid.NewGuid().ToString();

            var contentManager = TestContext.GetService<ContentService>();
            return contentManager.Add(new ContentCreate
            {
                Type = ContentType.Course,
                LogoUrl = "http://" + Guid.NewGuid().ToString(),
                Tags = new string[] { "tag" },
                Name = name
            }).Result;
        }

        public DiscussionDetail AddContentReviewDiscussion(int contentId)
        {
            var mgr = TestContext.GetService<DiscussionService>();
            return mgr.Add(new DiscussionCreate
            {
                Type = DiscussionType.ContentReview,
                ContentId = contentId,
                Name = "Content Review"
            }).Result;
        }

        public DiscussionDetailComment AddDiscussionComment(DiscussionDetail discussion)
        {
            var mgr = TestContext.GetService<DiscussionService>();
            return mgr.AddComment(discussion.Id, new CommentCreate
            {
                Text = "Comment " + Guid.NewGuid().ToString()
            }).Result;
        }

        public void VoteOnComment(DiscussionDetailComment comment, int val)
        {
            var mgr = TestContext.GetService<DiscussionService>();
            if (val > 0)
                mgr.UpVote(comment.Id).Wait();

            if (val < 0)
                mgr.DownVote(comment.Id).Wait();
        }
    }
}
