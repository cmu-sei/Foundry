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
using Newtonsoft.Json.Linq;
using Foundry.Analytics.Services;
using Stack.Http.Attributes;
using Stack.Http.Identity;
using Stack.Http.Identity.Attributes;
using System;
using System.Threading.Tasks;

namespace Foundry.Analytics.Controllers
{
    /// <summary>
    /// integrations
    /// </summary>
    [StackAuthorize]
    public class IntegrationsController : StackController
    {
        IntegrationsService Service { get; }

        /// <summary>
        /// create instance
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="integrationsService"></param>
        public IntegrationsController(IStackIdentityResolver identityResolver, IntegrationsService integrationsService)
            : base(identityResolver)
        {
            Service = integrationsService ?? throw new ArgumentNullException(nameof(integrationsService));
        }

        /// <summary>
        /// retrieves exercise leaderboard details
        /// </summary>
        /// <param name="id">The exercise guid</param>
        /// <param name="limit">The maximum number of results to return</param>
        /// <returns></returns>
        [HttpGet("api/integrations/pctc/exercise/{id}/leaderboards/{limit}")]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> ExerciseLeaderboard([FromRoute]string id, [FromRoute]int limit)
        {
            string data = await Service.GetExerciseLeaderboardResults(id, limit);
            return Ok(JObject.Parse(data));
        }
    }
}
