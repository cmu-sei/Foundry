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
using Foundry.Analytics.Services;
using Foundry.Analytics.ViewModels;
using Stack.Http.Attributes;
using Stack.Http.Identity;
using Stack.Http.Identity.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Analytics.Controllers
{
    /// <summary>
    /// event api endpoints
    /// </summary>
    [StackAuthorize]
    public class EventController : StackController
    {
        ClientEventService ClientEventService { get; }
        ContentEventService ContentEventService { get; }
        UserEventService UserEventService { get; }

        /// <summary>
        /// create instance
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="clientEventService"></param>
        /// <param name="contentEventService"></param>
        /// <param name="userEventService"></param>
        public EventController(IStackIdentityResolver identityResolver, ClientEventService clientEventService, ContentEventService contentEventService, UserEventService userEventService)
            : base(identityResolver)
        {
            ClientEventService = clientEventService ?? throw new ArgumentNullException(nameof(clientEventService));
            ContentEventService = contentEventService ?? throw new ArgumentNullException(nameof(contentEventService));
            UserEventService = userEventService ?? throw new ArgumentNullException(nameof(userEventService));
        }

        /// <summary>
        /// get all events
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/events")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(List<IAnalyticsEventSummary>), 200)]
        public async Task<IActionResult> GetAll()
        {
            //NOTE: right now we only track 'page-view' client events so ignore for the time being
            //var clientEvents = await ClientEventService.GetAll(new ClientEventDataFilter());
            var contentEvents = await ContentEventService.GetAll(new ContentEventDataFilter());
            var userEvents = await UserEventService.GetAll(new UserEventDataFilter());

            var events = new List<IAnalyticsEventSummary>();

            //events.AddRange(clientEvents.Results);
            events.AddRange(contentEvents.Results);
            events.AddRange(userEvents.Results);

            events = events.OrderByDescending(e => e.Created).ToList();

            return Ok(events);
        }
    }
}
