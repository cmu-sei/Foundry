/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Stack.Communication.Notifications
{
    public class NotificationCreate
    {
        /// <summary>
        /// the subject of the notification
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// the body of the notification
        /// </summary>
        [Required]
        public string Body { get; set; }

        /// <summary>
        /// the url of the object
        /// </summary>
        public string Url { get; set; }

        public List<NotificationCreateValue> Values { get; set; } = new List<NotificationCreateValue>();

        /// <summary>
        /// the global id of the object
        /// </summary>
        public string GlobalId { get; set; }

        /// <summary>
        /// the priority of the notification
        /// </summary>
        public Priority Priority { get; set; }

        /// <summary>
        /// array of recipient guids for the notification
        /// </summary>
        public string[] Recipients { get; set; }
    }

    public enum Priority
    {
        Normal = 0,
        Elevated = 1,
        High = 2,
        System = 3
    }
}
