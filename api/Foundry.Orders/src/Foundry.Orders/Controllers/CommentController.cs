/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Stack.Http.Identity;
using Stack.Http.Identity.Attributes;
using Foundry.Orders.Data.Entities;
using Foundry.Orders.Options;
using Foundry.Orders.Services;
using Foundry.Orders.ViewModels;
using Stack.Patterns.Service.Models;
using System;
using System.Threading.Tasks;

namespace Foundry.Orders.Controllers
{
    /// <summary>
    /// comment api endpoints
    /// </summary>
    [SecurityHeaders]
    [StackAuthorize]
    public class CommentController : BaseController
    {
        CommentService _commentService;

        /// <summary>
        /// create comment controller
        /// </summary>
        /// <param name="identityResolver"></param>
        /// <param name="mapper"></param>
        /// <param name="commentService"></param>
        public CommentController(IStackIdentityResolver identityResolver, IMapper mapper, CommentService commentService)
            : base(identityResolver, mapper)
        {
            _commentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
        }

        /// <summary>
        /// get all comments for order
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="dataFilter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/order/{orderId}/comments")]
        [ProducesResponseType(typeof(PagedResult<Comment, CommentDetail>), 200)]
        public async Task<IActionResult> GetAll([FromRoute]int orderId, [FromQuery]CommentDataFilter dataFilter = null)
        {
            return Ok(await _commentService.GetAll(orderId, dataFilter));
        }

        /// <summary>
        /// add comment to order
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/order/{orderId}/comments")]
        [ProducesResponseType(typeof(CommentDetail), 201)]
        public async Task<IActionResult> Post([FromRoute]int orderId, [FromBody]CommentEdit model)
        {
            try
            {
                var result = await _commentService.Add(orderId, model);
                return Created("api/order/" + orderId + "/" + result.Id, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// update comment
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/order/{id}/comment")]
        [ProducesResponseType(typeof(CommentDetail), 200)]
        public async Task<IActionResult> Put([FromRoute]int id, [FromBody]CommentEdit model)
        {
            try
            {
                return Ok(await _commentService.Update(id, model));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

