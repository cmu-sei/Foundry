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

namespace Foundry.Buckets.Data.Entities
{
    public class Bucket : IAudit, ISlug, IGlobal
    {
        [Key]
        public int Id { get; set; }

        public string GlobalId { get; set; }

        [MaxLength(250)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [MaxLength(250)]
        public string Slug { get; set; }

        public ICollection<File> Files { get; set; } = new HashSet<File>();

        public ICollection<BucketAccount> BucketAccounts { get; set; } = new HashSet<BucketAccount>();

        public string CreatedById { get; set; }

        [ForeignKey("CreatedById")]
        public Account CreatedBy { get; set; }

        public DateTime Created { get; set; }

        public BucketSharingType BucketSharingType { get; set; }

        public string RestrictedKey { get; set; }
    }
}
