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
    public class MemberRequestController : Controller
    {
        MemberRequestService MemberRequestService { get; }

        public MemberRequestController(MemberRequestService memberRequestService)
        {
            MemberRequestService = memberRequestService ?? throw new ArgumentNullException(nameof(memberRequestService));
        }

        /// <summary>
        /// get all member requests by parent id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet("api/group/{id}/member-requests")]
        [ProducesResponseType(typeof(PagedResult<MemberRequest, MemberRequestDetail>), 200)]
        [JsonExceptionFilter]
        public async Task<IActionResult> GetAllByGroupId([FromRoute]string id, [FromQuery]MemberRequestDataFilter search = null)
        {
            return Ok(await MemberRequestService.GetAllByGroupId(id, search ?? new MemberRequestDataFilter()));
        }

        /// <summary>
        /// add member request
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("api/group/{id}/member-requests")]
        [ProducesResponseType(typeof(MemberRequestDetail), 200)]
        [JsonExceptionFilter]
        public async Task<IActionResult> Add([FromRoute]string id, [FromBody]MemberRequestCreate model)
        {
            if (!id.Equals(model.GroupId, StringComparison.InvariantCultureIgnoreCase))
                throw new InvalidModelException("Group Id mismatch");

            return Ok(await MemberRequestService.Add(model));
        }

        /// <summary>
        /// update member request
        /// </summary>
        /// <param name="id"></param>
        /// <param name="accountId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("api/group/{id}/member-request/{accountId}")]
        [ProducesResponseType(typeof(MemberRequestDetail), 200)]
        [JsonExceptionFilter]
        public async Task<IActionResult> Update([FromRoute]string id, [FromRoute]string accountId, [FromBody]MemberRequestUpdate model)
        {
            if (!id.Equals(model.GroupId, StringComparison.InvariantCultureIgnoreCase))
                throw new InvalidModelException("Group Id mismatch");

            if (!accountId.Equals(model.AccountId, StringComparison.InvariantCultureIgnoreCase))
                throw new InvalidModelException("Account Id mismatch");

            return Ok(await MemberRequestService.Update(model));
        }


        /// <summary>
        /// remove member request
        /// </summary>
        /// <param name="id"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpDelete("api/group/{id}/member-request/{accountId}")]
        [ProducesResponseType(typeof(MemberRequestDetail), 200)]
        [JsonExceptionFilter]
        public async Task<IActionResult> Delete([FromRoute]string id, [FromRoute]string accountId)
        {
            var result = await MemberRequestService.Delete(new MemberRequestDelete { GroupId = id, AccountId = accountId });
            return Ok(result);
        }
    }
}

