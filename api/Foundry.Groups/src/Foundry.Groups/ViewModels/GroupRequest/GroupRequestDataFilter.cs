/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Foundry.Groups.Data;
using Stack.Http.Identity;
using Stack.Patterns.Service;
using Stack.Patterns.Service.Models;
using System.Linq;

namespace Foundry.Groups.ViewModels
{
    /// <summary>
    /// group request data filter
    /// </summary>
    public class GroupRequestDataFilter : IDataFilter<GroupRequest>
    {
        public string Term { get; set; } = string.Empty;
        public int Skip { get; set; }
        public int Take { get; set; }
        public string Filter { get; set; }
        public string Sort { get; set; }

        /// <summary>
        /// filter group request query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="identity"></param>
        /// <returns></returns>
        public IQueryable<GroupRequest> FilterQuery(IQueryable<GroupRequest> query, IStackIdentity identity)
        {
            var keyValues = Filter.ToFilterKeyValues();

            foreach (var filter in keyValues)
            {
                var groupRequestStatusValues = filter.ToEnumValues<GroupRequestStatus>();
                var key = filter.Key.Replace("!", "");
                var not = filter.Key.StartsWith("!");

                switch (key)
                {
                    case "status":
                        query = not
                            ? query.Where(gr => !groupRequestStatusValues.Contains(gr.Status))
                            : query.Where(gr => groupRequestStatusValues.Contains(gr.Status));
                        break;
                    default:
                        break;
                }
            }

            return query;
        }

        public IQueryable<GroupRequest> SearchQuery(IQueryable<GroupRequest> query)
        {
            if (string.IsNullOrWhiteSpace(Term))
                return query;

            var term = Term.ToLower().Trim();

            return query.Where(gr =>
                gr.ParentGroup.Name.ToLower().Contains(term) ||
                gr.ChildGroup.Name.ToLower().Contains(term) ||
                (gr.ParentGroup.Description != null && gr.ParentGroup.Description.ToLower().Contains(term)) ||
                (gr.ChildGroup.Description != null && gr.ChildGroup.Description.ToLower().Contains(term))
            );
        }

        public IOrderedQueryable<GroupRequest> SortQuery(IQueryable<GroupRequest> query)
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
                case "recent":
                default:
                    ordered = desc
                        ? query.OrderByDescending(g => g.Created)
                        : query.OrderBy(g => g.Created);
                    break;
            }

            return ordered;
        }
    }
}

