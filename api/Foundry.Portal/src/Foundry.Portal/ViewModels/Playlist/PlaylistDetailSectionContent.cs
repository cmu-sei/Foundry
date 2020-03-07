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
using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Portal.ViewModels
{
    public class PlaylistDetailSectionContent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ContentType Type { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? Updated { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? Imported { get; set; }
        public string ImportedBy { get; set; }
        public int Order { get; set; }
        public int ChannelId { get; set; }
        public string ChannelName { get; set; } = string.Empty;
        public string ChannelSlug { get; set; }
        public string PublisherId { get; set; }
        public string PublisherName { get; set; } = string.Empty;
        public string PublisherSlug { get; set; }
        public string PublisherThumbnailUrl { get; set; }
        public int? AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string LogoUrl { get; set; }
        public string Slug { get; set; }
        public List<PlaylistDetailSectionContentTag> Tags { get; set; } = new List<PlaylistDetailSectionContentTag>();
        public string Url { get; set; }
        public DateTime? Start { get; set; }

        public DateTime? End { get; set; }

        public string StartDate { get; set; }

        public string StartTime { get; set; }

        public string EndDate { get; set; }

        public string EndTime { get; set; }
    }
}

