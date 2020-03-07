/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Stack.Http.Identity;
using Stack.Patterns.Service;
using Stack.Patterns.Service.Models;
using Foundry.Portal.Data;
using Foundry.Portal.Data.Entities;
using System.Linq;

namespace Foundry.Portal.ViewModels
{
    public class ProfileDataFilter : IDataFilter<Profile>
    {
        public string Term { get; set; } = string.Empty;
        public int Skip { get; set; }
        public int Take { get; set; }
        public string Filter { get; set; }
        public string Sort { get; set; }

        public IQueryable<Profile> FilterQuery(IQueryable<Profile> query, IStackIdentity profile)
        {
            var keyValues = Filter.ToFilterKeyValues();

            foreach (var filter in keyValues)
            {
                var ratingValues = filter.ToEnumValues<Rating>();
                var intValues = filter.ToIntValues();

                switch (filter.Key)
                {
                    case "letter":
                        query = query.Where(p => p.Name.ToLower().StartsWith(filter.Value.ToLower()));
                        break;
                    case "minratingaverage":
                        if (ratingValues.Any())
                        {
                            double minRatingAverage = (double)ratingValues.First();
                            query = query.Where(p => p.RatingAverage >= minRatingAverage);
                        }
                        break;
                    case "maxratingaverage":
                        if (ratingValues.Any())
                        {
                            double maxRatingAverage = (double)ratingValues.First();
                            query = query.Where(p => p.RatingAverage <= maxRatingAverage);
                        }
                        break;
                    case "minratingtotal":
                        if (intValues.Any())
                        {
                            int minRatingTotal = intValues.First();
                            query = query.Where(p => p.RatingTotal >= minRatingTotal);
                        }
                        break;
                    case "maxratingtotal":
                        if (intValues.Any())
                        {
                            int maxRatingTotal = intValues.First();
                            query = query.Where(p => p.RatingTotal <= maxRatingTotal);
                        }
                        break;
                    default:
                        break;
                }
            }

            return query;
        }

        public IQueryable<Profile> SearchQuery(IQueryable<Profile> query)
        {
            if (string.IsNullOrWhiteSpace(Term))
                return query;

            var term = Term.ToLower().Trim();

            return query.Where(p =>
                p.Name.ToLower().Contains(term) ||
                (p.Description != null && p.Description.ToLower().Contains(term))
            );
        }

        public IOrderedQueryable<Profile> SortQuery(IQueryable<Profile> query)
        {
            if (string.IsNullOrWhiteSpace(Sort))
            {
                Sort = "alphabetic";
            }

            var sort = Sort.ToLower().Trim().Replace("-", "");
            var desc = Sort.StartsWith("-") ? true : false;

            IOrderedQueryable<Profile> ordered = query.OrderBy(p => 0);

            switch (sort)
            {
                case "top":
                    ordered = desc
                        ? query.OrderBy(p => p.RatingTotal > 0 ? (p.RatingAverage * p.RatingTotal) / p.RatingTotal : 0)
                        : query.OrderByDescending(p => p.RatingTotal > 0 ? (p.RatingAverage * p.RatingTotal) / p.RatingTotal : 0);
                    break;
                case "contributions":
                    ordered = query.OrderByDescending(p => p.ProfileContents.Count(pc => pc.Rating != Rating.Unrated));
                    break;
                case "recent":
                    ordered = desc
                        ? query.OrderByDescending(c => c.Created)
                        : query.OrderBy(c => c.Created);
                    break;
                case "alphabetic":
                default:
                    ordered = desc
                        ? query.OrderByDescending(c => c.Name.ToLower())
                        : query.OrderBy(c => c.Name.ToLower());
                    break;
            }

            return ordered;
        }
    }
}
