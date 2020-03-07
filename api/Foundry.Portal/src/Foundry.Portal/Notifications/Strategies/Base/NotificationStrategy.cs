/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Foundry.Portal.Data;
using Foundry.Portal.Events;
using Foundry.Portal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Portal.Notifications.Strategies
{
    public abstract class NotificationStrategy : INotificationStrategy
    {
        public CommunicationOptions CommunicationOptions { get; set; }
        public SketchDbContext DbContext { get; set; }
        public IDomainEvent DomainEvent { get; set; }
        public abstract DomainEventType DomainEventType { get; }
        public string Label
        {
            get
            {
                string value = DomainEventType.ToString().ToLower();

                if (value.Contains("add") ||
                    value.Contains("follow") ||
                    value.Contains("join") ||
                    value.Contains("request") ||
                    value.Contains("accept") ||
                    value.Contains("admit"))
                    return "add";

                if (value.Contains("flagged"))
                    return "warning";

                if (value.Contains("delete") ||
                    value.Contains("unfollow") ||
                    value.Contains("leave"))
                    return "delete";

                if (value.Contains("update") ||
                    value.Contains("rate") ||
                    value.Contains("level"))
                    return "update";

                if (value.Contains("post"))
                    return "post";

                if (value.Contains("permissiongranted") ||
                    value.Contains("system"))
                    return "system";

                return "-";
            }
        }
        public abstract Task<NotificationCreate> Build();

        protected const SystemPermissions PowerUser = SystemPermissions.PowerUser;
        protected const ChannelPermission ChannelOwner = ChannelPermission.Owner;

        protected string GlobalId
        {
            get { return DomainEvent.Id.ToString().ToLower(); }
        }

        protected string ParentId
        {
            get { return (DomainEvent.ParentId ?? Guid.Empty).ToString().ToLower(); ; }
        }

        readonly DomainEventType[] content = new DomainEventType[] { DomainEventType.ContentAdd, DomainEventType.ContentFlagged, DomainEventType.ContentLevel, DomainEventType.ContentRate, DomainEventType.ContentUpdate };

        readonly DomainEventType[] playlist = new DomainEventType[] { DomainEventType.PlaylistAdd, DomainEventType.PlaylistContentAdd, DomainEventType.PlaylistContentDelete, DomainEventType.PlaylistFollow, DomainEventType.PlaylistGroupFollow, DomainEventType.PlaylistGroupUnfollow, DomainEventType.PlaylistRate, DomainEventType.PlaylistUnfollow, DomainEventType.PlaylistUpdate };

        protected IEnumerable<string> PowerUsers
        {
            get
            {
                return DbContext.Profiles
                    .Where(p => (p.Permissions & PowerUser) == PowerUser)
                    .Select(p => p.GlobalId).ToList().Distinct();
            }
        }

        public string ToLocalUrl(string prefix, int id, string slug)
        {
            return string.Format("{0}/{1}/{2}/{3}", CommunicationOptions.SourceUrl, prefix, id, slug);
        }

        public List<NotificationCreateValue> ToValues()
        {
            return new List<NotificationCreateValue>() {
                new NotificationCreateValue { Key = "type", Value = DomainEventType.ToString() },
                new NotificationCreateValue { Key = "label", Value = Label }
            };
        }
    }
}
