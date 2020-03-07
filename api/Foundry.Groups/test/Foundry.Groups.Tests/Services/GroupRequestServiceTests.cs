/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Foundry.Groups.Data;
using Foundry.Groups.ViewModels;
using System.Threading.Tasks;
using Xunit;

namespace Foundry.Groups.Tests
{
    [Collection("AutoMapper")]
    public class GroupRequestServiceTests : ServiceTests
    {
        [Fact]
        public async Task Add_GroupRequestCreateCompletesSuccessfully()
        {
            using (var context = CreateTestContext(GetAdministrator()))
            {
                var account = await CreateAccount(context);
                var groupService = GetGroupService(context);
                var groupRequestService = GetGroupRequestService(context);

                var parent = await groupService.Add(GetGroupCreate());
                var child = await groupService.Add(GetGroupCreate());

                var request = await groupRequestService.Add(new GroupRequestCreate { ParentGroupId = parent.Id, ChildGroupId = child.Id });

                Assert.NotNull(request);
            }
        }

        [Fact]
        public async Task Update_GroupRequestUpdateCompletesSuccessfully()
        {
            using (var context = CreateTestContext(GetAdministrator()))
            {
                var account = await CreateAccount(context);
                var groupService = GetGroupService(context);
                var groupRequestService = GetGroupRequestService(context);

                var parent = await groupService.Add(GetGroupCreate());
                var child = await groupService.Add(GetGroupCreate());
                var request = await groupRequestService.Add(new GroupRequestCreate { ParentGroupId = parent.Id, ChildGroupId = child.Id });

                var update = await groupRequestService.Update(new GroupRequestUpdate { ParentGroupId = parent.Id, ChildGroupId = child.Id, Status = GroupRequestStatus.Denied });

                Assert.NotNull(update);
            }
        }

        [Fact]
        public async Task Update_GroupRequestApprovedSetsParentSuccessfully()
        {
            using (var context = CreateTestContext(GetAdministrator()))
            {
                var account = await CreateAccount(context);
                var groupService = GetGroupService(context);
                var groupRequestService = GetGroupRequestService(context);

                var parent = await groupService.Add(GetGroupCreate());
                var child = await groupService.Add(GetGroupCreate());
                var request = await groupRequestService.Add(new GroupRequestCreate { ParentGroupId = parent.Id, ChildGroupId = child.Id });
                var update = await groupRequestService.Update(new GroupRequestUpdate { ParentGroupId = parent.Id, ChildGroupId = child.Id, Status = GroupRequestStatus.Approved });

                var saved = await groupService.GetById(child.Id);

                Assert.Equal(parent.Id, saved.ParentId);
            }
        }
    }
}
