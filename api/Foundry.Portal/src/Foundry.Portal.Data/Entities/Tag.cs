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

namespace Foundry.Portal.Data.Entities
{
    public class Tag : IEntityPrimary
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        /// <summary>
        /// the url friendly tag text (ie: 'xxxxx-yyyyy', 'xxx-yyyy-zzzz')
        /// </summary>
        public string Slug { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// "" or NICE
        /// </summary>
        public string TagType { get; set; }

        /// <summary>
        /// Type=NICE: Knowledge, Skill, Ability
        /// </summary>
        public string TagSubType { get; set; }

        public DateTime Created { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? Updated { get; set; }

        public string UpdatedBy { get; set; }

        public ICollection<ContentTag> ContentTags { get; set; } = new HashSet<ContentTag>();

        public ICollection<PlaylistTag> PlaylistTags { get; set; } = new HashSet<PlaylistTag>();

    }
}
