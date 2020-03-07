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
using Microsoft.Extensions.Logging;
using Foundry.Portal.Data.Entities;
using Foundry.Portal.Services;
using Foundry.Portal.ViewModels;
using Stack.Http.Attributes;
using Stack.Http.Identity.Attributes;
using Stack.Patterns.Service.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foundry.Portal.Api.Controllers
{
    /// <summary>
    /// extensions controller
    /// </summary>
    [SecurityHeaders]
    [StackAuthorize]
    public class ApplicationController : ApiController
    {
        ApplicationService _applicationService;

        /// <summary>
        /// create an instance of the application controller
        /// </summary>

        public ApplicationController(ApplicationService applicationService, CoreOptions options, ILoggerFactory mill)
            : base(options, mill)
        {
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
        }

        /// <summary>
        /// get all applications
        /// </summary>
        [Route("api/apps")]
        [HttpGet]
        [AllowAnonymous]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PagedResult<Application, ApplicationSummary>), 200)]
        public async Task<IActionResult> GetAll([FromQuery]ApplicationDataFilter search = null)
        {
            return Ok(await _applicationService.GetAll(search));
        }

        /// <summary>
        /// update application
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("api/app/{id}")]
        [HttpPut]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(ApplicationSummary), 200)]
        public async Task<IActionResult> Update([FromBody] ApplicationUpdate model)
        {
            return Ok(await _applicationService.Update(model));
        }

        /// <summary>
        /// push application to all profiles
        /// </summary>
        [Route("api/apps/push")]
        [HttpPost]
        [AllowAnonymous]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> PushApplication([FromBody] int[] ids)
        {
            return Ok(await _applicationService.AddToAllProfiles(ids));
        }

        /// <summary>
        /// synchronize applications with identity server
        /// </summary>
        /// <returns></returns>
        [Route("api/apps/sync")]
        [HttpPost]
        [AllowAnonymous]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> SyncApplication()
        {
            return Ok(await _applicationService.Synchronize());
        }

        /// <summary>
        /// get all applications
        /// </summary>
        [Route("api/myapps")]
        [HttpGet]
        [AllowAnonymous]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(List<ApplicationSummary>), 200)]
        public async Task<IActionResult> GetAllForIdentity()
        {
            return Ok(await _applicationService.GetAll(new ApplicationDataFilter { Filter = "myapps|!hidden" }));
        }

        /// <summary>
        /// bookmark application for identity
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        [Route("api/myapps/{slug}")]
        [HttpPut]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> AddToProfile([FromRoute] string slug)
        {
            return Ok(await _applicationService.AddToIdentity(slug));
        }

        /// <summary>
        /// remove bookmarked application from identity
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        [Route("api/myapps/{slug}")]
        [HttpDelete]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> DeleteFromProfile([FromRoute] string slug)
        {
            return Ok(await _applicationService.DeleteFromIdentity(slug));
        }
    }
}

