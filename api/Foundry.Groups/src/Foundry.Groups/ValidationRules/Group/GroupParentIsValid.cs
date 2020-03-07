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
using System.Threading.Tasks;

namespace Foundry.Groups.ValidationRules
{
    /// <summary>
    /// validate parent is valid
    /// </summary>
    /// <remarks>This may not be enforced</remarks>
    public class GroupParentIsValid :
        IValidationRule<GroupUpdate>,
        IValidationRule<GroupRequestUpdate>,
        IValidationRule<GroupRequestCreate>
    {
        public GroupParentIsValid(GroupsDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public GroupsDbContext DbContext { get; set; }

        /// <summary>
        /// validate update model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task Validate(GroupUpdate model)
        {
            if (model == null)
                throw new InvalidModelException("null");

            await Validate(model.Id, model.ParentId);
        }

        public async Task Validate(GroupRequestUpdate model)
        {
            if (model == null)
                throw new InvalidModelException("null");

            await Validate(model.ChildGroupId, model.ParentGroupId);
        }

        public async Task Validate(GroupRequestCreate model)
        {
            if (model == null)
                throw new InvalidModelException("null");

            await Validate(model.ChildGroupId, model.ParentGroupId);
        }

        async Task Validate(string childGroupId, string parentGroupId)
        {
            if (string.IsNullOrWhiteSpace(parentGroupId))
                return;

            var parent = await DbContext.Groups.SingleOrDefaultAsync(g => g.Id == parentGroupId);

            if (parent.Key.ToLower().Contains(childGroupId.ToLower()))
                throw new InvalidModelException("The group '" + parent.Name + "' is a descendent of this group and cannot be selected as it's parent");
        }
    }
}
