/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.AspNetCore.Mvc;
using Foundry.Buckets.Data.Entities;
using Foundry.Buckets.Services;
using Foundry.Buckets.ViewModels;
using Stack.Http.Attributes;
using Stack.Http.Identity;
using Stack.Http.Identity.Attributes;
using Stack.Patterns.Service.Models;
using System;
using System.Threading.Tasks;

namespace Foundry.Buckets.Controllers
{
    /// <summary>
    /// account api endpoint
    /// </summary>
    [StackAuthorize]
    public class AccountController : BaseController
    {
        AccountService AccountService { get; }

        /// <summary>
        /// creates an instance of account controller
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="accountService"></param>
        public AccountController(IStackIdentityResolver identityResolver, AccountService accountService)
            : base(identityResolver)
        {
            AccountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        }

        /// <summary>
        /// get accounts
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet("api/accounts")]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(PagedResult<Account, AccountSummary>), 200)]
        public async Task<IActionResult> GetAll([FromQuery]AccountDataFilter search = null)
        {
            return Ok(await AccountService.GetAll(search));
        }

        /// <summary>
        /// get account by global id
        /// </summary>
        /// <param name="globalId"></param>
        /// <returns></returns>
        [HttpGet("api/account/{globalId}")]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(AccountDetail), 200)]
        public async Task<IActionResult> GetByGlobalId([FromRoute]string globalId)
        {
            return Ok(await AccountService.GetByGlobalId(globalId));
        }

        /// <summary>
        /// update account
        /// </summary>
        /// <param name="globalId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("api/account/{globalId}")]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(AccountDetail), 200)]
        public async Task<IActionResult> Update([FromRoute]string globalId, [FromBody]AccountUpdate model)
        {
            return Ok(await AccountService.Update(model));
        }

        /// <summary>
        /// create account
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("api/accounts")]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(AccountDetail), 200)]
        public async Task<IActionResult> Add([FromBody]AccountCreate model)
        {
            return Ok(await AccountService.Add(model));
        }
    }
}
