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
    /// validate member exists
    /// </summary>
    public class MemberDoesNotExist :
        IValidationRule<MemberRequestCreate>,
        IValidationRule<MemberCreate>,
        IValidationRule<MemberInviteAccept>
    {
        public MemberDoesNotExist(GroupsDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public GroupsDbContext DbContext { get; set; }

        /// <summary>
        /// validate member request create
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task Validate(MemberRequestCreate model)
        {
            if (await MemberExists(model.AccountId, model.GroupId))
                throw new InvalidModelException("Account is already a member of this group and cannot create member request.");
        }

        /// <summary>
        /// validate member create
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task Validate(MemberCreate model)
        {
            if (await MemberExists(model.AccountId, model.GroupId))
                throw new InvalidModelException("Account is already a member of this group and cannot create member.");
        }

        /// <summary>
        /// validate member invite accept
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task Validate(MemberInviteAccept model)
        {
            if (await DbContext.Members.AnyAsync(m => m.AccountId == model.AccountId && m.Group.MemberInviteCode == model.Code))
                throw new InvalidModelException("Account is already a member of this group and cannot create member.");
        }

        async Task<bool> MemberExists(string accountId, string groupId)
        {
            return await DbContext.Members.AnyAsync(m => m.AccountId == accountId && m.GroupId == groupId);
        }
    }
}
