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
    public class Group : IAudit, ISlug
    {
        [Key]
        [Required(AllowEmptyStrings = false)]
        public string Id { get; set; } = Guid.NewGuid().ToString().ToLower();

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Slug { get; set; }

        public string Description { get; set; }

        public string Summary { get; set; }

        public string LogoUrl { get; set; }

        public ICollection<Group> Children { get; set; } = new HashSet<Group>();

        public ICollection<Member> Members { get; set; } = new HashSet<Member>();

        public ICollection<MemberRequest> MemberRequests { get; set; } = new HashSet<MemberRequest>();

        public string MemberInviteCode { get; set; }

        public string GroupInviteCode { get; set; }

        [ForeignKey("ParentId")]
        public Group Parent { get; set; }

        public string ParentId { get; set; }

        /// <summary>
        /// warehoused parent ids for queries
        /// </summary>
        /// <example>
        /// GUID|GUID|GUID
        /// CCEE66F8-05C0-4FFF-BD88-07E66FE22694|2A3D6CA8-291D-4CC5-AB01-99A99EFB833D|{Id}
        /// </example>
        public string Key { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Updated { get; set; }
    }
}

