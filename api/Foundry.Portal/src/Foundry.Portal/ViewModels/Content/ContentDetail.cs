/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Foundry.Portal.Data;
using System;
using System.Collections.Generic;

namespace Foundry.Portal.ViewModels
{
    public class ContentDetail : IActions, IAccess, ILabels
    {
        public int Id { get; set; }
        public string GlobalId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Summary { get; set; }
        public string Copyright { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? Updated { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? Imported { get; set; }
        public string ImportedBy { get; set; }
        public List<ContentDetailTag> Tags { get; set; } = new List<ContentDetailTag>();
        public string Url { get; set; }
        public string LogoUrl { get; set; }
        public string HoverUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public string TrailerUrl { get; set; }
        public bool IsFlagged { get; set; }
        public bool IsBookmarked { get; set; }
        public int FlagCount { get; set; }
        public List<ContentDetailFlag> Flags { get; set; } = new List<ContentDetailFlag>();
        public int Order { get; set; }

        public int? AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string AuthorSlug { get; set; } = string.Empty;
        public string PublisherId { get; set; }
        public string PublisherName { get; set; } = string.Empty;
        public string PublisherSlug { get; set; } = string.Empty;
        public string PublisherThumbnailUrl { get; set; }
        public List<ContentDetailProfileContent> ProfileContents { get; set; } = new List<ContentDetailProfileContent>();
        public List<ContentDetailDiscussion> Discussions { get; set; } = new List<ContentDetailDiscussion>();
        public List<ContentDetailContentKeyValue> KeyValues { get; set; } = new List<ContentDetailContentKeyValue>();

        public List<ContentDetailPlaylist> Playlists { get; set; } = new List<ContentDetailPlaylist>();

        public bool IsDisabled { get; set; }
        public bool IsRecommended { get; set; }
        public bool IsFeatured { get; set; }
        public int FeaturedOrder { get; set; }
        // user context
        [JsonConverter(typeof(StringEnumConverter))]
        public Rating UserRating { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Difficulty UserDifficulty { get; set; }
        public bool CanAccess { get; set; }
        public bool CanEdit { get; set; }
        // calculations
        public RatingMetricDetail Rating { get; set; } = new RatingMetricDetail();
        public DifficultyMetricDetail Difficulty { get; set; } = new DifficultyMetricDetail();
        public string LaunchUrl { get; internal set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ContentType Type { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public string EndDate { get; set; }
        public string EndTime { get; set; }

        public List<string> Access { get; set; } = new List<string>();
        public List<string> Actions { get; set; } = new List<string>();
        /// <summary>
        /// these are system defined labels like "new" and "top" to be used by the UI
        /// </summary>
        public List<string> Labels { get; set; } = new List<string>();
    }
}

