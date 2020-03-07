/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.EntityFrameworkCore;
using Foundry.Communications.Data;
using Foundry.Communications.Data.Entities;
using Foundry.Communications.Data.Repositories;
using Stack.Patterns.Repository;
using System.Linq;

namespace Foundry.Communications.Repositories
{
    /// <summary>
    /// notification repository
    /// </summary>
    public class NotificationRepository : Repository<CommunicationDbContext, Notification>, INotificationRepository
    {
        /// <summary>
        /// create an instance of notification repository
        /// </summary>
        /// <param name="db"></param>
        public NotificationRepository(CommunicationDbContext db)
            : base(db) { }

        /// <summary>
        /// get all notifications for targetId
        /// </summary>
        /// <param name="targetId"></param>
        /// <returns></returns>
        public IQueryable<Notification> GetAll(string targetId)
        {
            var ids = DbContext.Recipients
                .Where(r => r.TargetId.ToLower() == targetId.ToLower() && !r.Deleted.HasValue)
                .Select(r => r.NotificationId).ToArray();

            return DbContext.Notifications
                .Include(n => n.Values)
                .Include(n => n.Recipients)
                .Where(n => ids.Contains(n.Id) && !n.Deleted.HasValue);
        }

        /// <summary>
        /// get all receipients for notification by targetId
        /// </summary>
        /// <param name="id"></param>
        /// <param name="targetId"></param>
        /// <returns></returns>
        public IQueryable<Recipient> GetRecipientsForNotification(int id, string targetId)
        {
            return DbContext.Recipients.Where(r => r.NotificationId == id && r.TargetId.ToLower() == targetId.ToLower());
        }
    }
}
