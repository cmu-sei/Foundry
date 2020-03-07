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
using Stack.Http.Identity;
using System;
using System.Linq;

namespace Foundry.Groups
{
    public static class IStackIdentityExtensions
    {
        const string Administrator = "administrator";
        public static AccountActions SetActions(this IStackIdentity identity, Account account)
        {
            var actions = new AccountActions();
            if (identity != null && identity.Permissions.Contains(Administrator))
            {
                actions.Edit = true;
                actions.Delete = true;
            }

            return actions;
        }

        public static GroupActions SetActions(this IStackIdentity identity, Group group)
        {
            var actions = new GroupActions();

            if (identity != null)
            {
                var member = group.Members.SingleOrDefault(bs => bs.AccountId.ToLower() == identity.Id.ToLower());
                var memberRequest = group.MemberRequests.SingleOrDefault(bs => bs.AccountId.ToLower() == identity.Id.ToLower());

                if (identity.Permissions.Contains(Administrator))
                {
                    actions.Edit = true;
                    actions.Delete = true;
                }
                else if (member != null)
                {
                    actions.Edit = member.IsOwner || member.IsManager;
                    actions.Delete = member.IsOwner;
                }

                actions.Leave = member != null;
                actions.Join = member == null  && memberRequest == null;
            }

            return actions;
        }

        public static GroupRoles SetRoles(this IStackIdentity identity, Group group)
        {
            var roles = new GroupRoles();

            if (identity != null)
            {
                var member = group.Members.SingleOrDefault(bs => bs.AccountId.ToLower() == identity.Id.ToLower());

                if (member != null)
                {
                    roles.Member = true;
                    roles.Manager = member.IsManager;
                    roles.Owner = member.IsOwner;
                }
            }

            return roles;
        }

        public static MemberActions SetActions(this IStackIdentity identity, Member member)
        {
            var actions = new MemberActions();
            if (identity != null)
            {
                if (identity.Permissions.Contains(Administrator))
                {
                    actions.Edit = true;
                    actions.Delete = true;
                }
                else if (member != null)
                {
                    actions.Edit = member.IsOwner || member.IsManager;
                    actions.Delete = member.IsOwner;
                }

                if (member.AccountId == identity.Id)
                {
                    actions.Delete = true;
                }
            }

            return actions;
        }

        public static MemberRequestActions SetActions(this IStackIdentity identity, MemberRequest memberRequest)
        {
            var actions = new MemberRequestActions();
            if (identity != null)
            {
                if (identity.Permissions.Contains(Administrator))
                {
                    actions.Edit = true;
                    actions.Delete = true;
                }
                else
                {
                    var member = memberRequest.Group.Members.SingleOrDefault(m => m.AccountId == identity.Id);

                    if (member != null)
                    {
                        actions.Edit = member.IsOwner || member.IsManager;
                        actions.Delete = member.IsOwner;
                    }
                    else
                    {
                        actions.Delete = memberRequest.AccountId == identity.Id && memberRequest.Status == MemberRequestStatus.Pending;
                    }
                }
            }

            return actions;
        }

        public static GroupRequestActions SetActions(this IStackIdentity identity, GroupRequest groupRequest)
        {
            var actions = new GroupRequestActions();
            if (identity != null)
            {
                if (identity.Permissions.Contains(Administrator))
                {
                    actions.Edit = true;
                    actions.Delete = true;
                }
                else
                {
                    var parentGroupMember = groupRequest.ParentGroup.Members.SingleOrDefault(m => m.AccountId == identity.Id);

                    if (parentGroupMember != null)
                    {
                        actions.Edit = parentGroupMember.IsOwner || parentGroupMember.IsManager;
                        actions.Delete = parentGroupMember.IsOwner;
                    }

                    var childGroupMember = groupRequest.ChildGroup.Members.SingleOrDefault(m => m.AccountId == identity.Id);

                    if (childGroupMember != null && childGroupMember.IsOwner && groupRequest.Status == GroupRequestStatus.Pending)
                    {
                        actions.Delete = true;
                    }

                }
            }

            return actions;
        }
    }
}

