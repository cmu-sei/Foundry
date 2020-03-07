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
    /// content event api endpoints
    /// </summary>
    [StackAuthorize]
    public class ContentEventController : StackController
    {
        ContentEventService Service { get; }

        /// <summary>
        /// create instance
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="contentEventService"></param>
        public ContentEventController(IStackIdentityResolver identityResolver, ContentEventService contentEventService)
            : base(identityResolver)
        {
            Service = contentEventService ?? throw new ArgumentNullException(nameof(contentEventService));
        }

        /// <summary>
        /// get all content events
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet("api/events/content")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PagedResult<ContentEvent, ContentEventSummary>), 200)]
        public async Task<IActionResult> GetAll([FromQuery]ContentEventDataFilter search)
        {
            return Ok(await Service.GetAll(search));
        }

        /// <summary>
        /// add a content event for current profile
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("api/events/content")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(ContentEventSummary), 200)]
        public async Task<IActionResult> Add([FromBody]ContentEventCreate model)
        {
            return Ok(await Service.Add(model));
        }
    }
}
