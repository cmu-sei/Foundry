/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Foundry.Groups.ViewModels;
using Stack.Http.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Foundry.Groups.Tests
{
    [Collection("AutoMapper")]
    public class GroupServiceTests : ServiceTests
    {
        [Fact]
        public async Task Add_GroupAddCompletesSuccessfully()
        {
            using (var context = CreateTestContext(GetAdministrator()))
            {
                var groupService = GetGroupService(context);

                var group = await groupService.Add(GetGroupCreate());

                Assert.NotNull(group);
                Assert.Equal(group.Id, group.Key);
            }
        }

        [Fact]
        public async Task Add_GroupAddCreatorIsOwnerCompletesSuccessfully()
        {
            using (var context = CreateTestContext(GetAdministrator()))
            {
                var groupService = GetGroupService(context);

                var group = await groupService.Add(GetGroupCreate());

                Assert.NotNull(group);
                Assert.True(group.Counts.Members == 1);
                Assert.True(group.Roles.Owner);
            }
        }

        [Fact]
        public async Task Update_GroupUpdateCompletesSuccessfully()
        {
            using (var context = CreateTestContext(GetAdministrator()))
            {
                var groupService = GetGroupService(context);

                var group = await groupService.Add(GetGroupCreate());

                Assert.NotNull(group);

                var updated = await groupService.Update(GetGroupUpdate(group.Id, "Updated"));

                Assert.NotNull(updated);
                Assert.True(updated.Counts.Members == 1);
                Assert.True(updated.Roles.Owner);
                Assert.True(updated.Name == "Updated");
                Assert.Equal(updated.Id, updated.Key);
            }
        }

        [Fact]
        public async Task Add_GroupAddAsAdministratorWithParentIdSetSuccessfully()
        {
            using (var context = CreateTestContext(GetAdministrator()))
            {
                var groupService = GetGroupService(context);

                var parent = await groupService.Add(GetGroupCreate("Parent"));

                Assert.NotNull(parent);

                var create = GetGroupCreate();
                create.ParentId = parent.Id;
                var group = await groupService.Add(create);

                Assert.True(group.ParentName == parent.Name);
            }
        }

        [Fact]
        public async Task Add_GroupAddAsUserWithParentIdNotSetSuccessfully()
        {
            using (var context = CreateTestContext(GetUser()))
            {
                var groupService = GetGroupService(context);

                var parent = await groupService.Add(GetGroupCreate("Parent"));

                Assert.NotNull(parent);

                var create = GetGroupCreate();
                create.ParentId = parent.Id;
                var group = await groupService.Add(create);

                Assert.True(group.ParentId == null);
            }
        }

        [Fact]
        public async Task Add_ChildKeysUpdateSuccesfully()
        {
            using (var context = CreateTestContext(GetAdministrator()))
            {
                var groupService = GetGroupService(context);

                var groups = new List<GroupDetail>();
                var keys = new List<string>();

                for (int i = 0; i < 5; i++)
                {
                    var model = GetGroupCreate(string.Format("Group {0}", i));

                    if (i > 0)
                        model.ParentId = groups[i - 1].Id;

                    var group = await groupService.Add(model);

                    keys.Add(group.Id);
                    groups.Add(group);

                    var actual = group.Key;
                    var expected = string.Join("|", keys);

                    Assert.Equal(expected, actual);
                }
            }
        }

        [Fact]
        public async Task Add_ChildMoveUpdatesKeySuccesfully()
        {
            using (var context = CreateTestContext(GetAdministrator()))
            {
                var groupService = GetGroupService(context);

                var groups = new List<GroupDetail>();

                for (int i = 0; i < 5; i++)
                {
                    var create = GetGroupCreate(string.Format("Group {0}", i));

                    if (i > 0)
                        create.ParentId = groups[i - 1].Id;

                    groups.Add(await groupService.Add(create));
                }

                // move [3] to root child
                var root = groups[0];
                var target = groups[3];
                var update = new GroupUpdate()
                {
                    Id = target.Id,
                    Name = target.Name,
                    Description = target.Description,
                    LogoUrl = "http://logo.url",
                    Summary = target.Summary,
                    ParentId = root.Id
                };

                var saved = await groupService.Update(update);

                var expected = string.Format("{0}|{1}", root.Id, saved.Id);
                var actual = saved.Key;
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public async Task Add_MovedGroupChildUpdatesKeySuccesfully()
        {
            using (var context = CreateTestContext(GetAdministrator()))
            {
                var groupService = GetGroupService(context);

                var groups = new List<GroupDetail>();

                for (int i = 0; i < 5; i++)
                {
                    var create = GetGroupCreate(string.Format("Group {0}", i));

                    if (i > 0)
                        create.ParentId = groups[i - 1].Id;

                    groups.Add(await groupService.Add(create));
                }

                // move [3] to root child
                var root = groups[0];
                var target = groups[3];
                var update = new GroupUpdate()
                {
                    Id = target.Id,
                    Name = target.Name,
                    Description = target.Description,
                    LogoUrl = "http://logo.url",
                    Summary = target.Summary,
                    ParentId = root.Id
                };

                var saved = await groupService.Update(update);

                var expected = string.Format("{0}|{1}", root.Id, saved.Id);
                var actual = saved.Key;
                Assert.Equal(expected, actual);

                // check if [4], a child of [3] key is correct
                var child = groups[4];
                var childOfMovedGroup = await groupService.GetById(child.Id);

                expected = string.Format("{0}|{1}|{2}", root.Id, saved.Id, child.Id);
                actual = childOfMovedGroup.Key;

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public async Task Add_CannotMakeAncestorChildOfDescendent()
        {
            using (var context = CreateTestContext(GetAdministrator()))
            {
                var groupService = GetGroupService(context);

                var groups = new List<GroupDetail>();

                for (int i = 0; i < 5; i++)
                {
                    var text = string.Format("Group {0}", i);
                    var create = new GroupCreate() { Name = text, Description = text, LogoUrl = "http://logo.url", Summary = text };

                    if (i > 0)
                        create.ParentId = groups[i - 1].Id;

                    groups.Add(await groupService.Add(create));
                }

                var target = groups.First();
                var update = new GroupUpdate()
                {
                    Id = target.Id,
                    Name = target.Name,
                    Description = target.Description,
                    LogoUrl = "http://logo.url",
                    Summary = target.Summary,
                    ParentId = groups.Last().Id
                };

                Assert.ThrowsAsync<InvalidModelException>(async () => await groupService.Update(update)).Wait();
            }
        }

        [Fact]
        public async Task GetMemberInviteCodeById_InviteCodeGeneratesSuccessfully()
        {
            using (var context = CreateTestContext(GetAdministrator()))
            {
                var groupService = GetGroupService(context);
                var group = await groupService.Add(GetGroupCreate());

                var code = await groupService.GetMemberInviteCodeById(group.Id);
                Assert.NotNull(code);
                Assert.NotEqual(string.Empty, code);

                var getByMemberInviteCode = await groupService.GetByMemberInviteCode(code);

                Assert.NotNull(getByMemberInviteCode);
                Assert.Equal(group.Id, getByMemberInviteCode.Id);

            }
        }

        [Fact]
        public async Task GetMemberInviteCodeById_InviteCodeUpdatesSuccessfully()
        {
            using (var context = CreateTestContext(GetAdministrator()))
            {
                var groupService = GetGroupService(context);
                var group = await groupService.Add(GetGroupCreate());

                var initialCode = await groupService.GetMemberInviteCodeById(group.Id);
                Assert.NotNull(initialCode);
                Assert.NotEqual(string.Empty, initialCode);

                var updatedCode = await groupService.UpdateMemberInviteCodeById(group.Id);
                Assert.NotNull(updatedCode);
                Assert.NotEqual(string.Empty, updatedCode);
                Assert.NotEqual(updatedCode, initialCode);
            }
        }

        [Fact]
        public async Task GetGroupInviteCodeById_InviteCodeGeneratesSuccessfully()
        {
            using (var context = CreateTestContext(GetAdministrator()))
            {
                var groupService = GetGroupService(context);
                var group = await groupService.Add(GetGroupCreate());

                var code = await groupService.GetGroupInviteCodeById(group.Id);
                Assert.NotNull(code);
                Assert.NotEqual(string.Empty, code);

                var getByGroupInviteCode = await groupService.GetByGroupInviteCode(code);

                Assert.NotNull(getByGroupInviteCode);
                Assert.Equal(group.Id, getByGroupInviteCode.Id);

            }
        }

        [Fact]
        public async Task UpdateGroupInviteCodeById_InviteCodeUpdatesSuccessfully()
        {
            using (var context = CreateTestContext(GetAdministrator()))
            {
                var groupService = GetGroupService(context);
                var group = await groupService.Add(GetGroupCreate());

                var initialCode = await groupService.GetGroupInviteCodeById(group.Id);
                Assert.NotNull(initialCode);
                Assert.NotEqual(string.Empty, initialCode);

                var updatedCode = await groupService.UpdateGroupInviteCodeById(group.Id);
                Assert.NotNull(updatedCode);
                Assert.NotEqual(string.Empty, updatedCode);
                Assert.NotEqual(updatedCode, initialCode);
            }
        }
    }
}
