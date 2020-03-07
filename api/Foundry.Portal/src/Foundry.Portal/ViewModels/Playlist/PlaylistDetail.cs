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
    public class PlaylistDetail
    {
        public int Id { get; set; }
        public string GlobalId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string Description { get; set; }
        public string Summary { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? Updated { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? Imported { get; set; }
        public string ImportedBy { get; set; }
        public bool IsDefault { get; set; }
        public bool IsPublic { get; set; }
        public List<PlaylistDetailSection> Sections { get; set; } = new List<PlaylistDetailSection>();
        public List<PlaylistDetailTag> Tags { get; set; } = new List<PlaylistDetailTag>();
        public int SectionCount { get; set; }
        public int ContentCount { get; set; }
        public List<PlaylistDetailProfileFollower> ProfileFollowers { get; set; } = new List<PlaylistDetailProfileFollower>();
        public List<PlaylistDetailGroupFollower> GroupFollowers { get; set; } = new List<PlaylistDetailGroupFollower>();
        public int ProfileFollowerCount { get; set; }
        public int GroupFollowerCount { get; set; }
        public bool CanFollow { get; set; }
        public bool IsFollowing { get; set; }
        public bool IsOwner { get; set; }
        public bool CanEdit { get; set; }
        public string LogoUrl { get; set; }
        public string TrailerUrl { get; set; }
        public bool IsRecommended { get; set; }
        public bool IsFeatured { get; set; }
        public int FeaturedOrder { get; set; }
        // user context
        [JsonConverter(typeof(StringEnumConverter))]
        public Rating UserRating { get; set; }
        //calculations
        public RatingMetricDetail Rating { get; set; } = new RatingMetricDetail();
        public List<PlaylistDetailPlaylistKeyValue> KeyValues { get; set; } = new List<PlaylistDetailPlaylistKeyValue>();
        public string Copyright { get; set; }
        public int? ProfileId { get; set; }
        public string ProfileName { get; set; } = string.Empty;
        public string ProfileSlug { get; set; } = string.Empty;
        public string PublisherId { get; set; } = string.Empty;
        public string PublisherName { get; set; } = string.Empty;
        public string PublisherSlug { get; set; } = string.Empty;
    }
}

