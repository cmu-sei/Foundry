/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using IdentityModel.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Foundry.Groups.Data;
using Foundry.Groups.Data.Repositories;
using Foundry.Groups.Options;
using Stack.Http.Options;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Foundry.Groups.Services
{
    public class MigrationService
    {
        IGroupRepository GroupRepository { get; }
        IMemberRepository MemberRepository { get; }
        AuthorizationOptions AuthorizationOptions { get; }
        MarketOptions MarketOptions { get; }
        TokenResponse TokenResponse { get; set; }
        DateTime TokenExpires { get; set; }

        public MigrationService(IGroupRepository groupRepository, IMemberRepository memberRepository, AuthorizationOptions authorizationOptions, MarketOptions marketOptions)
        {
            AuthorizationOptions = authorizationOptions;
            MarketOptions = marketOptions;
            GroupRepository = groupRepository;
            MemberRepository = memberRepository;
        }

        /// <summary>
        /// migrate 
        /// </summary>
        /// <param name="dataFilter"></param>
        /// <returns></returns>
        public async Task Migrate(LegacyDataFilter dataFilter)
        {
            var legacyGroupPagedResult = await GetGroupsAsync(dataFilter);

            var db = GroupRepository.DbContext;

            foreach (var legacyGroup in legacyGroupPagedResult.Results)
            {
                var isNewGroup = false;
                var group = await db.Groups.SingleOrDefaultAsync(g => g.Id.ToLower() == legacyGroup.GlobalId.ToLower());

                if (group == null)
                {
                    isNewGroup = true;
                    group = new Group
                    {
                        Id = legacyGroup.GlobalId.ToLower()
                    };
                }

                group.Name = legacyGroup.Name;
                group.Created = legacyGroup.Created;
                group.Description = legacyGroup.Description;
                group.LogoUrl = legacyGroup.LogoUrl;
                group.Summary = legacyGroup.Summary;
                group.Updated = legacyGroup.Updated;
                group.Key = group.Id;

                if (isNewGroup) await db.Groups.AddAsync(group);

                await db.SaveChangesAsync();

                var legacyMemberPagedResult = await GetMembersAsync(legacyGroup.Id);

                foreach (var legacyMember in legacyMemberPagedResult.Results)
                {
                    var isNewAccount = false;
                    var account = await db.Accounts.SingleOrDefaultAsync(a => a.Id.ToLower() == legacyMember.ProfileGlobalId.ToLower());

                    if (account == null)
                    {
                        isNewAccount = true;
                        account = new Account
                        {
                            Id = legacyMember.ProfileGlobalId.ToLower()
                        };
                    }

                    account.Name = legacyMember.ProfileName;

                    if (isNewAccount) await db.Accounts.AddAsync(account);

                    await db.SaveChangesAsync();

                    var isNewMember = false;
                    var member = await db.Members.SingleOrDefaultAsync(m => m.GroupId == group.Id && m.AccountId == account.Id);

                    if (member == null)
                    {
                        isNewMember = true;
                        member = new Member
                        {
                            GroupId = group.Id,
                            AccountId = account.Id
                        };
                    }

                    member.IsOwner = legacyMember.IsOwner;
                    if (!member.IsOwner)
                    {
                        member.IsManager = legacyMember.HasManageMembers || legacyMember.HasEditGroup;
                    }

                    if (isNewMember) await db.Members.AddAsync(member);

                    await db.SaveChangesAsync();
                }
            }
        }

        async Task<TokenResponse> GetTokenAsync(bool always = false)
        {
            if (TokenResponse == null || TokenExpires > DateTime.UtcNow || always)
            {
                var discoveryClient = await DiscoveryClient.GetAsync(AuthorizationOptions.Authority);

                if (discoveryClient.IsError)
                    throw new SecurityTokenException(discoveryClient.Error);

                var client = new TokenClient(discoveryClient.TokenEndpoint, AuthorizationOptions.ClientId, AuthorizationOptions.ClientSecret);
                var response = await client.RequestClientCredentialsAsync(AuthorizationOptions.AuthorizationScope);

                if (response.IsError)
                    throw new SecurityTokenException(response.Error);

                TokenResponse = response;
                TokenExpires = DateTime.UtcNow.AddSeconds(response.ExpiresIn);
            }

            return TokenResponse;
        }

        public async Task<LegacyPagedResult<LegacyGroupSummary>> GetGroupsAsync(LegacyDataFilter dataFilter)
        {
            using (var client = new HttpClient { BaseAddress = new Uri(MarketOptions.Url) })
            {
                client.SetBearerToken((await GetTokenAsync()).AccessToken);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                dataFilter = dataFilter ?? new LegacyDataFilter();

                var response = await client.GetAsync("api/groups?skip=" + dataFilter.Skip + "&take=" + dataFilter.Take);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        var content = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<LegacyPagedResult<LegacyGroupSummary>>(content);
                    default:
                        throw new Exception(await response.RequestMessage.Content.ReadAsStringAsync());
                }
            }
        }

        public async Task<LegacyPagedResult<LegacyMemberDetail>> GetMembersAsync(int id)
        {
            using (var client = new HttpClient { BaseAddress = new Uri(MarketOptions.Url) })
            {
                client.SetBearerToken((await GetTokenAsync()).AccessToken);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync("api/group/" + id + "/members");

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        var content = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<LegacyPagedResult<LegacyMemberDetail>>(content);
                    default:
                        throw new Exception(await response.RequestMessage.Content.ReadAsStringAsync());
                }
            }
        }

        public class LegacyPagedResult<TModel>
        where TModel : class
        {
            public LegacyDataFilter DataFilter { get; set; }
            public int Total { get; set; }
            public TModel[] Results { get; set; }
        }

        public class LegacyDataFilter
        {
            public string Term { get; set; }
            public int Skip { get; set; }
            public int Take { get; set; }
            public string Filter { get; set; }
            public string Sort { get; set; }
        }

        public class LegacyGroupSummary
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Slug { get; set; } = string.Empty;
            public DateTime Created { get; set; }
            public string CreatedBy { get; set; }
            public DateTime? Updated { get; set; }
            public string UpdatedBy { get; set; }
            public DateTime? Imported { get; set; }
            public string ImportedBy { get; set; }
            public int MemberCount { get; set; }
            public string Description { get; set; }
            public string Summary { get; set; }
            public string GlobalId { get; set; }
            public string LogoUrl { get; set; }
            public string ThumbnailUrl { get; set; }
        }

        public class LegacyMemberDetail
        {
            public int Id { get; set; }
            public LegacyGroupPermission Permissions { get; set; }
            public int GroupId { get; set; }
            public string GroupGlobalId { get; set; }
            public string GroupName { get; set; }
            public int ProfileId { get; set; }
            public string ProfileName { get; set; }
            public string ProfileGlobalId { get; set; }
            public bool NeedsApproval { get; set; }
            public bool CanManage { get; set; }
            public int MembershipCount { get; set; }
            public bool HasRunReports { get; set; }
            public bool HasEditGroup { get; set; }
            public bool HasManageMembers { get; set; }
            public bool IsOwner { get; set; }
        }

        [Flags]
        public enum LegacyGroupPermission
        {
            None = 0x00,
            Member = 0x01,
            Manage_Members = 0x02,
            Run_Reports = 0x04,
            Edit = 0x08,
            Owner = Int32.MaxValue
        }
    }
}

