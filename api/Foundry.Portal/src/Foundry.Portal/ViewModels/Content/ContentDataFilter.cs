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
using Stack.Http.Identity;
using Stack.Patterns.Service;
using Stack.Patterns.Service.Models;
using System;
using System.Linq;

namespace Foundry.Portal.ViewModels
{
    public class ContentDataFilter : IDataFilter<Content>
    {
        public string Term { get; set; } = string.Empty;
        public int Skip { get; set; }
        public int Take { get; set; }
        public string Filter { get; set; }
        public string Sort { get; set; }

        public IQueryable<Content> FilterQuery(IQueryable<Content> query, IStackIdentity identity)
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
                    case "created":
                        query = not
                            ? query.Where(c => c.AuthorId != identity.GetId())
                            : query.Where(c => c.AuthorId == identity.GetId());
                        break;
                    case "playlist":
                        query = not
                            ? query.Where(c => !c.SectionContents.Any(sc => intValues.Contains(sc.Section.PlaylistId)))
                            : query.Where(c => c.SectionContents.Any(sc => intValues.Contains(sc.Section.PlaylistId)));
                        break;
                    case "group":
                        query = not
                            ? query.Where(c => c.PublisherId != filter.Value)
                            : query.Where(c => c.PublisherId == filter.Value);

                        break;
                    case "tag":
                        query = not
                            ? query.Where(c => !c.ContentTags.Any(ct => filter.StringValues.Contains(ct.Tag.Name.ToLower())))
                            : query.Where(c => c.ContentTags.Any(ct => filter.StringValues.Contains(ct.Tag.Name.ToLower())));
                        break;
                    case "bookmarked":
                        query = not
                            ? query.Where(c => !c.Start.HasValue && !c.ProfileContents.Any(pc => pc.Bookmarked.HasValue && pc.ProfileId == identity.GetId()))
                            : query.Where(c => !c.Start.HasValue && c.ProfileContents.Any(pc => pc.Bookmarked.HasValue && pc.ProfileId == identity.GetId()));
                        break;
                    case "contenttype":
                        var contentTypes = filter.ToEnumValues<ContentType>();
                        query = not
                            ? query.Where(c => !contentTypes.Contains(c.Type))
                            : query.Where(c => contentTypes.Contains(c.Type));
                        break;
                    case "myevents":
                        query = not
                            ? query.Where(c => c.Start.HasValue && !c.ProfileContents.Any(pc => pc.ProfileId == identity.GetId() && pc.Bookmarked.HasValue))
                            : query.Where(c => c.Start.HasValue && c.ProfileContents.Any(pc => pc.ProfileId == identity.GetId() && pc.Bookmarked.HasValue));
                        break;
                    case "recommended":
                        query = not
                            ? query.Where(c => !c.IsRecommended)
                            : query.Where(c => c.IsRecommended);
                        break;
                    case "startdate":
                        if (filter.StringValues.Any() && DateTime.TryParse(filter.StringValues.First(), out var start))
                        {
                            // if the start or end date for content is greater than or equal to the date
                            query = query.Where(c => c.Start.HasValue && c.Start >= start || c.End.HasValue && c.End >= start);
                        }
                        break;
                    case "enddate":
                        if (filter.StringValues.Any() && DateTime.TryParse(filter.StringValues.First(), out var end))
                        {
                            // if the start or end date for content is less than or equal to the date
                            query = query.Where(c => c.Start.HasValue && c.Start <= end || c.End.HasValue && c.End <= end);
                        }
                        break;
                    case "minratingaverage":
                        if (ratingValues.Any())
                        {
                            double minRatingAverage = (double)ratingValues.First();
                            query = query.Where(c => c.RatingAverage >= minRatingAverage);
                        }
                        break;
                    case "maxratingaverage":
                        if (ratingValues.Any())
                        {
                            double maxRatingAverage = (double)ratingValues.First();
                            query = query.Where(c => c.RatingAverage <= maxRatingAverage);
                        }
                        break;
                    case "minratingtotal":
                        if (intValues.Any())
                        {
                            int minRatingTotal = intValues.First();
                            query = query.Where(c => c.RatingTotal >= minRatingTotal);
                        }
                        break;
                    case "maxratingtotal":
                        if (intValues.Any())
                        {
                            int maxRatingTotal = intValues.First();
                            query = query.Where(c => c.RatingTotal <= maxRatingTotal);
                        }
                        break;
                    case "featured":
                        query = not
                            ? query.Where(c => !c.IsFeatured)
                            : query.Where(c => c.IsFeatured);
                        break;
                    case "flagged":
                        query = not
                            ? query.Where(c => c.FlagCount == 0)
                            : query.Where(c => c.FlagCount > 0);
                        break;
                    case "url":
                           query = not
                            ? query.Where(c => filter.Value.Trim().ToLower() != c.Url.ToLower())
                            : query.Where(c => filter.Value.Trim().ToLower() == c.Url.ToLower());
                        break;
                    default:
                        break;
                }
            }

            return query;
        }

        public IQueryable<Content> SearchQuery(IQueryable<Content> query)
        {
            if (string.IsNullOrWhiteSpace(Term))
                return query;

            var term = Term.ToLower().Trim();

            return query.Where(c =>
                c.Name.ToLower().Contains(term) ||
                (c.Description != null && c.Description.ToLower().Contains(term)) ||
                c.Tags.Contains(term)
            );
        }

        public IOrderedQueryable<Content> SortQuery(IQueryable<Content> query)
        {
            if (string.IsNullOrWhiteSpace(Sort))
            {
                Sort = "alphabetic";
            }

            var sort = Sort.ToLower().Trim().Replace("-", "");
            var desc = Sort.StartsWith("-") ? true : false;

            var ordered = query.OrderBy(c => 0);

            switch (sort)
            {
                case "alphabetic":
                    ordered = desc
                        ? query.OrderByDescending(c => c.Name)
                        : query.OrderBy(c => c.Name);
                    break;
                case "recent":
                    ordered = desc
                        ? query.OrderByDescending(c => c.Created)
                        : query.OrderBy(c => c.Created);
                    break;
                case "date":
                    ordered = desc
                        ? query.OrderByDescending(c => c.Start)
                        : query.OrderBy(c => c.Start);
                    break;
                case "level":
                    ordered = desc
                        ? query.OrderByDescending(c => c.DifficultyAverage)
                        : query.OrderBy(c => c.DifficultyAverage);
                    break;
                case "rating":
                    ordered = desc
                        ? query.OrderByDescending(c => c.RatingAverage)
                        : query.OrderBy(c => c.RatingAverage);
                    break;
                case "popular":
                    ordered = desc
                        ? query.OrderByDescending(c => c.RatingTotal)
                        : query.OrderBy(c => c.RatingTotal);
                    break;
                case "top":
                    ordered = desc
                       ? query.OrderBy(c => c.RatingTotal > 0 ? (c.RatingAverage * c.RatingTotal) / c.RatingTotal : 0)
                       : query.OrderByDescending(c => c.RatingTotal > 0 ? (c.RatingAverage * c.RatingTotal) / c.RatingTotal : 0);
                    break;
                case "index":
                default:
                    ordered = desc
                        ? query.OrderByDescending(c => c.Order).ThenBy(c => c.Id)
                        : query.OrderBy(c => c.Order).ThenBy(c => c.Id);
                    break;
            }

            return ordered;
        }
    }
}

