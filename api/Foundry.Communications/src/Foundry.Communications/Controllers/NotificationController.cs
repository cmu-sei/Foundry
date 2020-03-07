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
using Foundry.Communications.Data.Entities;
using Foundry.Communications.Services;
using Foundry.Communications.ViewModels;
using Stack.Http.Attributes;
using Stack.Http.Identity;
using Stack.Patterns.Service.Models;
using System;
using System.Threading.Tasks;

namespace Foundry.Communications.Controllers
{
    /// <summary>
    /// notification controller
    /// </summary>
    [Authorize]
    public class NotificationController : StackController
    {
        NotificationService NotificationService { get; }

        /// <summary>
        /// create an instance of the notification controller
        /// </summary>
        /// <param name="stackIdentityResolver"></param>
        /// <param name="notificationService"></param>
        public NotificationController(IStackIdentityResolver stackIdentityResolver, NotificationService notificationService)
            : base(stackIdentityResolver)
        {
            NotificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        /// <summary>
        /// get all notifications
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet("api/notifications")]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(PagedResult<Notification, NotificationSummary>), 200)]
        public async Task<IActionResult> GetAll([FromQuery]NotificationDataFilter search = null)
        {
            return Ok(await NotificationService.GetAll(search));
        }

        /// <summary>
        /// add notification
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("api/notifications")]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> Add([FromBody]NotificationCreate model)
        {
            return Ok(await NotificationService.Add(model));
        }

        /// <summary>
        /// delete notification
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("api/notification/{id}")]
        [JsonExceptionFilter]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            return Ok(await NotificationService.DeleteById(id));
        }

        /// <summary>
        /// delete all notifications by id
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete("api/notifications")]
        [JsonExceptionFilter]
        public async Task<IActionResult> DeleteAll([FromBody]int[] ids)
        {
            return Ok(await NotificationService.DeleteAllById(ids));
        }

        /// <summary>
        /// mark notification as read
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("api/notification/{id}/read")]
        [JsonExceptionFilter]
        public async Task<IActionResult> MarkAsRead([FromRoute]int id)
        {
            return Ok(await NotificationService.MarkAsRead(id));
        }

        /// <summary>
        /// mark notification as unread
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("api/notification/{id}/unread")]
        [JsonExceptionFilter]
        public async Task<IActionResult> MarkAsUnread([FromRoute]int id)
        {
            return Ok(await NotificationService.MarkAsUnread(id));
        }

        /// <summary>
        /// get notification count
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/notifications/count/unread")]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(int), 200)]
        public async Task<IActionResult> Count()
        {
            return Ok(await NotificationService.GetUnreadCount());
        }

        /// <summary>
        /// clean up excessive notifications for user
        /// </summary>
        /// <returns></returns>
        [HttpDelete("api/notifications/clean")]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(int), 200)]
        public async Task<IActionResult> CleanUp()
        {
            return Ok(await NotificationService.DeleteAllReadByIdentity());
        }
    }
}
