/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using System;
using System.Collections.Generic;

namespace Stack.Communication.Notifications
{
    public class NotificationSummary
    {
        /// <summary>
        /// the id of the notification
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// the subject of the notification
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// the body of the notification
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// the url related to the notification
        /// </summary>
        public string Url { get; set; }

        public List<NotificationSummaryValue> Values { get; set; } = new List<NotificationSummaryValue>();

        /// <summary>
        /// the global id object
        /// </summary>
        public string GlobalId { get; set; }

        /// <summary>
        /// the priority of the notification
        /// </summary>
        public Priority Priority { get; set; }

        /// <summary>
        /// the date the notification was created
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// the date the notification was read
        /// </summary>
        public DateTime? Read { get; set; }

        /// <summary>
        /// pulled from message source logoUrl
        /// </summary>
        public string LogoUrl { get; set; }

        /// <summary>
        /// the source application that created the notification
        /// </summary>
        public string Source { get; set; }
    }
}
