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
using Foundry.Portal.Data.Entities;
using Foundry.Portal.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Foundry.Portal.Extensions
{
    public static class IRatedExtensions
    {
        public static RatingMetricDetail ToRatingMetricDetail(this IRated rated)
        {
            if (rated == null)
                return new RatingMetricDetail();

            int poor = 0;
            int fair = 0;
            int good = 0;
            int great = 0;

            if (rated is Content content)
            {
                var profileContents = content.ProfileContents ?? new HashSet<ProfileContent>();

                poor = profileContents.Count(pc => pc.Rating == Rating.Poor);
                fair = profileContents.Count(pc => pc.Rating == Rating.Fair);
                good = profileContents.Count(pc => pc.Rating == Rating.Good);
                great = profileContents.Count(pc => pc.Rating == Rating.Great);
            }

            return new RatingMetricDetail()
            {
                Average = rated.RatingAverage,
                Median = rated.RatingMedian,
                Poor = poor,
                Fair = fair,
                Good = good,
                Great = great,
                Total = rated.RatingTotal
            };
        }
    }
}
