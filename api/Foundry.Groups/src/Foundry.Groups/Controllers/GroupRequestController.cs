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
using Foundry.Groups.Data;
using Foundry.Groups.Services;
using Foundry.Groups.ViewModels;
using Stack.Http.Attributes;
using Stack.Http.Exceptions;
using Stack.Http.Identity.Attributes;
using Stack.Patterns.Service.Models;
using System;
using System.Threading.Tasks;

namespace Foundry.Groups.Controllers
{
    [StackAuthorize]
    public class GroupRequestController : Controller
    {
        GroupRequestService GroupRequestService { get; }

        public GroupRequestController(GroupRequestService groupRequestService)
        {
            GroupRequestService = groupRequestService ?? throw new ArgumentNullException(nameof(groupRequestService));
        }

        /// <summary>
        /// get all group requests by parent id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet("api/group/{id}/group-requests")]
        [ProducesResponseType(typeof(PagedResult<GroupRequest, GroupRequestDetail>), 200)]
        [JsonExceptionFilter]
        public async Task<IActionResult> GetAllByParentId([FromRoute]string id, [FromQuery]GroupRequestDataFilter search = null)
        {
            return Ok(await GroupRequestService.GetAllByParentId(id, search ?? new GroupRequestDataFilter()));
        }

        /// <summary>
        /// add group request
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("api/group/{id}/group-requests")]
        [ProducesResponseType(typeof(GroupRequestDetail), 200)]
        [JsonExceptionFilter]
        public async Task<IActionResult> Add([FromRoute]string id, [FromBody]GroupRequestCreate model)
        {
            if (!id.Equals(model.ParentGroupId, StringComparison.InvariantCultureIgnoreCase))
                throw new InvalidModelException("Parent Id mismatch");

            return Ok(await GroupRequestService.Add(model));
        }

        /// <summary>
        /// update group request
        /// </summary>
        /// <param name="id"></param>
        /// <param name="childId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("api/group/{id}/group-request/{childId}")]
        [ProducesResponseType(typeof(GroupRequestDetail), 200)]
        [JsonExceptionFilter]
        public async Task<IActionResult> UpdateGroupRequest([FromRoute]string id, [FromRoute]string childId, [FromBody]GroupRequestUpdate model)
        {
            if (!id.Equals(model.ParentGroupId, StringComparison.InvariantCultureIgnoreCase))
                throw new InvalidModelException("Parent Id mismatch");

            if (!childId.Equals(model.ChildGroupId, StringComparison.InvariantCultureIgnoreCase))
                throw new InvalidModelException("Child Id mismatch");

            return Ok(await GroupRequestService.Update(model));
        }

        /// <summary>
        /// remove group request
        /// </summary>
        /// <param name="id"></param>
        /// <param name="childId"></param>
        /// <returns></returns>
        [HttpDelete("api/group/{id}/group-request/{childId}")]
        [ProducesResponseType(typeof(GroupRequestDetail), 200)]
        [JsonExceptionFilter]
        public async Task<IActionResult> Delete([FromRoute]string id, [FromRoute]string childId)
        {
            var result = await GroupRequestService.Delete(new GroupRequestDelete { ParentGroupId = id, ChildGroupId = childId });
            return Ok(result);
        }
    }
}

