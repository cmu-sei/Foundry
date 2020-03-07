/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Foundry.Portal.Data;

namespace Foundry.Portal.ViewModels
{
    public class ContentPatch
    {
        public int Id { get; set; }
        public string Name { get; set; } = null;
        public string Description { get; set; } = null;
        public string Url { get; set; } = null;
        public string LogoUrl { get; set; } = null;
        public string HoverUrl { get; set; } = null;
        public string ThumbnailUrl { get; set; } = null;
        public string TrailerUrl { get; set; } = null;
        public string Settings { get; set; } = null;
        public int? Order { get; set; } = null;
        //public int? ChannelId { get; set; } = null;
        public string PublisherId { get; set; } = null;
        public string PublisherName { get; set; } = null;
        public string PublisherSlug { get; set; } = null;
        public bool? IsDisabled { get; set; } = null;
        public bool? IsRecommended { get; set; } = null;
        public bool? IsFeatured { get; set; } = null;
        public int? FeaturedOrder { get; set; } = null;
        public string[] Tags { get; set; } = null;
        public ContentType? Type { get; set; } = null;
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public string EndDate { get; set; }
        public string EndTime { get; set; }
    }
}

