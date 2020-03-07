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
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Foundry.Communications.Data.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Foundry.Communications.Hubs
{
    /// <summary>
    /// notification hub
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class NotificationHub : Hub
    {
        INotificationRepository _notificationRepository;
        CancellationToken _cancellationToken;

        /// <summary>
        /// create an instance of notification hub
        /// </summary>
        /// <param name="notificationRepository"></param>
        public NotificationHub(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
            _cancellationToken = new CancellationTokenSource().Token;
        }

        /// <summary>
        /// get notification history by globalId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task History(string id)
        {
            var notifications = await _notificationRepository.GetAll(id).ToListAsync();
            await Clients.Caller.SendAsync("History", notifications);
        }
    }
}
