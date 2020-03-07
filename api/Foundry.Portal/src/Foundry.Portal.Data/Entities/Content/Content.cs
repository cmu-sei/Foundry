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
    public class Content : IEntityPrimary, IDiscussions, IEntityGlobal, IRated, IDifficulty, IEntityLogo, IImported, IStackGroupPublisher
    {
        public int Id { get; set; }
        public string GlobalId { get; set; } = Guid.Empty.ToString();
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Summary { get; set; }
        public string Copyright { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? Updated { get; set; }
        public string UpdatedBy { get; set; }
        public Profile Author { get; set; }
        public int? AuthorId { get; set; }
        public ContentType Type { get; set; }
        /// <summary>
        /// warehoused tag slugs
        /// </summary>
        public string Tags { get; set; }
        public string Url { get; set; }
        public string LogoUrl { get; set; }
        public string HoverUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public string TrailerUrl { get; set; }
        public string Settings { get; set; }
        public int Order { get; set; }
        public string PublisherId { get; set; }
        public string PublisherName { get; set; }
        public string PublisherSlug { get; set; }
        public ICollection<ProfileContent> ProfileContents { get; set; } = new HashSet<ProfileContent>();
        public ICollection<Discussion> Discussions { get; set; } = new HashSet<Discussion>();
        public ICollection<SectionContent> SectionContents { get; set; } = new HashSet<SectionContent>();
        public ICollection<ContentTag> ContentTags { get; set; } = new HashSet<ContentTag>();
        public bool IsDisabled { get; set; } = false;
        public bool IsRecommended { get; set; } = false;
        public bool IsFeatured { get; set; } = false;
        public int FeaturedOrder { get; set; }
        public double RatingAverage { get; set; }
        public Rating RatingMedian { get; set; }
        public int RatingTotal { get; set; }
        public double DifficultyAverage { get; set; }
        public Difficulty DifficultyMedian { get; set; }
        public int DifficultyTotal { get; set; }
        public ICollection<ContentKeyValue> KeyValues { get; set; } = new HashSet<ContentKeyValue>();
        public int FlagCount { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public DateTime? Imported { get; set; }
        public string ImportedBy { get; set; }
    }
}

