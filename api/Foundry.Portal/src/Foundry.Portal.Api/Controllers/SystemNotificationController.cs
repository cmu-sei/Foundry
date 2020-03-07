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
using Foundry.Portal.Notifications;
using Foundry.Portal.ViewModels;
using Stack.Http.Attributes;
using Stack.Http.Identity;
using Stack.Http.Identity.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Portal.Api.Controllers
{
    /// <summary>
    /// setting controller
    /// </summary>
    [SecurityHeaders]
    [StackAuthorize]
    public class SystemNotificationController : ApiController
    {
        INotificationHandler _notificationHandler;
        IStackIdentityResolver _identityResolver;
        SketchDbContext _db;

        /// <summary>
        /// create an instance of system notification controller
        /// </summary>
        /// <param name="db"></param>
        /// <param name="identityResolver"></param>
        /// <param name="notificationHandler"></param>
        /// <param name="options"></param>
        /// <param name="mill"></param>
        public SystemNotificationController(SketchDbContext db, IStackIdentityResolver identityResolver, INotificationHandler notificationHandler, CoreOptions options, ILoggerFactory mill)
            : base(options, mill)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _notificationHandler = notificationHandler ?? throw new ArgumentNullException(nameof(notificationHandler));
            _identityResolver = identityResolver ?? throw new ArgumentNullException(nameof(identityResolver));
        }

        /// <summary>
        /// add system notification
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("api/system-notifications")]
        [HttpPost]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(bool), 200)]
        [StackAuthorize(StackAuthorizeType.RequireAll, "Administrator")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Add([FromBody]SystemNotificationCreate model)
        {
            var identity = await _identityResolver.GetIdentityAsync();

            if (!identity.Permissions.Contains("administrator"))
                throw new UnauthorizedAccessException();

            var recipients = _db.Profiles.Select(p => p.GlobalId).ToArray();

            var values = new List<NotificationCreateValue>() {
                new NotificationCreateValue { Key = "type", Value = DomainEventType.System.ToString() },
                new NotificationCreateValue { Key = "label", Value = "system" } };

            var notification = new NotificationCreate
            {
                Body = model.Body,
                Values = values,
                Subject = model.Subject,
                Priority = Priority.System,
                GlobalId = Guid.NewGuid().ToString().ToLower(),
                Recipients = recipients
            };

            return Ok(await _notificationHandler.PostNotificationAsync(notification));
        }
    }
}

