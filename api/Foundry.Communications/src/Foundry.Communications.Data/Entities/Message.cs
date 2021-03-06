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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Foundry.Communications.Data.Entities
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        public string Subject { get; set; }
        [Required]
        public string Body { get; set; }
        /// <summary>
        /// this should be the complete url back to the source applications object
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// each client/extension may use this to categorize their messages as they see fit
        /// </summary>
        public ICollection<MessageValue> Values { get; set; } = new HashSet<MessageValue>();

        /// <summary>
        /// this is the unique identifier for the object related to the message
        /// </summary>
        public string GlobalId { get; set; }
        public Priority Priority { get; set; }
        public int? GroupId { get; set; }
        [ForeignKey("GroupId")]
        public Group Group { get; set; }
        [Required]
        public string SourceId { get; set; }
        [ForeignKey("SourceId")]
        public Source Source { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Broadcast { get; set; }
        public DateTime? Deleted { get; set; }

        public ICollection<Recipient> Recipients { get; set; } = new HashSet<Recipient>();
    }

    public enum Priority
    {
        Normal = 0,
        Elevated = 1,
        High = 2,
        System = 3
    }
}
