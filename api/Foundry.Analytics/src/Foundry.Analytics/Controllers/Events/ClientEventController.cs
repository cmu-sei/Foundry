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
using Foundry.Analytics.Data;
using Foundry.Analytics.Services;
using Foundry.Analytics.ViewModels;
using Stack.Http.Attributes;
using Stack.Http.Identity;
using Stack.Http.Identity.Attributes;
using Stack.Patterns.Service.Models;
using System;
using System.Threading.Tasks;

namespace Foundry.Analytics.Controllers
{
    /// <summary>
    /// client event api endpoints
    /// </summary>
    [StackAuthorize]
    public class ClientEventController : StackController
    {
        ClientEventService Service { get; }

        /// <summary>
        /// creates instance
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="clientEventService"></param>
        public ClientEventController(IStackIdentityResolver identityResolver, ClientEventService clientEventService)
            : base(identityResolver)
        {
            Service = clientEventService ?? throw new ArgumentNullException(nameof(clientEventService));
        }

        /// <summary>
        /// get all client events
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet("api/events/client")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PagedResult<ClientEvent, ClientEventSummary>), 200)]
        public async Task<IActionResult> GetAll([FromQuery]ClientEventDataFilter search)
        {
            return Ok(await Service.GetAll(search));
        }

        /// <summary>
        /// get page views client event report
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [HttpGet("api/events/client/page-views-metric")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PageViewMetric), 200)]
        public async Task<IActionResult> GetPageViewMetric([FromQuery]string url)
        {
            return Ok(await Service.GetPageViewMetric(url));
        }

        /// <summary>
        /// add a client event
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("api/events/client")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(ClientEventSummary), 200)]
        public async Task<IActionResult> Add([FromBody]ClientEventCreate model)
        {
            return Ok(await Service.Add(model));
        }
    }
}
