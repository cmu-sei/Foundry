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
using Foundry.Portal.Services;
using Foundry.Portal.ViewModels;
using Stack.Http.Attributes;
using Stack.Http.Identity.Attributes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foundry.Portal.Api.Controllers
{
    /// <summary>
    /// setting controller
    /// </summary>
    [SecurityHeaders]
    [StackAuthorize]
    public class SettingController : ApiController
    {
        SettingService SettingService { get; }

        /// <summary>
        /// creates an instance of the setting controller
        /// </summary>
        /// <param name="settingService"></param>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        public SettingController(SettingService settingService, CoreOptions options, ILoggerFactory logger)
            : base(options, logger)
        {
            SettingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
        }

        /// <summary>
        /// get all settings
        /// </summary>
        [Route("api/settings")]
        [HttpGet]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(List<SettingDetail>), 200)]
        public async Task<IActionResult> GetAll(SettingDataFilter search)
        {
            return Ok(await SettingService.GetAll(search));
        }

        /// <summary>
        /// get setting by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Route("api/setting/{key}")]
        [HttpGet]
        [JsonExceptionFilter]
        [AllowAnonymous]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(SettingDetail), 200)]
        public async Task<IActionResult> Get([FromRoute]string key)
        {
            return Ok(await SettingService.GetByKey(key));
        }

        /// <summary>
        /// update setting
        /// </summary>
        /// <param name="key"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("api/setting/{key}")]
        [HttpPut]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(SettingDetail), 200)]
        public async Task<IActionResult> Update([FromRoute]string key, [FromBody]SettingUpdate model)
        {
            return Ok(await SettingService.Update(model));
        }

        /// <summary>
        /// add setting
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("api/settings")]
        [HttpPost]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(SettingDetail), 200)]
        public async Task<IActionResult> Add([FromBody]SettingCreate model)
        {
            return Ok(await SettingService.Add(model));
        }
    }
}

