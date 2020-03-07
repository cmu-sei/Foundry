/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stack.DomainEvents;
using Foundry.Groups.Data;
using Foundry.Groups.Data.Repositories;
using Foundry.Groups.ViewModels;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Patterns.Service.Models;
using Stack.Validation.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Groups.Services
{
    /// <summary>
    /// group service
    /// </summary>
    public class GroupService : DispatchService<IGroupRepository, Group>
    {
        /// <summary>
        /// create an instance of group service
        /// </summary>
        /// <param name="domainEventDispatcher"></param>
        /// <param name="identityResolver"></param>
        /// <param name="groupRepository"></param>
        /// <param name="mapper"></param>
        /// <param name="validationHandler"></param>
        /// <param name="logger"></param>
        public GroupService(IDomainEventDispatcher domainEventDispatcher, IStackIdentityResolver identityResolver, IGroupRepository groupRepository, IMapper mapper, IValidationHandler validationHandler, ILogger<GroupService> logger)
            : base(domainEventDispatcher, identityResolver, groupRepository, mapper, validationHandler, logger) { }

        /// <summary>
        /// get all groups
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<PagedResult<Group, GroupSummary>> GetAll(GroupDataFilter search = null)
        {
            return await PagedResult<Group, GroupSummary>(Repository.GetAll(), search);
        }

        /// <summary>
        /// get groups by account id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<PagedResult<Group, GroupSummary>> GetAllGroupsByAccountId(string id, GroupDataFilter search = null)
        {
             var query = Repository.GetAll().Where(g => g.Members.Any(m => m.AccountId == id));

            return await PagedResult<Group, GroupSummary>(query, search);
        }

        /// <summary>
        /// get all by parent id
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<PagedResult<Group, GroupSummary>> GetAllByParentId(string parentId, GroupDataFilter search = null)
        {
            var query = Repository.GetAll().Where(g => g.ParentId == parentId);

            return await PagedResult<Group, GroupSummary>(query, search);
        }

        /// <summary>
        /// get group by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<GroupDetail> GetById(string id)
        {
            return Map<GroupDetail>(await Repository.GetById(id));
        }

        /// <summary>
        /// get tree
        /// </summary>
        /// <returns></returns>
        public async Task<List<TreeGroupSummary>> GetTree()
        {
            var tree = new List<TreeGroupSummary>();

            var groups = await Repository.GetAll()
                .OrderBy(g => g.Name).ToListAsync();

            tree.AddRange(MapChildren(null, groups));

            return tree;
        }

        /// <summary>
        /// recursive map children
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="all"></param>
        /// <returns></returns>
        List<TreeGroupSummary> MapChildren(string parentId, List<Group> all)
        {
            var tree = new List<TreeGroupSummary>();

            var children = all.Where(g => g.ParentId == parentId);

            foreach (var child in children)
            {
                var model = new TreeGroupSummary { Id = child.Id, Name = child.Name, Slug = child.Slug, Key = child.Key };
                model.Children = MapChildren(child.Id, all);
                tree.Add(model);
            }

            return tree;
        }

        /// <summary>
        /// add group
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<GroupDetail> Add(GroupCreate model)
        {
            await ValidationHandler.ValidateRulesFor(model);

            var group = new Group
            {
                Description = model.Description,
                LogoUrl = model.LogoUrl,
                Name = model.Name,
                Summary = model.Summary
            };

            if (IsAdministrator && !string.IsNullOrEmpty(model.Id))
            {
                group.Id = model.Id;
            }

            if (IsAdministrator && group.ParentId != model.ParentId)
            {
                group.ParentId = model.ParentId;
            }

            var saved = await Repository.Add(group);

            await UpdateKey(DbContext, saved);

            saved.Members.Add(new Member
            {
                AccountId = Identity.Id,
                GroupId = group.Id,
                IsOwner = true
            });

            await Repository.DbContext.SaveChangesAsync();

            await DispatchAsync(new DomainEvent(saved, saved.Id, saved.Name, "groupadd"));

            return await GetById(saved.Id);
        }

        /// <summary>
        /// update group
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<GroupDetail> Update(GroupUpdate model)
        {
            await ValidationHandler.ValidateRulesFor(model);

            var group = await Repository.GetById(model.Id);

            group.LogoUrl = model.LogoUrl;
            group.Name = model.Name;
            group.Summary = model.Summary;
            group.Description = model.Description;

            if (IsAdministrator && group.ParentId != model.ParentId)
            {
                group.ParentId = model.ParentId;
            }

            var saved = await Repository.Update(group);

            await UpdateKey(DbContext, saved);

            await DispatchAsync(new DomainEvent(group, group.Id, group.Name, "groupupdate"));

            return await GetById(saved.Id);
        }

        /// <summary>
        /// delete by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteById(string id)
        {
            var group = await Repository.GetById(id);

            var parentId = group.ParentId;

            var children = await Repository.GetAll()
                .Where(g => g.Key.Contains(id) && g.Id != id)
                .ToListAsync();

            foreach (var child in children)
            {
                // remove parent id from child key
                child.Key = child.Key.Replace(id + "|", "");

                if (child.ParentId == id)
                {
                    // if group is direct child of parent, change parentId to parent's parentId
                    child.ParentId = parentId;
                }
            }

            // delete member and group requests
            foreach (MemberRequest memberRequest in group.MemberRequests)
            {
                Repository.DbContext.MemberRequests.Remove(memberRequest);
            }

            var groupRequests = Repository.DbContext.GroupRequests.Where(g => g.ParentGroupId == id || g.ChildGroupId == id);

            foreach (GroupRequest groupRequest in groupRequests)
            {
                Repository.DbContext.GroupRequests.Remove(groupRequest);
            }

            var cloneForNotification = group.Clone();

            await Repository.DbContext.SaveChangesAsync();

            await Repository.Delete(group);

            await DispatchAsync(new DomainEvent(cloneForNotification, cloneForNotification.Id, cloneForNotification.Name, "groupdelete"));

            return true;
        }

        /// <summary>
        /// update key
        /// </summary>
        /// <param name="db"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        internal static async Task UpdateKey(GroupsDbContext db, Group group)
        {
            var key = group.Id;

            if (!string.IsNullOrWhiteSpace(group.ParentId))
            {
                // if group has a parent the parent's key should prefix the group key
                var parent = await db.Groups.SingleOrDefaultAsync(g => g.Id == group.ParentId);
                key = string.Format("{0}|{1}", parent.Key, group.Id);
            }

            group.Key = key;

            var children = await db.Groups
               .Where(g => g.Key.Contains(group.Id) && g.Id != group.Id)
               .ToListAsync();

            SetDescendentKeys(group, children);

            await db.SaveChangesAsync();
        }

        /// <summary>
        /// accept group invite and update key
        /// </summary>
        /// <param name="code"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public async Task<GroupDetail> AcceptGroupInvite(string code, string groupId)
        {
            var model = new GroupInviteAccept
            {
                Code = code,
                GroupId = groupId
            };

            await ValidationHandler.ValidateRulesFor(model);

            var parent = await DbContext.Groups.SingleOrDefaultAsync(g => g.GroupInviteCode == model.Code);
            var child = await Repository.GetById(model.GroupId);

            child.ParentId = parent.Id;

            var saved = await Repository.Update(child);
            await UpdateKey(DbContext, saved);

            await DispatchAsync(new DomainEvent(child, child.Id, child.Name, "groupupdate"));

            return await GetById(saved.Id);
        }

        /// <summary>
        /// accept member invite
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<MemberDetail> AcceptMemberInvite(string code)
        {
            var model = new MemberInviteAccept
            {
                AccountId = Identity.Id,
                Code = code
            };

            await ValidationHandler.ValidateRulesFor(model);

            var account = await DbContext.Accounts.SingleOrDefaultAsync(g => g.Id == model.AccountId);
            var parent = await DbContext.Groups.SingleOrDefaultAsync(g => g.MemberInviteCode == model.Code);

            var create = new MemberCreate
            {
                AccountId = account.Id,
                AccountName = account.Name,
                GroupId = parent.Id
            };

            var member = MemberService.Add(create, DbContext);

            await DispatchAsync(new DomainEvent(member, member.Group.Id, member.Group.Name, "memberadd"));

            return Map<MemberDetail>(member);
        }

        /// <summary>
        /// set ancestor keys
        /// </summary>
        /// <param name="group"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        static void SetDescendentKeys(Group group, IEnumerable<Group> children)
        {
            if (children.Any())
            {
                // get all parent keys
                var parentKeys = group.Key.Split("|").ToList();

                foreach (var child in children)
                {
                    // get all child keys excluding ancestor keys
                    var keys = child.Key.Split("|").ToList();
                    keys = keys.Skip(keys.IndexOf(child.Id)).ToList();

                    // insert ancestor keys
                    keys.InsertRange(0, parentKeys);

                    child.Key = string.Join("|", keys);
                }
            }
        }

        /// <summary>
        /// get group by invite code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<GroupDetail> GetByMemberInviteCode(string code)
        {
            var group = await DbContext.Groups.SingleOrDefaultAsync(g => g.MemberInviteCode == code);

            if (group == null)
                throw new EntityNotFoundException("Invalid code");

            return await GetById(group.Id);
        }

        /// <summary>
        /// get invite code for group by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<string> GetMemberInviteCodeById(string id)
        {
            var group = await DbContext.Groups.Include(g => g.Members).SingleOrDefaultAsync(g => g.Id == id);

            if (group.Members.Any(m => m.AccountId == Identity.Id && (m.IsManager || m.IsOwner)))
            {
                if (string.IsNullOrWhiteSpace(group.MemberInviteCode))
                {
                    // create invite code
                    group.MemberInviteCode = Guid.NewGuid().ToString().ToLower();
                    await DbContext.SaveChangesAsync();
                }

                return group.MemberInviteCode;
            }

            return string.Empty;
        }

        /// <summary>
        /// re generate invite code
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<string> UpdateMemberInviteCodeById(string id)
        {
            var group = await DbContext.Groups.Include(g => g.Members).SingleOrDefaultAsync(g => g.Id == id);

            if (group.Members.Any(m => m.AccountId == Identity.Id && (m.IsManager || m.IsOwner)))
            {
                group.MemberInviteCode = Guid.NewGuid().ToString().ToLower();
                await DbContext.SaveChangesAsync();

                return group.MemberInviteCode;
            }

            return string.Empty;
        }

        /// <summary>
        /// delete invite code
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteMemberInviteCodeById(string id)
        {
            var group = await DbContext.Groups.Include(g => g.Members).SingleOrDefaultAsync(g => g.Id == id);

            if (group.Members.Any(m => m.AccountId == Identity.Id && (m.IsManager || m.IsOwner)))
            {
                group.MemberInviteCode = string.Empty;
                await DbContext.SaveChangesAsync();

                return true;
            }

            return false;
        }

        /// <summary>
        /// get group by group invite code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<GroupDetail> GetByGroupInviteCode(string code)
        {
            var group = await DbContext.Groups.SingleOrDefaultAsync(g => g.GroupInviteCode == code);

            if (group == null)
                throw new EntityNotFoundException("Invalid code");

            return await GetById(group.Id);
        }

        /// <summary>
        /// get group invite code for group by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<string> GetGroupInviteCodeById(string id)
        {
            var group = await DbContext.Groups.Include(g => g.Members).SingleOrDefaultAsync(g => g.Id == id);

            if (group.Members.Any(m => m.AccountId == Identity.Id && (m.IsManager || m.IsOwner)))
            {
                if (string.IsNullOrWhiteSpace(group.GroupInviteCode))
                {
                    // create invite code
                    group.GroupInviteCode = Guid.NewGuid().ToString().ToLower();
                    await DbContext.SaveChangesAsync();
                }

                return group.GroupInviteCode;
            }

            return string.Empty;
        }

        /// <summary>
        /// re generate group invite code
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<string> UpdateGroupInviteCodeById(string id)
        {
            var group = await DbContext.Groups.Include(g => g.Members).SingleOrDefaultAsync(g => g.Id == id);

            if (group.Members.Any(m => m.AccountId == Identity.Id && (m.IsManager || m.IsOwner)))
            {
                group.GroupInviteCode = Guid.NewGuid().ToString().ToLower();
                await DbContext.SaveChangesAsync();

                return group.GroupInviteCode;
            }

            return string.Empty;
        }

        /// <summary>
        /// delete group invite code
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteGroupInviteCodeById(string id)
        {
            var group = await DbContext.Groups.Include(g => g.Members).SingleOrDefaultAsync(g => g.Id == id);

            if (group.Members.Any(m => m.AccountId == Identity.Id && (m.IsManager || m.IsOwner)))
            {
                group.GroupInviteCode = string.Empty;
                await DbContext.SaveChangesAsync();

                return true;
            }

            return false;
        }
    }
}

