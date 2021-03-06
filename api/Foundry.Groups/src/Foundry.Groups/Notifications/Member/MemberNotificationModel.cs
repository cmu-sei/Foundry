/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Foundry.Groups.Data;
using System.Collections.Generic;
using System.Linq;

namespace Foundry.Groups.Notifications
{
    public class MemberNotificationModel
    {
        public string Subject { get; set; }

        public string Body { get; set; }

        public string Label { get; set; }

        public string GroupName { get; set; }

        public string MemberName { get; set; }

        public string GroupId { get; set; }

        public string GroupSlug { get; set; }

        public IEnumerable<string> Recipients { get; set; } = new List<string>();

        public static MemberNotificationModel ToModel(string action, Group group, string accountId, string accountName)
        {
            var subject = "";
            var body = "";
            var label = "";

            var recipients = group.Members.Where(m => m.IsOwner || m.IsManager).Select(m => m.AccountId).ToList();

            switch (action)
            {
                case "add":
                    subject = "Member Added";
                    body = string.Format("'{0}' was added to the group '{1}'.", accountName, group.Name);
                    recipients.Add(accountId);
                    label = "add";
                    break;
                case "delete":
                    subject = "Member Deleted";
                    body = string.Format("'{0}' was deleted from group '{1}'.", accountName, group.Name);
                    recipients.Add(accountId);
                    label = "delete";
                    break;
                case "leave":
                    subject = "Member Left";
                    body = string.Format("'{0}' left the group '{1}'.", accountName, group.Name);
                    recipients.Add(accountId);
                    label = "delete";
                    break;
                case "promote":
                    subject = "Member Promoted";
                    body = string.Format("'{0}' was promoted in the group '{1}'.", accountName, group.Name);
                    recipients.Add(accountId);
                    label = "add";
                    break;
                case "demote":
                    subject = "Member Demoted";
                    body = string.Format("'{0}' was demoted in the group '{1}'.", accountName, group.Name);
                    recipients.Add(accountId);
                    label = "delete";
                    break;
            }

            return new MemberNotificationModel
            {
                Subject = subject,
                Body = body,
                GroupId = group.Id,
                GroupName = group.Name,
                GroupSlug = group.Slug,
                MemberName = accountName,
                Label = label,
                Recipients = recipients
            };
        }
    }

}

