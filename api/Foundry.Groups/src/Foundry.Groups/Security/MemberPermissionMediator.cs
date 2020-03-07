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
using Stack.Http.Identity;
using System.Linq;

namespace Foundry.Groups.Security
{
    public class MemberPermissionMediator : PermissionMediator<Member>
    {
        public MemberPermissionMediator(IStackIdentityResolver identityResolver)
            : base(identityResolver) { }

        public override IQueryable<Member> Process(IQueryable<Member> query)
        {
            if (IsAdministrator)
                return query;

            return query;
        }

        public override bool CanUpdate(Member entity)
        {
            if (IsAdministrator)
                return true;

            if (entity.Group.Members.Any(m => m.AccountId == Identity.Id && (m.IsManager || m.IsOwner)))
                return true;

            return false;
        }

        public override bool CanDelete(Member entity)
        {
            if (IsAdministrator)
                return true;

            if (entity.Group.Members.Any(m => m.AccountId == Identity.Id && (m.IsManager || m.IsOwner)))
                return true;

            if (entity.AccountId == Identity.Id)
                return true;

            return false;
        }

        public override bool CanAdd(Member entity)
        {
            if (IsAdministrator)
                return true;

            return false;
        }
    }
}

