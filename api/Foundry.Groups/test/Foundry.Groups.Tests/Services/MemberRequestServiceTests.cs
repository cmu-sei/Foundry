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
    public class MemberRequestServiceTests : ServiceTests
    {
        [Fact]
        public async Task Add_MemberRequestCreateCompletesSuccessfully()
        {
            using (var context = CreateTestContext(GetAdministrator()))
            {
                var account = await CreateAccount(context);
                var groupService = GetGroupService(context);
                var memberRequestService = GetMemberRequestService(context);

                var group = await groupService.Add(GetGroupCreate());

                var request = await memberRequestService.Add(new MemberRequestCreate { AccountId = account.Id, AccountName = account.Name, GroupId = group.Id });

                Assert.NotNull(request);
            }
        }

        [Fact]
        public async Task Update_MemberRequestUpdateCompletesSuccessfully()
        {
            using (var context = CreateTestContext(GetAdministrator()))
            {
                var account = await CreateAccount(context);
                var groupService = GetGroupService(context);
                var memberRequestService = GetMemberRequestService(context);

                var group = await groupService.Add(GetGroupCreate());

                var create = await memberRequestService.Add(new MemberRequestCreate { AccountId = account.Id, AccountName = account.Name, GroupId = group.Id });

                var update = await memberRequestService.Update(new MemberRequestUpdate { AccountId = account.Id, GroupId = group.Id, Status = MemberRequestStatus.Denied });

                Assert.NotNull(update);
            }
        }

        [Fact]
        public async Task Update_MemberRequestApprovedCreatesMemberCompletesSuccessfully()
        {
            using (var context = CreateTestContext(GetAdministrator()))
            {
                var account = await CreateAccount(context);
                var groupService = GetGroupService(context);
                var memberService = GetMemberService(context);
                var memberRequestService = GetMemberRequestService(context);

                var group = await groupService.Add(GetGroupCreate());

                var create = await memberRequestService.Add(new MemberRequestCreate { AccountId = account.Id, AccountName = account.Name, GroupId = group.Id });

                var update = await memberRequestService.Update(new MemberRequestUpdate { AccountId = account.Id, GroupId = group.Id, Status = MemberRequestStatus.Approved });

                var member = await memberService.Get(group.Id, account.Id);

                Assert.NotNull(member);
            }
        }
    }
}
