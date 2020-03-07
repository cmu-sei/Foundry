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
using Microsoft.Extensions.Logging;
using Foundry.Groups.Data;
using Foundry.Groups.Services;
using Foundry.Groups.ViewModels;
using Stack.Http.Attributes;
using Stack.Http.Identity.Attributes;
using Stack.Patterns.Service.Models;
using System.Threading.Tasks;

namespace Foundry.Groups.Controllers
{
    /// <summary>
    /// account controller
    /// </summary>
    [StackAuthorize]
    public class AccountController : Controller<AccountService>
    {
        /// <summary>
        /// create an instance of account service
        /// </summary>
        /// <param name="accountService"></param>
        /// <param name="logger"></param>
        public AccountController(AccountService accountService, ILogger<AccountController> logger)
            : base(accountService, logger) { }

        /// <summary>
        /// get all accounts
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet("api/accounts")]
        [ProducesResponseType(typeof(PagedResult<Account, AccountSummary>), 200)]
        [JsonExceptionFilter]
        public async Task<IActionResult> GetAll([FromQuery]AccountDataFilter search = null)
        {
            return Ok(await Service.GetAll(search ?? new AccountDataFilter()));
        }

        /// <summary>
        /// get account by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("api/account/{id}")]
        [ProducesResponseType(typeof(AccountDetail), 200)]
        [JsonExceptionFilter]
        public async Task<IActionResult> GetById([FromRoute] string id)
        {
            return Ok(await Service.GetById(id));
        }
    }
}

