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
using Mos.xApi;
using Foundry.Portal.Data;
using Foundry.Portal.Data.Entities;
using Foundry.Portal.Services;
using Foundry.Portal.ViewModels;
using Stack.Http.Attributes;
using Stack.Http.Identity.Attributes;
using Stack.Patterns.Service.Models;
using System;
using System.Threading.Tasks;

namespace Foundry.Portal.Api.Controllers
{
    /// <summary>
    /// profile api endpoints
    /// </summary>
    [SecurityHeaders]
    [StackAuthorize]
    public class ProfileController : ApiController
    {
        ProfileService _profileService;
        readonly PlaylistService _playlistService;
        ContentService _contentService;

        /// <summary>
        /// api profile endpoints
        /// </summary>
        /// <param name="options"></param>
        /// <param name="mill"></param>
        /// <param name="profileService"></param>
        /// <param name="playlistService"></param>
        /// <param name="contentService"></param>
        public ProfileController(
            CoreOptions options,
            ILoggerFactory mill,
            ProfileService profileService,
            PlaylistService playlistService,
            ContentService contentService
        ) : base(options, mill)
        {
            _profileService = profileService ?? throw new ArgumentNullException("profileService");
            _playlistService = playlistService ?? throw new ArgumentNullException("playlistService");
            _contentService = contentService ?? throw new ArgumentNullException("contentService");
        }

        /// <summary>
        /// search all profiles
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [Route("api/profiles")]
        [HttpGet]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PagedResult<Profile, ProfileSummary>), 200)]
        public async Task<IActionResult> GetAll([FromQuery]ProfileDataFilter search = null)
        {
            return Ok(await _profileService.GetAll(search));
        }

        /// <summary>
        /// get all authors by rating
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [Route("api/authors/top")]
        [HttpGet]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PagedResult<Profile, ProfileSummary>), 200)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GetAllByRating([FromQuery]ProfileDataFilter search = null)
        {
            return Ok(await _profileService.GetAllByRating(Rating.Good, 10, search));
        }

        /// <summary>
        /// get profile by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/profile/{id:int}")]
        [HttpGet]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(ProfileDetail), 200)]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            return Ok(await _profileService.GetById(id));
        }

        /// <summary>
        /// get profile by global id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)] //TODO: if this is not ignored by the swagger api gen it causes a conflict and server error with the GetById method with a similar route [Route("api/profile/{id:int}")]
        [Route("api/profile/{id:guid}")]
        [HttpGet]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(ProfileDetail), 200)]
        public async Task<IActionResult> GetByGlobalId([FromRoute]Guid id)
        {
            return Ok(await _profileService.GetByGlobalId(id.ToString().ToLower()));
        }

        /// <summary>
        /// add profile
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("api/profiles")]
        [HttpPost]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(ProfileDetail), 200)]
        public async Task<IActionResult> Add([FromBody]ProfileCreate model)
        {
            return Ok(await _profileService.Add(model));
        }

        /// <summary>
        /// update profile
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("api/profile/{id}")]
        [HttpPut]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(ProfileDetail), 200)]
        public async Task<IActionResult> Update([FromRoute]int id, [FromBody]ProfileUpdate model)
        {
            return Ok(await _profileService.Update(model));
        }

        /// <summary>
        /// add or update profile key value
        /// </summary>
        /// <param name="id">profile id</param>
        /// <param name="key">profile key</param>
        /// <param name="value">profile value</param>
        /// <returns></returns>
        [Route("api/profile/{id}/{key}")]
        [HttpPut]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(ProfileDetail), 200)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> SetKeyValue([FromRoute]int id, [FromRoute]string key, [FromBody]string value)
        {
            await _profileService.SetKeyValue(id, key, value);

            return Ok(await _profileService.GetById(id));
        }


        /// <summary>
        /// get all content for profile by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        [Route("api/profile/{id:int}/content")]
        [HttpGet]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PagedResult<Content, ContentSummary>), 200)]
        public async Task<IActionResult> Content([FromRoute] int id, [FromQuery] ContentDataFilter search = null)
        {
            return Ok(await _contentService.GetAllContentByProfileId(id, search));
        }

        /// <summary>
        /// enable profile
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/profile/{id}/disabled")]
        [HttpPut]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(ProfileSummary), 200)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Enabled([FromRoute]int id)
        {
            return Ok(await _profileService.SetDisabled(id, false));
        }

        /// <summary>
        /// disable profile
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/profile/{id}/disabled")]
        [HttpDelete]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(ProfileSummary), 200)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Disabled([FromRoute]int id)
        {
            return Ok(await _profileService.SetDisabled(id, true));
        }

        /// <summary>
        /// toggle administrator
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/profile/{id}/permissions/administrator")]
        [HttpPut]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(ProfileSummary), 200)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ToggleAdministrator([FromRoute]int id)
        {
            return Ok(await _profileService.TogglePermission(id, SystemPermissions.Administrator));
        }

        /// <summary>
        /// toggle power user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/profile/{id}/permissions/poweruser")]
        [HttpPut]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(ProfileSummary), 200)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> TogglePowerUser([FromRoute]int id)
        {
            return Ok(await _profileService.TogglePermission(id, SystemPermissions.PowerUser));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}

