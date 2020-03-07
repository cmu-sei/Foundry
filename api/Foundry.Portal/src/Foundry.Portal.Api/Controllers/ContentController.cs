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
using Foundry.Portal.Data;
using Foundry.Portal.Data.Entities;
using Foundry.Portal.Services;
using Foundry.Portal.ViewModels;
using Stack.Http.Attributes;
using Stack.Http.Identity.Attributes;
using Stack.Patterns.Service.Models;
using Stack.Validation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Portal.Api.Controllers
{
    /// <summary>
    /// content api endpoints
    /// </summary>
    [SecurityHeaders]
    [StackAuthorize]
    public class ContentController : ApiController
    {
        ContentService _contentService;
        DiscussionService _discussionService;
        TagService _tagService;

        /// <summary>
        /// creates an instance of the ContentController
        /// </summary>
        /// <param name="options"></param>
        /// <param name="mill"></param>
        /// <param name="contentService"></param>
        /// <param name="discussionService"></param>
        /// <param name="tagService"></param>
        public ContentController(CoreOptions options, ILoggerFactory mill, ContentService contentService, DiscussionService discussionService, TagService tagService)
            : base(options, mill)
        {
            _contentService = contentService ?? throw new ArgumentNullException("contentService");
            _discussionService = discussionService ?? throw new ArgumentNullException("discussionService");
            _tagService = tagService ?? throw new ArgumentNullException("tagService");
        }

        /// <summary>
        /// get all content
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [Route("api/contents")]
        [HttpGet]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PagedResult<Content, ContentSummary>), 200)]
        public async Task<IActionResult> GetAll([FromQuery]ContentDataFilter search)
        {
            search = search ?? new ContentDataFilter();
            var result = await _contentService.GetAllAccessibleByProfileId(search);
            return Ok(result);
        }

        /// <summary>
        /// get content by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/content/{id:int}")]
        [HttpGet]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(ContentDetail), 200)]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            var content = await _contentService.GetById(id);
            return Ok(content);
        }

        /// <summary>
        /// get content by global id
        /// </summary>
        /// <param name="globalId"></param>
        /// <returns></returns>
        [Route("api/content/{globalId:guid}")]
        [HttpGet]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(ContentDetail), 200)]
        public async Task<IActionResult> GetByGlobalId([FromRoute]Guid globalId)
        {
            var content = await _contentService.GetByGlobalId(globalId.ToString());
            return Ok(content);
        }

        /// <summary>
        /// update content
        /// </summary>
        /// <param name="id"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [Route("api/content/{id}")]
        [HttpPut]
        [JsonExceptionFilter]
        [ValidateModel]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(ContentDetail), 200)]
        public async Task<IActionResult> Update([FromRoute]int id, [FromBody]ContentUpdate content)
        {
            return Ok(await _contentService.Update(content));
        }

        /// <summary>
        /// disable content
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [Route("api/contents/disable")]
        [HttpPut]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> Disable([FromBody]int[] ids)
        {
            return Ok(await _contentService.Disable(ids));
        }

        /// <summary>
        /// enable content
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [Route("api/contents/enable")]
        [HttpPut]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> Enable([FromBody]int[] ids)
        {
            return Ok(await _contentService.Enable(ids));
        }

        /// <summary>
        /// update sponsor for content
        /// </summary>
        /// <param name="sponsorId"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        [Route("api/sponsor/{sponsorId}/contents")]
        [HttpPut]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> UpdateSponsor(string sponsorId, [FromBody]int[] ids)
        {
            return Ok(await _contentService.UpdateSponsor(sponsorId, ids));
        }

        /// <summary>
        /// update author for content
        /// </summary>
        /// <param name="authorId"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        [Route("api/author/{authorId}/contents")]
        [HttpPut]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> UpdateAuthor(int authorId, [FromBody]int[] ids)
        {
            return Ok(await _contentService.UpdateAuthor(authorId, ids));
        }

        /// <summary>
        /// add content
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        [Route("api/contents")]
        [HttpPost]
        [JsonExceptionFilter]
        [ValidateModel]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(ContentDetail), 201)]
        public async Task<IActionResult> Add([FromBody]ContentCreate content)
        {
            var model = await _contentService.Add(content);
            return Created("api/content/" + model.Id, model);
        }

        /// <summary>
        /// add bookmark for content
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/content/{id}/bookmark")]
        [HttpPost]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> AddBookmark([FromRoute]int id)
        {
            await _contentService.AddBookmark(id);
            return Ok(true);
        }

        /// <summary>
        /// delete bookmark for content
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/content/{id}/bookmark")]
        [HttpDelete]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> RemoveBookmark([FromRoute]int id)
        {
            await _contentService.RemoveBookmark(id);
            return Ok(false);
        }

        /// <summary>
        /// add a collection of content to a channel
        /// </summary>
        /// <param name="id"></param>
        /// <param name="contentItems"></param>
        /// <returns></returns>
        [Route("api/channel/{id}/contents")]
        [HttpPost]
        [JsonExceptionFilter]
        [ValidateModel]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(int), 200)]
        public async Task<IActionResult> AddRange([FromRoute] int id, [FromBody]List<ContentCreate> contentItems)
        {
            return Ok(await _contentService.SaveRangeAsync(id, contentItems));
        }

        /// <summary>
        /// update specific fields
        /// </summary>
        /// <param name="id"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [Route("api/content/{id}")]
        [HttpPatch]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(ContentSummary), 200)]
        public async Task<IActionResult> Patch([FromRoute]int id, [FromBody]ContentPatch content)
        {
            return Ok(await _contentService.Patch(id, content));
        }

        /// <summary>
        /// launch content
        /// </summary>
        /// <param name="contentGlobalId"></param>
        /// <param name="profileGlobalId"></param>
        /// <returns></returns>
        [Route("api/launch/{contentGlobalId}/{profileGlobalId}")]
        [AllowAnonymous]
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Launch([FromRoute]string contentGlobalId, [FromRoute]string profileGlobalId)
        {
            var model = await _contentService.Engage(contentGlobalId, profileGlobalId);

            if (model == null)
                return NotFound("Content was not found");

            if (string.IsNullOrWhiteSpace(model.LaunchUrl))
                return BadRequest("Content does not have a url");

            var content = await _contentService.GetByGlobalId(contentGlobalId);

            string qs = content.Url.Contains("?") ? "&" : "?";

            string url = string.Format("{0}{1}auth-hint=nextstep&content={2}&profile={3}", content.Url, qs, contentGlobalId, profileGlobalId);

            return Json(new { Url = url });
        }

        /// <summary>
        /// add an array of tag strings to content
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        [Route("api/content/{id}/tags")]
        [HttpPost]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> UpdateTags([FromRoute]int id, [FromBody] string[] tags)
        {
            var model = await _tagService.SetContentTags(id, tags);
            return Ok(model);
        }

        /// <summary>
        /// delete content by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/content/{id:int}")]
        [HttpDelete]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            await _contentService.DeleteById(id);

            return Ok();
        }

        /// <summary>
        /// delete content by global id
        /// </summary>
        /// <param name="globalId"></param>
        /// <returns></returns>
        [Route("api/content/{globalId:guid}")]
        [HttpDelete]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> DeleteByGlobalId([FromRoute]Guid globalId)
        {
            await _contentService.DeleteByGlobalId(globalId.ToString());
            return Ok();
        }

        /// <summary>
        /// delete content by global ids
        /// </summary>
        /// <param name="globalIds"></param>
        /// <returns></returns>
        [Route("api/contents")]
        [HttpDelete]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> DeleteByGlobalIds([FromBody]List<Guid> globalIds)
        {
            await _contentService.DeleteByGlobalIds(globalIds.Select(id => id.ToString()).ToList());
            return Ok();
        }

        /// <summary>
        /// rate content
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rating"></param>
        /// <returns></returns>
        [Route("api/content/{id}/rate/{rating}")]
        [HttpPost]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(RatingMetricDetail), 200)]
        public async Task<IActionResult> SetRating([FromRoute] int id, [FromRoute] Rating rating)
        {
            var model = await _contentService.AddOrUpdateRating(id, rating);
            return Ok(model.Rating);
        }

        /// <summary>
        /// rate contents difficulty
        /// </summary>
        /// <param name="id"></param>
        /// <param name="difficulty"></param>
        /// <returns></returns>
        [Route("api/content/{id}/difficulty/{difficulty}")]
        [HttpPost]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(DifficultyMetricDetail), 200)]
        public async Task<IActionResult> SetDifficulty([FromRoute]int id, [FromRoute] Difficulty difficulty)
        {
            var model = await _contentService.AddOrUpdateDifficulty(id, difficulty);
            return Ok(model.Difficulty);
        }

        /// <summary>
        /// get all content comments by comment id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        [Route("api/content/{id}/comments")]
        [HttpGet]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(PagedResult<Comment, DiscussionDetailComment>), 200)]
        public async Task<IActionResult> Comments([FromRoute]int id, [FromQuery] CommentDataFilter search = null)
        {
            return Ok(await _discussionService.GetAllCommentsByContentId(id, search));
        }

        /// <summary>
        /// add a review comment for content by content id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        [Route("api/content/{id}/review")]
        [HttpPost]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(DiscussionDetailComment), 200)]
        public async Task<IActionResult> AddContentReviewComment([FromRoute] int id, [FromBody] string text)
        {
            var model = await _discussionService.AddContentComment(id, DiscussionType.ContentReview, text);
            return Created("api/comment/" + model.Id, model);
        }

        /// <summary>
        /// flag a content for review
        /// </summary>
        /// <param name="id"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        [Route("api/content/{id}/flag")]
        [HttpPost]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(string), 202)]
        public async Task<IActionResult> AddFlag([FromRoute] int id, [FromBody] string comment)
        {
            await _contentService.AddFlag(id, comment);
            return Accepted();
        }

        /// <summary>
        /// flag a content for review
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/content/{id}/flag")]
        [HttpDelete]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> RemoveFlag([FromRoute] int id)
        {
            await _contentService.RemoveFlag(id);
            return Accepted();
        }

        /// <summary>
        /// reject flag
        /// </summary>
        /// <param name="id"></param>
        /// <param name="profileId"></param>
        /// <returns></returns>
        [Route("api/content/{id}/flag/{profileId}")]
        [HttpDelete]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> RejectFlag([FromRoute] int id, [FromRoute] int profileId)
        {
            await _contentService.RejectFlag(id, profileId);
            return Ok(true);
        }

        /// <summary>
        /// accept flag
        /// </summary>
        /// <param name="id"></param>
        /// <param name="profileId"></param>
        /// <returns></returns>
        [Route("api/content/{id}/flag/{profileId}")]
        [HttpPost]
        [JsonExceptionFilter]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> AccepFlag([FromRoute] int id, [FromRoute] int profileId)
        {
            await _contentService.AcceptFlag(id, profileId);
            return Ok(true);
        }

        /// <summary>
        /// get content types
        /// </summary>
        /// <returns></returns>
        [Route("api/contenttypes")]
        [HttpGet]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(string[]), 200)]
        public IActionResult GetAllContentTypes()
        {
            var values = Enum.GetNames(typeof(ContentType)).Where(ct => ct != "NotSet").OrderBy(ct => ct);

            return Ok(values);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}

