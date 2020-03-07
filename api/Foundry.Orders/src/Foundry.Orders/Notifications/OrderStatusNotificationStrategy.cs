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
using Foundry.Orders.Data.Entities;
using Stack.Communication.Notifications;
using Stack.DomainEvents;
using System.Threading.Tasks;

namespace Foundry.Orders
{
    public class OrderStatusNotificationStrategy : NotificationCreateStrategy
    {
        public OrderStatusNotificationStrategy(CommunicationOptions communicationOptions, IDomainEvent domainEvent)
            : base(communicationOptions, domainEvent) { }

        public override string Label => "add";

        public override string Type => "orderstatus";

        string ToLocalUrl(string prefix, int id)
        {
            return string.Format("{0}/{1}/{2}", CommunicationOptions.ClientUrl, prefix, id);
        }

        public async override Task<NotificationCreate> GetModel()
        {
            var order = DomainEvent.Entity as Order;

            var creator = order.CreatedBy.GlobalId;

            var notification = new NotificationCreate
            {
                GlobalId = DomainEvent.Id,
                Subject = "Order Status Changed",
                Body = string.Format("Order '{0}' status changed to '{1}'.", DomainEvent.Name.PadLeft(5, '0'), order.Status.ToString()),
                Values = ToNotificationCreateValues(),
                Url = ToLocalUrl("order", order.Id),
                Recipients = new string[] { creator }
            };

            return notification;
        }
    }
}
