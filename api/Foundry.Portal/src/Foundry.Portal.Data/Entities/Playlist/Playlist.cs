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
    public class Playlist : IEntityPrimary, IEntityGlobal, IEntityLogo, IRated, IStackGroupPublisher
    {
        public int Id { get; set; }
        [Required]
        public string GlobalId { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Summary { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? Updated { get; set; }
        public string UpdatedBy { get; set; }
        public string LogoUrl { get; set; }
        public string TrailerUrl { get; set; }
        public bool IsDefault { get; set; }
        public bool IsPublic { get; set; }
        public int? ProfileId { get; set; }
        public double RatingAverage { get; set; }
        public Rating RatingMedian { get; set; }
        public int RatingTotal { get; set; }
        public Profile Profile { get; set; }
        public ICollection<Section> Sections { get; set; } = new HashSet<Section>();
        public ICollection<ProfileFollower> ProfileFollowers { get; set; } = new HashSet<ProfileFollower>();
        public ICollection<PlaylistKeyValue> KeyValues { get; set; } = new HashSet<PlaylistKeyValue>();
        public ICollection<PlaylistGroup> PlaylistGroups { get; set; } = new HashSet<PlaylistGroup>();
        public DateTime? Imported { get; set; }
        public string ImportedBy { get; set; }
        /// <summary>
        /// warehoused tag slugs
        /// </summary>
        public string Tags { get; set; }

        public ICollection<PlaylistTag> PlaylistTags { get; set; } = new HashSet<PlaylistTag>();

        public bool IsDisabled { get; set; } = false;

        public bool IsRecommended { get; set; } = false;

        public bool IsFeatured { get; set; } = false;

        public int FeaturedOrder { get; set; }
        public string Copyright { get; set; }
        public string PublisherId { get; set; }
        public string PublisherName { get; set; }
        public string PublisherSlug { get; set; }
    }
}

