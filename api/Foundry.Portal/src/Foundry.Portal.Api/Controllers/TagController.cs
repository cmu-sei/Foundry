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
    /// tag api endpoints
    /// </summary>
    [SecurityHeaders]
    [StackAuthorize]
    public class TagController : ApiController
    {
        TagService TagService { get; set; }

        /// <summary>
        /// creates an instance of the TagController
        /// </summary>
        /// <param name="options"></param>
        /// <param name="mill"></param>
        /// <param name="tagService"></param>
        public TagController(CoreOptions options, ILoggerFactory mill, TagService tagService)
            : base(options, mill)
        {
            TagService = tagService ?? throw new ArgumentNullException("tagService");
        }

        /// <summary>
        /// get all tags
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [Route("api/tags")]
        [HttpGet]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PagedResult<Tag, TagSummary>), 200)]
        public async Task<IActionResult> GetAll([FromQuery]TagDataFilter search = null)
        {
            return Ok(await TagService.GetAll(search));
        }

        /// <summary>
        /// add tag
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        [Route("api/tags")]
        [HttpPost]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(TagDetail[]), 200)]
        public async Task<IActionResult> Add([FromBody] string[] tags)
        {
            return Ok(await TagService.AddTags(string.Empty, tags));
        }

        /// <summary>
        /// add tags with type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        [Route("api/{type}/tags")]
        [HttpPost]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(TagDetail[]), 200)]
        public async Task<IActionResult> Add([FromQuery]string type, [FromBody] string[] tags)
        {
            return Ok(await TagService.AddTags(type, tags));
        }

        /// <summary>
        /// update existing tag
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("api/tag/{id}")]
        [HttpPut]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(TagDetail), 200)]
        public async Task<IActionResult> Update(int id, [FromBody] TagUpdate model)
        {
            return Ok(await TagService.Update(id, model));
        }

        /// <summary>
        /// delete a tag by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Route("api/tag/{name}")]
        [HttpDelete]
        [ProducesResponseType(typeof(bool), 200)]
        [JsonExceptionFilter]
        public async Task<IActionResult> Delete(string name)
        {
            return Ok(await TagService.Delete(name));
        }

        /// <summary>
        /// deletes tags by array of ids
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        [Route("api/tags")]
        [HttpDelete]
        [ProducesResponseType(typeof(string[]), 200)]
        [JsonExceptionFilter]
        public async Task<IActionResult> DeleteRange([FromBody] int[] tags)
        {
            return Ok(await TagService.Delete(tags));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}

