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
    public class PlaylistDataFilter : IDataFilter<Playlist>
    {
        public string Term { get; set; } = string.Empty;
        public int Skip { get; set; }
        public int Take { get; set; }
        public string Filter { get; set; }
        public string Sort { get; set; }

        public IQueryable<Playlist> FilterQuery(IQueryable<Playlist> query, IStackIdentity identity)
        {
            var keyValues = Filter.ToFilterKeyValues();

            foreach (var filter in keyValues)
            {
                var ratingValues = filter.ToEnumValues<Rating>();
                var intValues = filter.ToIntValues();
                var key = filter.Key.Replace("!", "");
                var not = filter.Key.StartsWith("!");

                switch (key)
                {
                    case "public":
                        query = not
                            ? query.Where(pl => !pl.IsPublic)
                            : query.Where(pl => pl.IsPublic);
                        break;
                    case "following":
                        query = not
                            ? query.Where(pl => !pl.ProfileFollowers.Any(s => s.ProfileId == identity.GetId() || pl.ProfileId == identity.GetId()))
                            : query.Where(pl => pl.ProfileFollowers.Any(s => s.ProfileId == identity.GetId()) || pl.ProfileId == identity.GetId());
                        break;
                    case "managed+":
                        if (!identity.Permissions.Contains(SystemPermissions.PowerUser))
                        {
                            goto case "managed";
                        }
                        break;
                    case "managed":
                        query = not
                            ? query.Where(pl => pl.ProfileId != identity.GetId())
                            : query.Where(pl => pl.ProfileId == identity.GetId());
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
                    case "featured":
                        query = not
                            ? query.Where(p => !p.IsFeatured)
                            : query.Where(p => p.IsFeatured);
                        break;
                    case "recommended":
                        query = not
                            ? query.Where(p => !p.IsRecommended)
                            : query.Where(p => p.IsRecommended);
                        break;
                    case "tag":
                        query = not
                            ? query.Where(p => !p.PlaylistTags.Any(pt => filter.StringValues.Contains(pt.Tag.Name.ToLower())))
                            : query.Where(p => p.PlaylistTags.Any(pt => filter.StringValues.Contains(pt.Tag.Name.ToLower())));
                        break;
                    default:
                        break;
                }
            }

            return query;
        }

        public IQueryable<Playlist> SearchQuery(IQueryable<Playlist> query)
        {
            if (string.IsNullOrWhiteSpace(Term))
                return query;

            var term = Term.ToLower().Trim();

            return query.Where(pl =>
                pl.Name.ToLower().Contains(term) ||
                (pl.Description != null && pl.Description.ToLower().Contains(term))
            );
        }

        public IOrderedQueryable<Playlist> SortQuery(IQueryable<Playlist> query)
        {
            if (string.IsNullOrWhiteSpace(Sort))
            {
                Sort = "alphabetic";
            }

            var sort = Sort.ToLower().Trim().Replace("-", "");
            var desc = Sort.StartsWith("-") ? true : false;

            var ordered = query.OrderBy(p => 0);

            switch (sort)
            {
                case "recent":
                    ordered = desc
                        ? query.OrderByDescending(c => c.Created)
                        : query.OrderBy(c => c.Created);
                    break;
                case "rating":
                    ordered = desc
                        ? query.OrderByDescending(c => c.RatingAverage)
                        : query.OrderBy(c => c.RatingAverage);
                    break;
                case "popular":
                    ordered = desc
                        ? query.OrderByDescending(c => c.ProfileFollowers.Count())
                        : query.OrderBy(c => c.ProfileFollowers.Count());
                    break;
                case "top":
                    ordered = desc
                        ? query.OrderBy(p => p.RatingTotal > 0 ? (p.RatingAverage * p.RatingTotal) / p.RatingTotal : 0)
                        : query.OrderByDescending(p => p.RatingTotal > 0 ? (p.RatingAverage * p.RatingTotal) / p.RatingTotal : 0);
                    break;
                default:
                case "alphabetic":
                    ordered = desc
                        ? query.OrderByDescending(c => c.Name)
                        : query.OrderBy(c => c.Name);
                    break;
            }

            return ordered;
        }
    }
}

