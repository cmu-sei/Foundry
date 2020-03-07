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
    /// validate group exists
    /// </summary>
    /// <remarks>This may not be enforced</remarks>
    public class GroupExists :
        IValidationRule<GroupUpdate>,
        IValidationRule<GroupInviteAccept>,
        IValidationRule<MemberInviteAccept>,
        IValidationRule<MemberCreate>,
        IValidationRule<MemberRequestCreate>,
        IValidationRule<MemberRequestUpdate>,
        IValidationRule<MemberDelete>
    {
        public GroupExists(GroupsDbContext dbContext)
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
            await Validate(model.Id);
        }

        /// <summary>
        /// validate member create
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task Validate(MemberCreate model)
        {
            await Validate(model.GroupId);
        }

        /// <summary>
        /// validate member request
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task Validate(MemberRequestCreate model)
        {
            await Validate(model.GroupId);
        }

        /// <summary>
        /// validate member update
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task Validate(MemberRequestUpdate model)
        {
            await Validate(model.GroupId);
        }

        /// <summary>
        /// validate member delete
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task Validate(MemberDelete model)
        {
            await Validate(model.GroupId);
        }

        /// <summary>
        /// valid group invite accept
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task Validate(GroupInviteAccept model)
        {
            await Validate(model.GroupId);
        }

        /// <summary>
        /// valid member invite accept
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task Validate(MemberInviteAccept model)
        {
            if (!DbContext.Groups.Any(g => g.MemberInviteCode == model.Code))
                throw new EntityNotFoundException("Group was not found.");
        }


        async Task Validate(string id)
        {
            if (!DbContext.Groups.Any(g => g.Id == id))
                throw new EntityNotFoundException("Group was not found.");
        }
    }
}

