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
using System.Text;

namespace Foundry.Groups.Data
{
    public class MemberRequest : IAudit
    {
        [Required(AllowEmptyStrings = false)]
        public string GroupId { get; set; }

        [ForeignKey("GroupId")]
        public Group Group { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string AccountId { get; set; }

        [ForeignKey("AccountId")]
        public Account Account { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Updated { get; set; }

        public MemberRequestStatus Status { get; set; } = MemberRequestStatus.Pending;
    }

    public enum MemberRequestStatus
    {
        Pending = 0,
        Approved = 1,
        Denied = 2
    }
}

