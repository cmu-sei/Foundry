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
using Foundry.Portal.Data;
using Foundry.Portal.Data.Entities;
using Foundry.Portal.Services;
using Foundry.Portal.ViewModels;
using Stack.Http.Attributes;
using Stack.Http.Identity.Attributes;
using Stack.Patterns.Service.Models;
using Stack.Validation.Attributes;
using System;
using System.Threading.Tasks;

namespace Foundry.Portal.Api.Controllers
{
    /// <summary>
    /// discussion api endpoints
    /// </summary>
    [SecurityHeaders]
    [StackAuthorize]
    public class DiscussionController : ApiController
    {
        DiscussionService _discussionService;

        /// <summary>
        /// creates an instance of the DiscussionController
        /// </summary>
        /// <param name="options"></param>
        /// <param name="mill"></param>
        /// <param name="discussionService"></param>
        public DiscussionController(CoreOptions options, ILoggerFactory mill, DiscussionService discussionService)
            : base(options, mill)
        {
            _discussionService = discussionService ?? throw new ArgumentNullException("discussionService");
        }

        /// <summary>
        /// get all discussions
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [Route("api/discussions")]
        [HttpGet]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PagedResult<Discussion, DiscussionSummary>), 200)]
        public async Task<IActionResult> GetAll([FromQuery]DiscussionDataFilter search)
        {
            var model = await _discussionService.GetAll(search);
            return Ok(model);
        }

        /// <summary>
        /// get discussion by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/discussion/{id}")]
        [HttpGet]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(DiscussionDetail), 200)]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            return Ok(await _discussionService.GetById(id));
        }

        /// <summary>
        /// update discussion by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="discussion"></param>
        /// <returns></returns>
        [Route("api/discussion/{id}")]
        [HttpPut]
        [JsonExceptionFilter]
        [ValidateModel]
        [ProducesResponseType(typeof(DiscussionDetail), 200)]
        public async Task<IActionResult> Update([FromRoute]int id, [FromBody]DiscussionUpdate discussion)
        {
            return Ok(await _discussionService.Update(discussion));
        }

        /// <summary>
        /// add discussion
        /// </summary>
        /// <param name="discussion"></param>
        /// <returns></returns>
        [Route("api/discussions")]
        [HttpPost]
        [JsonExceptionFilter]
        [ValidateModel]
        [ProducesResponseType(typeof(DiscussionDetail), 201)]
        public async Task<IActionResult> Add([FromBody]DiscussionCreate discussion)
        {
            var model = await _discussionService.Add(discussion);
            return Created("api/discussion/" + model.Id, model);
        }

        /// <summary>
        /// get discussion comments by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        [Route("api/discussion/{id}/comments")]
        [HttpGet]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PagedResult<Comment, DiscussionDetailComment>), 200)]
        public async Task<IActionResult> Comments([FromRoute] int id, [FromQuery]CommentDataFilter search = null)
        {
            return Ok(await _discussionService.GetAllCommentsByDiscussionId(id, search));
        }

        /// <summary>
        /// get discussion by search filters
        /// [TODO] update angular client to use discussion.id
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [Route("api/discussion/comments")]
        [HttpGet]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PagedResult<Comment, DiscussionDetailComment>), 200)]
        public async Task<IActionResult> Comments([FromQuery]DiscussionDataFilter search)
        {
            return Ok(await _discussionService.GetAll(search));
        }

        /// <summary>
        /// get discussion by content id and discussion type
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [Route("api/content/{id}/discussion/{type}")]
        [HttpGet]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(DiscussionDetail[]), 200)]
        public async Task<IActionResult> GetContentDiscussionByType([FromRoute]int id, [FromRoute]DiscussionType type)
        {
            return Ok((await _discussionService.GetByContentIdAndDiscussionType(type, id)).ToArray());
        }

        /// <summary>
        /// add comment to discussion by id
        /// </summary>
        /// <param name="id">discussion id</param>
        /// <param name="comment"></param>
        /// <returns></returns>
        [Route("api/discussion/{id}/comments")]
        [HttpPost]
        [JsonExceptionFilter]
        [ValidateModel]
        [ProducesResponseType(typeof(DiscussionDetailComment), 200)]
        public async Task<IActionResult> AddComment([FromRoute]int id, [FromBody]CommentCreate comment)
        {
            return Ok(await _discussionService.AddComment(id, comment));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}

