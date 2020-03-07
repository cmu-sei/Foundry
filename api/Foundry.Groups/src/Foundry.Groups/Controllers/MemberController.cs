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
    /// <summary>
    /// member controller
    /// </summary>
    [StackAuthorize]
    public class MemberController : Controller
    {
        MemberService MemberService { get; }

        /// <summary>
        /// create an instance of member service
        /// </summary>
        /// <param name="memberService"></param>
        public MemberController(MemberService memberService)
        {
            MemberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
        }

        /// <summary>
        /// get all members of a group by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet("api/group/{id}/members")]
        [ProducesResponseType(typeof(PagedResult<Member, MemberSummary>), 200)]
        [JsonExceptionFilter]
        public async Task<IActionResult> GetAllByGroupId([FromRoute]string id, [FromQuery]MemberDataFilter search = null)
        {
            return Ok(await MemberService.GetAllByGroupId(id, search ?? new MemberDataFilter()));
        }

        /// <summary>
        /// add member
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("api/group/{id}/members")]
        [ProducesResponseType(typeof(MemberDetail), 200)]
        [JsonExceptionFilter]
        public async Task<IActionResult> Add([FromRoute]string id, [FromBody]MemberCreate model)
        {
            if (id != model.GroupId)
                throw new InvalidModelException("Id mismatch");

            var member = await MemberService.Add(model);
            return Created(string.Format("~/api/group/{0}/member/{1}", member.GroupId, member.AccountId), member);
        }

        /// <summary>
        /// update member
        /// </summary>
        /// <param name="id"></param>
        /// <param name="accountId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("api/group/{id}/member/{accountId}")]
        [ProducesResponseType(typeof(MemberDetail), 200)]
        [JsonExceptionFilter]
        public async Task<IActionResult> Update([FromRoute]string id, [FromRoute]string accountId, [FromBody]MemberUpdate model)
        {
            if (id != model.GroupId)
                throw new InvalidModelException("Group Id mismatch");

            if (accountId != model.AccountId)
                throw new InvalidModelException("Account Id mismatch");

            var member = await MemberService.Update(model);
            return Ok(member);
        }

        /// <summary>
        /// remove member
        /// </summary>
        /// <param name="id"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpDelete("api/group/{id}/member/{accountId}")]
        [ProducesResponseType(typeof(MemberDetail), 200)]
        [JsonExceptionFilter]
        public async Task<IActionResult> Delete([FromRoute]string id, [FromRoute]string accountId)
        {
            var result = await MemberService.Delete(new MemberDelete { GroupId = id, AccountId = accountId });
            return Ok(result);
        }
    }
}

