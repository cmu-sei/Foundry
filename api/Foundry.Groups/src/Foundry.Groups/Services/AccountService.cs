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
using Foundry.Groups.Data;
using Foundry.Groups.Data.Repositories;
using Foundry.Groups.ViewModels;
using Stack.Http.Identity;
using Stack.Patterns.Service.Models;
using Stack.Validation.Handlers;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Groups.Services
{
    /// <summary>
    /// account service
    /// </summary>
    public class AccountService : Service<IAccountRepository, Account>
    {
        /// <summary>
        /// create an instance of account service
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="accountRepository"></param>
        /// <param name="mapper"></param>
        /// <param name="validationHandler"></param>
        public AccountService(IStackIdentityResolver identityResolver, IAccountRepository accountRepository, IMapper mapper, IValidationHandler validationHandler)
            : base(identityResolver, accountRepository, mapper, validationHandler) { }

        /// <summary>
        /// get all accounts
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<PagedResult<Account, AccountSummary>> GetAll(AccountDataFilter search = null)
        {
            return await PagedResult<Account, AccountSummary>(Repository.GetAll(), search);
        }

        /// <summary>
        /// get account by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<AccountDetail> GetById(string id)
        {
            return Map<AccountDetail>(await Repository.GetById(id));
        }
    }
}

