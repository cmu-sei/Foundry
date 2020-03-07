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
using Foundry.Groups.Data;
using Foundry.Groups.Services;
using Foundry.Groups.ViewModels;
using Stack.Http.Attributes;
using Stack.Http.Exceptions;
using Stack.Http.Identity.Attributes;
using Stack.Patterns.Service.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foundry.Groups.Controllers
{
    /// <summary>
    /// group controller
    /// </summary>
    [StackAuthorize]
    public class GroupController : Controller<GroupService>
    {
        /// <summary>
        /// create an instance of group service
        /// </summary>
        /// <param name="groupService"></param>
        /// <param name="logger"></param>
        public GroupController(GroupService groupService, ILogger<GroupController> logger)
            : base(groupService, logger) { }

        /// <summary>
        /// get all groups
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet("api/groups")]
        [ProducesResponseType(typeof(PagedResult<Group, GroupSummary>), 200)]
        [JsonExceptionFilter]
        public async Task<IActionResult> GetAll([FromQuery]GroupDataFilter search = null)
        {
            return Ok(await Service.GetAll(search ?? new GroupDataFilter()));
        }

        /// <summary>
        /// get all groups for account by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet("api/account/{id}/groups")]
        [ProducesResponseType(typeof(GroupDetail), 200)]
        [JsonExceptionFilter]
        public async Task<IActionResult> GetAllGroupsByAccountId([FromRoute] string id, [FromQuery] GroupDataFilter search = null)
        {
            return Ok(await Service.GetAllGroupsByAccountId(id, search));
        }

        /// <summary>
        /// get group by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("api/group/{id}")]
        [ProducesResponseType(typeof(GroupDetail), 200)]
        [JsonExceptionFilter]
        public async Task<IActionResult> GetById([FromRoute] string id)
        {
            return Ok(await Service.GetById(id));
        }

        /// <summary>
        /// get group tree
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/tree")]
        [ProducesResponseType(typeof(List<TreeGroupSummary>), 200)]
        [JsonExceptionFilter]
        [AllowAnonymous]
        public async Task<IActionResult> GetTree()
        {
            return Ok(await Service.GetTree());
        }

        /// <summary>
        /// get children of group
        /// </summary>
        /// <param name="id"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet("api/group/{id}/children")]
        [ProducesResponseType(typeof(PagedResult<Group, GroupSummary>), 200)]
        [JsonExceptionFilter]
        public async Task<IActionResult> GetAllByParentId([FromRoute] string id, [FromQuery]GroupDataFilter search = null)
        {
            return Ok(await Service.GetAllByParentId(id, search ?? new GroupDataFilter()));
        }

        /// <summary>
        /// add group
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("api/groups")]
        [ProducesResponseType(typeof(GroupDetail), 200)]
        [JsonExceptionFilter]
        public async Task<IActionResult> Add([FromBody] GroupCreate model)
        {
            var group = await Service.Add(model);
            return Ok(group);
        }

        /// <summary>
        /// update group
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("api/group/{id}")]
        [ProducesResponseType(typeof(GroupDetail), 200)]
        [JsonExceptionFilter]
        public async Task<IActionResult> Update(string id, [FromBody] GroupUpdate model)
        {
            if (!id.Equals(model.Id, StringComparison.InvariantCultureIgnoreCase))
                throw new InvalidModelException("Id mismatch");

            return Ok(await Service.Update(model));
        }

        /// <summary>
        /// delete group
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("api/group/{id}")]
        [ProducesResponseType(typeof(bool), 200)]
        [JsonExceptionFilter]
        public async Task<IActionResult> Delete(string id)
        {
            return Ok(await Service.DeleteById(id));
        }

        /// <summary>
        /// get group by member invite code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet("api/member-invite/{code}")]
        [ProducesResponseType(typeof(GroupDetail), 200)]
        [JsonExceptionFilter]
        public async Task<IActionResult> GetByMemberInviteCode([FromRoute] string code)
        {
            return Ok(await Service.GetByMemberInviteCode(code));
        }

        /// <summary>
        /// accept member invite by invite code for authenticated user
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPut("api/member-invite/{code}")]
        [ProducesResponseType(typeof(MemberDetail), 200)]
        [JsonExceptionFilter]
        public async Task<IActionResult> AcceptMemberInvite([FromRoute]string code)
        {
            return Ok(await Service.AcceptMemberInvite(code));
        }

        /// <summary>
        /// get member invite code
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("api/group/{id}/member-invite")]
        [ProducesResponseType(typeof(string), 200)]
        [JsonExceptionFilter]
        public async Task<IActionResult> GetMemberInviteCodeById([FromRoute] string id)
        {
            return Ok(await Service.GetMemberInviteCodeById(id));
        }

        /// <summary>
        /// generate new member invite code
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("api/group/{id}/member-invite")]
        [ProducesResponseType(typeof(string), 200)]
        [JsonExceptionFilter]
        public async Task<IActionResult> UpdateMemberInviteCodeById([FromRoute] string id)
        {
            return Ok(await Service.UpdateMemberInviteCodeById(id));
        }

        /// <summary>
        /// delete member invite code
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("api/group/{id}/member-invite")]
        [ProducesResponseType(typeof(bool), 200)]
        [JsonExceptionFilter]
        public async Task<IActionResult> DeleteMemberInviteCodeById([FromRoute] string id)
        {
            return Ok(await Service.DeleteMemberInviteCodeById(id));
        }

        /// <summary>
        /// get group by group invite code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet("api/group-invite/{code}")]
        [ProducesResponseType(typeof(GroupDetail), 200)]
        [JsonExceptionFilter]
        public async Task<IActionResult> GetByGroupInviteCode([FromRoute] string code)
        {
            return Ok(await Service.GetByGroupInviteCode(code));
        }

        /// <summary>
        /// accept group invite by invite code for group id
        /// </summary>
        /// <param name="code"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("api/group-invite/{code}/group/{id}")]
        [ProducesResponseType(typeof(GroupDetail), 200)]
        [JsonExceptionFilter]
        public async Task<IActionResult> AcceptGroupInvite([FromRoute]string code, [FromRoute]string id)
        {
            return Ok(await Service.AcceptGroupInvite(code, id));
        }

        /// <summary>
        /// get group invite code
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("api/group/{id}/group-invite")]
        [ProducesResponseType(typeof(string), 200)]
        [JsonExceptionFilter]
        public async Task<IActionResult> GetGroupInviteCodeById([FromRoute] string id)
        {
            return Ok(await Service.GetGroupInviteCodeById(id));
        }

        /// <summary>
        /// generate new group invite code
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("api/group/{id}/group-invite")]
        [ProducesResponseType(typeof(string), 200)]
        [JsonExceptionFilter]
        public async Task<IActionResult> UpdateGroupInviteCodeById([FromRoute] string id)
        {
            return Ok(await Service.UpdateGroupInviteCodeById(id));
        }

        /// <summary>
        /// delete group invite code
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("api/group/{id}/group-invite")]
        [ProducesResponseType(typeof(bool), 200)]
        [JsonExceptionFilter]
        public async Task<IActionResult> DeleteGroupInviteCodeById([FromRoute] string id)
        {
            return Ok(await Service.DeleteGroupInviteCodeById(id));
        }
    }
}

