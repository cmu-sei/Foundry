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
using Foundry.Portal.Data.Entities;
using System.Linq;

namespace Foundry.Portal.ViewModels
{
    public class TagDataFilter : IDataFilter<Tag>
    {
        public string Term { get; set; } = string.Empty;
        public int Skip { get; set; }
        public int Take { get; set; }
        public string Filter { get; set; }
        public string Sort { get; set; }

        public IQueryable<Tag> FilterQuery(IQueryable<Tag> query, IStackIdentity profile)
        {
            var keyValues = Filter.ToFilterKeyValues();

            if (!keyValues.Any(kv => kv.Key == "type"))
            {
                // default to filter by emtpty type
                keyValues.Add(new FilterKeyValue("type="));
            }

            foreach (var filter in keyValues)
            {
                var key = filter.Key.Replace("!", "");
                var not = filter.Key.StartsWith("!");

                switch (key)
                {
                    case "type":
                        query = not
                            ? query.Where(x => (x.TagType == null ? "" : x.TagType.ToLower()) != filter.Value.ToLower())
                            : query.Where(x => (x.TagType == null ? "" : x.TagType.ToLower()) == filter.Value.ToLower());
                        break;
                    case "subtype":
                        query = not
                            ? query.Where(x => x.TagSubType.ToLower() != filter.Value.ToLower())
                            : query.Where(x => x.TagSubType.ToLower() == filter.Value.ToLower());
                        break;
                }
            }

            return query;
        }

        public IQueryable<Tag> SearchQuery(IQueryable<Tag> query)
        {
            if (string.IsNullOrWhiteSpace(Term))
                return query;

            var term = Term.ToLower().Trim();

            return query.Where(t =>
                t.Name.ToLower().Contains(term) ||
                (t.Description != null && t.Description.ToLower().Contains(term)) ||
                (t.TagType != null && t.TagType.ToLower().Contains(term)) ||
                (t.TagSubType != null && t.TagSubType.ToLower().Contains(term))
            );
        }

        public IOrderedQueryable<Tag> SortQuery(IQueryable<Tag> query)
        {
            if (string.IsNullOrWhiteSpace(Sort))
            {
                Sort = "alphabetic";
            }

            var sort = Sort.ToLower().Trim().Replace("-", "");
            var desc = Sort.StartsWith("-") ? true : false;

            switch (sort)
            {
                case "popular":
                    return desc
                        ? query.OrderBy(t => t.ContentTags.Count())
                        : query.OrderByDescending(t => t.ContentTags.Count());
                case "alphabetic":
                case "name":
                default:
                    return desc
                        ? query.OrderByDescending(c => c.Name)
                        : query.OrderBy(c => c.Name);
            }
        }
    }
}
