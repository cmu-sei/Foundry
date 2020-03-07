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
using System.Linq;

namespace Foundry.Portal.ViewModels
{
    public class PostDataFilter : IDataFilter<Post>
    {
        public string Term { get; set; } = string.Empty;

        public int Skip { get; set; }

        public int Take { get; set; }

        public string Filter { get; set; }

        public string Sort { get; set; }

        public IQueryable<Post> FilterQuery(IQueryable<Post> query, IStackIdentity profile)
        {
            var keyValues = Filter.ToFilterKeyValues();

            // if not getting posts with a parent only look at top level posts
            if (!keyValues.Any(kv => kv.Key.Replace("!", "").ToLower() == "parent"))
            {
                query = query.Where(p => !p.ParentId.HasValue);
            }

            foreach (var filter in keyValues)
            {
                var ratingValues = filter.ToEnumValues<Rating>();
                var intValues = filter.ToIntValues();
                var key = filter.Key.Replace("!", "");
                var not = filter.Key.StartsWith("!");

                switch (key)
                {
                    case "profile":
                        query = not
                            ? query.Where(p => !intValues.Any(v => v == p.ProfileId))
                            : query.Where(p => intValues.Any(v => v == p.ProfileId));
                        break;
                    case "parent":
                        query = not
                            ? query.Where(p => !intValues.Any(v => v == p.ParentId && p.ParentId.HasValue))
                            : query.Where(p => intValues.Any(v => v == p.ParentId && p.ParentId.HasValue));
                        break;
                    case "attachments":
                        query = not
                            ? query.Where(p => !p.Attachments.Any())
                            : query.Where(p => p.Attachments.Any());
                        break;
                    default:
                        break;
                }
            }

            return query;
        }

        public IQueryable<Post> SearchQuery(IQueryable<Post> query)
        {
            if (string.IsNullOrWhiteSpace(Term))
                return query;

            var term = Term.ToLower().Trim();
            return query.Where(p => p.Text.ToLower().Contains(term));
        }

        public IOrderedQueryable<Post> SortQuery(IQueryable<Post> query)
        {
            if (string.IsNullOrWhiteSpace(Sort))
            {
                Sort = "recent";
            }

            var sort = Sort.ToLower().Trim().Replace("-", "");
            var desc = Sort.StartsWith("-") ? true : false;

            var ordered = query.OrderBy(g => 0);

            switch (sort)
            {
                case "top":
                    ordered = desc
                        ? query.OrderBy(p => p.Value)
                        : query.OrderByDescending(p => p.Value);
                    break;
                case "recent":
                default:
                    ordered = desc
                        ? query.OrderByDescending(p => p.Created)
                        : query.OrderBy(p => p.Created);
                    break;
            }

            return ordered;
        }
    }
}
