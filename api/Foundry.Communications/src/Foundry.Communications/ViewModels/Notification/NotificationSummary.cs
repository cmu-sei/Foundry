/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Foundry.Communications.Data.Entities;
using System;
using System.Collections.Generic;

namespace Foundry.Communications.ViewModels
{
    public class NotificationSummary
    {
        public int Id { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string Url { get; set; }

        public List<NotificationSummaryValue> Values { get; set; } = new List<NotificationSummaryValue>();

        public string GlobalId { get; set; }

        public Priority Priority { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Read { get; set; }

        /// <summary>
        /// pulled from message source logoUrl
        /// </summary>
        public string LogoUrl { get; set; }

        public string Source { get; set; }
    }
}
