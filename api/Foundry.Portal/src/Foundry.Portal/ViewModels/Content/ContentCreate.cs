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
using Foundry.Portal.Validation.ValidationRules;
using System.ComponentModel.DataAnnotations;

namespace Foundry.Portal.ViewModels
{
    [Stack.Validation.Attributes.Validation(
        typeof(ContentNameIsRequired),
        typeof(ContentTypeIsRequired),
        typeof(ContentTagIsRequired),
        typeof(ContentLogoUrlIsRequired))]
    public class ContentCreate
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(Keys.MAXLENGTH_NAME)]
        public string Name { get; set; }
        [MaxLength(Keys.MAXLENGTH_DESCRIPTION)]
        public string Description { get; set; }
        [MaxLength(Keys.MAXLENGTH_SUMMARY)]
        public string Summary { get; set; }
        [MaxLength(Keys.MAXLENGTH_DESCRIPTION)]
        public string Copyright { get; set; }
        public string GlobalId { get; set; }
        public string[] Tags { get; set; }
        public string Url { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string LogoUrl { get; set; }
        public string HoverUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public string TrailerUrl { get; set; }
        public string Settings { get; set; }
        public int Order { get; set; }
        public string PublisherId { get; set; }
        public string PublisherName { get; set; }
        public string PublisherSlug { get; set; }
        public string AuthorId { get; set; }
        public bool IsDisabled { get; set; } = false;
        public bool IsRecommended { get; set; }
        public bool IsFeatured { get; set; }
        public int FeaturedOrder { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ContentType Type { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public string EndDate { get; set; }
        public string EndTime { get; set; }
    }
}

