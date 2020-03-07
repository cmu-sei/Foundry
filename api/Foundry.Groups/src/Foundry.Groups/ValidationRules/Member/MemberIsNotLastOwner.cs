/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.EntityFrameworkCore;
using Foundry.Groups.Data;
using Foundry.Groups.ViewModels;
using Stack.Http.Exceptions;
using Stack.Validation.Rules;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Groups.ValidationRules
{
    /// <summary>
    /// validate member is not the last owner of a group
    /// </summary>
    public class MemberIsNotLastOwner :
        IValidationRule<MemberUpdate>,
        IValidationRule<MemberDelete>
    {
        public MemberIsNotLastOwner(GroupsDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public GroupsDbContext DbContext { get; set; }

        /// <summary>
        /// validate member update
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task Validate(MemberUpdate model)
        {
            // if the member is an owner and is being updated to lose ownership
            if (IsMemberLastOwner(model.AccountId, model.GroupId) && !model.IsOwner)
                throw new InvalidModelException("Member is the last Owner of this Group and cannot be updated.");
        }

        /// <summary>
        /// validate member delete
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task Validate(MemberDelete model)
        {
            if (IsMemberLastOwner(model.AccountId, model.GroupId))
                throw new InvalidModelException("Member is the last Owner of this Group and cannot be deleted.");
        }

        bool IsMemberLastOwner(string accountId, string groupId)
        {
            var member = DbContext.Members.SingleOrDefault(m => m.AccountId == accountId && m.GroupId == groupId);

            if (member.IsOwner)
            {
                var count = DbContext.Members.Count(m => m.GroupId == groupId && m.IsOwner);
                return count == 1;
            }

            return false;
        }
    }
}
