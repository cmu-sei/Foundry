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
using Stack.Http.Exceptions;
using Stack.Http.Identity.Attributes;
using Stack.Patterns.Service.Models;
using Stack.Validation.Attributes;
using System;
using System.Threading.Tasks;

namespace Foundry.Portal.Api.Controllers
{
    /// <summary>
    /// post api
    /// </summary>
    [SecurityHeaders]
    [StackAuthorize]
    public class PostController : ApiController
    {
        PostService _postService;

        /// <summary>
        /// creates an instance of the PostController
        /// </summary>
        /// <param name="options"></param>
        /// <param name="mill"></param>
        /// <param name="postService"></param>
        public PostController(CoreOptions options, ILoggerFactory mill, PostService postService)
            : base(options, mill)
        {
            _postService = postService ?? throw new ArgumentNullException(nameof(postService));
        }

        /// <summary>
        /// add post
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("api/posts")]
        [HttpPost]
        [JsonExceptionFilter]
        [ValidateModel]
        [ProducesResponseType(typeof(PostDetail), 201)]
        public async Task<IActionResult> Add([FromBody]PostCreate model)
        {
            var saved = await _postService.Add(model);
            return Created("api/post/" + saved.Id, saved);
        }

        /// <summary>
        /// update post
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("api/post/{id}")]
        [HttpPost]
        [JsonExceptionFilter]
        [ValidateModel]
        [ProducesResponseType(typeof(PostDetail), 200)]
        public async Task<IActionResult> Update([FromRoute]int id, [FromBody]PostUpdate model)
        {
            if (id != model.Id)
                throw new InvalidIdentityException("Invalid identity.");

            return Ok(await _postService.Update(model));
        }

        /// <summary>
        /// delete post
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/post/{id}")]
        [HttpDelete]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            return Ok(await _postService.Delete(id));
        }

        /// <summary>
        /// up vote post
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/post/{id}/up")]
        [HttpPost]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PostVoteMetric), 201)]
        public async Task<IActionResult> UpVote([FromRoute]int id)
        {
            var value = await _postService.UpVote(id);
            return Ok(value);
        }

        /// <summary>
        /// down vote post
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/post/{id}/down")]
        [HttpPost]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PostVoteMetric), 201)]
        public async Task<IActionResult> DownVote([FromRoute]int id)
        {
            var value = await _postService.DownVote(id);
            return Ok(value);
        }

        /// <summary>
        /// get post by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/post/{id}")]
        [HttpGet]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PostDetail), 200)]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            return Ok(await _postService.GetById(id));
        }

        /// <summary>
        /// get all posts
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [Route("api/posts")]
        [HttpGet]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PagedResult<Post, PostDetail>), 200)]
        public async Task<IActionResult> GetAll([FromQuery]PostDataFilter search = null)
        {
            return Ok(await _postService.GetAll(search));
        }

        /// <summary>
        /// get all post replies by parent post id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        [Route("api/post/{id}/replies")]
        [HttpGet]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PagedResult<Post, PostDetail>), 200)]
        public async Task<IActionResult> GetAllReplies([FromRoute]int id, [FromQuery]PostDataFilter search = null)
        {
            search = search ?? new PostDataFilter();

            search.Filter = string.IsNullOrWhiteSpace(search.Filter)
                ? "parent=" + id
                : "|parent=" + id;

            return Ok(await _postService.GetAll(search));
        }

        /// <summary>
        /// get all posts by profile id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        [Route("api/profile/{id}/posts")]
        [HttpGet]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PagedResult<Post, PostDetail>), 200)]
        public async Task<IActionResult> GetAllByProfileId([FromRoute]int id, [FromQuery]PostDataFilter search = null)
        {
            search = search ?? new PostDataFilter();

            search.Filter = string.IsNullOrWhiteSpace(search.Filter)
                ? "profile=" + id
                : "|profile=" + id;

            return Ok(await _postService.GetAll(search));
        }
    }
}

