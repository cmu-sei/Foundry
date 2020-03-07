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
using Stack.Communication.Notifications;
using Stack.DomainEvents;
using Foundry.Groups.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Groups.Notifications
{
    public class MemberRequestNotification : NotificationCreateStrategy
    {
        string _label;

        public MemberRequestNotification(CommunicationOptions communicationOptions, IDomainEvent domainEvent)
            : base(communicationOptions, domainEvent) { }

        public override async Task<NotificationCreate> GetModel()
        {
            var model = DomainEvent.Entity as MemberRequestNotificationModel;

            _label = model.Label;

            var notification = new NotificationCreate
            {
                GlobalId = DomainEvent.Id,
                Subject = model.Subject,
                Body = model.Body,
                Values = ToNotificationCreateValues(),
                Url = ToLocalUrl("group", model.GroupId, model.GroupSlug),
                Recipients = model.Recipients.ToArray()
            };

            return notification;
        }

        public string ToLocalUrl(string prefix, string id, string slug)
        {
            return string.Format("{0}/{1}/{2}/{3}", CommunicationOptions.ClientUrl, prefix, id, slug);
        }

        public override string Label { get { return _label; } }

        public override string Type => "memberrequest";
    }
}
