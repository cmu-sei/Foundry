/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Foundry.Analytics.Services;
using Foundry.Analytics.xApi;
using Foundry.Analytics.xApi.Statements;
using Stack.Http.Attributes;
using Stack.Http.Identity;
using Stack.Http.Identity.Attributes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foundry.Analytics.Controllers
{
    /// <summary>
    /// learning record statement api endpoints
    /// </summary>
    [StackAuthorize]
    public class StatementController : StackController
    {
        StatementService Service { get; }

        /// <summary>
        /// creates an instance of statement controller
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="service"></param>
        public StatementController(IStackIdentityResolver identityResolver, StatementService service)
            : base(identityResolver)
        {
            Service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// get all statements by verb
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet("api/statements/{verb}")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(List<IStatement>), 200)]
        public async Task<IActionResult> GetAllByVerb([FromRoute]string verb, [FromQuery]int? limit = 0)
        {
            switch (verb)
            {
                case "logged-in":
                case "launched":
                case "viewed":
                case "searched":
                    return Ok(await Service.GetAllByVerb(verb, limit));
            }

            return NotFound();
        }

        /// <summary>
        /// get all statements for profile by verb
        /// </summary>
        /// <param name="globalId"></param>
        /// <param name="verb"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet("api/profile/{globalId}/statements/{verb}")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(List<IStatement>), 200)]
        public async Task<IActionResult> GetAllByAgentAndVerb([FromRoute]string globalId, [FromRoute]string verb, [FromQuery]int? limit = 0)
        {
            switch (verb)
            {
                case "logged-in":
                case "launched":
                case "viewed":
                case "searched":
                    return Ok(await Service.GetAllByAgentAndVerb(globalId, verb, limit));
            }

            return NotFound();
        }

        /// <summary>
        /// get all profile statement
        /// </summary>
        /// <param name="globalId"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet("api/profile/{globalId}/statements")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(List<IStatement>), 200)]
        public async Task<IActionResult> GetAllByProfileGlobalId([FromRoute]string globalId, [FromQuery]int? limit = 0)
        {
            return Ok(await Service.GetAllByAgent(globalId));
        }

        /// <summary>
        /// add launch
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("api/statements/launched")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> AddContentLaunch([FromBody] LaunchStatementCreate model)
        {
            return Ok(await Service.AddContentLaunch(model));
        }

        /// <summary>
        /// add view
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("api/statements/viewed")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> AddContentView([FromBody] ViewStatementCreate model)
        {
            return Ok(await Service.AddContentView(model));
        }

        /// <summary>
        /// add search
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("api/statements/searched")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> AddSearch([FromBody] SearchStatementCreate model)
        {
            return Ok(await Service.AddSearch(model));
        }

        /// <summary>
        /// add login
        /// </summary>
        /// <returns></returns>
        [HttpPost("api/statements/logged-in")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> AddLogin()
        {
            return Ok(await Service.AddUserLogin());
        }
    }
}
