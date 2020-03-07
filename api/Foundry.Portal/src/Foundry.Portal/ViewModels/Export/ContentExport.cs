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
    public class ContentExport
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string GlobalId { get; set; }
        public string Tags { get; set; }
        public string Url { get; set; }
        public string LogoUrl { get; set; }
        public string HoverUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public string TrailerUrl { get; set; }
        public string Settings { get; set; }
        public int Order { get; set; }
        public string Summary { get; set; }

        public string PublisherName { get; set; }
        public string PublisherGlobalId { get; set; }
        public string PublisherLogoUrl { get; set; }
        public string PublisherDescription { get; set; }

        public ContentType Type { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public string EndDate { get; set; }
        public string EndTime { get; set; }

        public string PlaylistName { get; set; }
        public string PlaylistGlobalId { get; set; }
        public string PlaylistLogoUrl { get; set; }
        public string PlaylistDescription { get; set; }
        public string PlaylistTrailerUrl { get; set; }
        public string PlaylistSummary { get; set; }
        public string PlaylistPublisherName { get; set; }
        public string PlaylistPublisherGlobalId { get; set; }
        public string PlaylistPublisherLogoUrl { get; set; }
        public string PlaylistPublisherDescription { get; set; }

        public string SectionName { get; set; }
        public int SectionOrder { get; set; }
        public int SectionContentOrder { get; set; }
    }
}
