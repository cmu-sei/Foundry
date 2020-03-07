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
using Stack.Http.Exceptions;
using Stack.Validation.Rules;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Groups.ValidationRules
{
    /// <summary>
    /// validate child group exists
    /// </summary>
    /// <remarks>This may not be enforced</remarks>
    public class GroupIsNotChild :
        IValidationRule<GroupRequestCreate>,
        IValidationRule<GroupInviteAccept>
    {
        public GroupIsNotChild(GroupsDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public GroupsDbContext DbContext { get; set; }

        /// <summary>
        /// validate create model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task Validate(GroupRequestCreate model)
        {
            if (model == null)
                throw new InvalidModelException("null");

            var parent = DbContext.Groups.SingleOrDefault(g => g.Id == model.ParentGroupId);

            if (parent == null)
                throw new EntityNotFoundException("Parent group was not found.");

            var child = DbContext.Groups.SingleOrDefault(g => g.Id == model.ChildGroupId);

            if (child == null)
                throw new EntityNotFoundException("Child group was not found.");

            Validate(parent, child);
        }

        /// <summary>
        /// validate invite model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task Validate(GroupInviteAccept model)
        {
            if (model == null)
                throw new InvalidModelException("null");

            var parent = DbContext.Groups.SingleOrDefault(g => g.GroupInviteCode == model.Code);

            if (parent == null)
                throw new EntityNotFoundException("Group not found for code.");

            var child = DbContext.Groups.SingleOrDefault(g => g.Id == model.GroupId);

            Validate(parent, child);
        }

        void Validate(Group parent, Group child)
        {
            if (child.ParentId == parent.Id)
                throw new EntityNotFoundException("Group is already a child of parent.");

            if (child.Key.Contains(parent.Id))
                throw new EntityNotFoundException("Group is already a descendent of parent.");
        }
    }
}

