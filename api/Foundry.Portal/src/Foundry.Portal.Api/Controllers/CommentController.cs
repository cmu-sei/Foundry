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
using Foundry.Portal.Services;
using Foundry.Portal.ViewModels;
using Stack.Http.Attributes;
using Stack.Http.Identity.Attributes;
using Stack.Validation.Attributes;
using System;
using System.Threading.Tasks;

namespace Foundry.Portal.Api.Controllers
{
    /// <summary>
    /// comment api endpoints
    /// </summary>
    [SecurityHeaders]
    [StackAuthorize]
    public class CommentController : ApiController
    {
        DiscussionService _discussionService;

        /// <summary>
        /// creates an instance of the CommentController
        /// </summary>
        /// <param name="options"></param>
        /// <param name="mill"></param>
        /// <param name="discussionService"></param>
        public CommentController(CoreOptions options, ILoggerFactory mill, DiscussionService discussionService)
            : base(options, mill)
        {
            _discussionService = discussionService ?? throw new ArgumentNullException("discussionService");
        }

        /// <summary>
        /// update comment by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        [Route("api/comment/{id}")]
        [HttpPut]
        [JsonExceptionFilter]
        [ValidateModel]
        [ProducesResponseType(typeof(DiscussionDetailComment), 200)]
        public async Task<IActionResult> UpdateComment([FromRoute] int id, [FromBody]CommentUpdate comment)
        {
            if (id != comment.Id)
                throw new InvalidOperationException();

            return Ok(await _discussionService.UpdateComment(comment));
        }

        /// <summary>
        /// up vote a comment by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/comment/{id}/up")]
        [HttpPost]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(DiscussionDetailComment), 200)]
        public async Task<IActionResult> UpVote([FromRoute] int id)
        {
            return Ok(await _discussionService.UpVote(id));
        }

        /// <summary>
        /// down vote a comment by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/comment/{id}/down")]
        [HttpPost]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(DiscussionDetailComment), 200)]
        public async Task<IActionResult> DownVote([FromRoute] int id)
        {
            return Ok(await _discussionService.DownVote(id));
        }

        /// <summary>
        ///  delete a comment by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/comment/{id}")]
        [HttpDelete]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            await _discussionService.DeleteComment(id);
            return Ok(true);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}

