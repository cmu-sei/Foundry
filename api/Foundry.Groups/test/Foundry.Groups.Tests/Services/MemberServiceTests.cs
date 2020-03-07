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
using Foundry.Groups.Repositories;
using Foundry.Groups.Security;
using Foundry.Groups.Services;
using Foundry.Groups.ViewModels;
using Stack.Http.Exceptions;
using System.Threading.Tasks;
using Xunit;

namespace Foundry.Groups.Tests
{
    [Collection("AutoMapper")]
    public class MemberServiceTests : ServiceTests
    {
        [Fact]
        public async Task Add_AdministratorMemberCreatedSuccessfully()
        {
            using (var context = CreateTestContext(GetAdministrator()))
            {
                var account = await CreateAccount(context);
                var groupService = GetGroupService(context);

                var group = await groupService.Add(GetGroupCreate());
                Assert.NotNull(group);

                var memberService = GetMemberService(context);

                var member = await memberService.Add(new MemberCreate { AccountId = account.Id, AccountName = account.Name, GroupId = group.Id });
                Assert.NotNull(member);
            }
        }

        [Fact]
        public async Task Add_NonAdministratorMemberCreatedFails()
        {
            using (var context = CreateTestContext(GetUser()))
            {
                var account = await CreateAccount(context);
                var groupService = GetGroupService(context);

                var group = await groupService.Add(GetGroupCreate());
                Assert.NotNull(group);

                var memberService = GetMemberService(context);

                Assert.ThrowsAsync<EntityPermissionException>(async () =>
                    await memberService.Add(new MemberCreate { AccountId = account.Id, AccountName = account.Name, GroupId = group.Id })
                ).Wait();
            }
        }

        [Fact]
        public async Task Update_ChangingLastOwnerFails()
        {
            var administrator = GetAdministrator();

            using (var context = CreateTestContext(administrator))
            {
                var groupService = GetGroupService(context);

                var group = await groupService.Add(GetGroupCreate());
                var memberService = GetMemberService(context);

                Assert.ThrowsAsync<InvalidModelException>(async () =>
                    await memberService.Update(new MemberUpdate { AccountId = administrator.Id, GroupId = group.Id, IsOwner = false })
                ).Wait();
            }
        }

        [Fact]
        public async Task Delete_DeletingLastOwnerFails()
        {
            var administrator = GetAdministrator();

            using (var context = CreateTestContext(administrator))
            {
                var groupService = GetGroupService(context);

                var group = await groupService.Add(GetGroupCreate());
                var memberService = GetMemberService(context);

                Assert.ThrowsAsync<InvalidModelException>(async () =>
                    await memberService.Delete(new MemberDelete { AccountId = administrator.Id, GroupId = group.Id })
                ).Wait();
            }
        }
    }
}
