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

namespace Foundry.Portal.Data.Entities
{
    public class ProfileContent : IEntityAudit
    {
        public int Id { get; set; }
        public int ContentId { get; set; }
        public Content Content { get; set; }
        public int ProfileId { get; set; }
        public Profile Profile { get; set; }
        public Difficulty Difficulty { get; set; } = Difficulty.Unrated;

        public Rating Rating { get; set; } = Rating.Unrated;
        public ICollection<ProfileContent> ProfileContents { get; set; } = new List<ProfileContent>();
        public DateTime Created { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? Updated { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? Flagged { get; set; }

        public string FlaggedComment { get; set; }

        public FlagStatusType FlagStatus { get; set;}

        public DateTime? Bookmarked { get; set; }
    }
}
