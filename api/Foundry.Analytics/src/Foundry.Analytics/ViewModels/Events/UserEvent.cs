/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Foundry.Analytics.Data;
using Stack.Http.Identity;
using Stack.Patterns.Service;
using Stack.Patterns.Service.Models;
using System;
using System.Linq;

namespace Foundry.Analytics.ViewModels
{
    public class UserEventCreate
    {
        public string Data { get; set; }
        public string Type { get; set; }
    }

    public class UserEventSummary : IAnalyticsEventSummary
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public string Type { get; set; }
        public string ClientId { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
    }

    public class UserEventDataFilter : IDataFilter<UserEvent>
    {
        public string Term { get; set; } = string.Empty;
        public int Skip { get; set; }
        public int Take { get; set; }
        public string Filter { get; set; }
        public string Sort { get; set; }

        public IQueryable<UserEvent> FilterQuery(IQueryable<UserEvent> query, IStackIdentity identity)
        {
            var keyValues = Filter.ToFilterKeyValues();

            foreach (var filter in keyValues)
            {
                var intValues = filter.ToIntValues();
                var key = filter.Key.Replace("!", "");
                var not = filter.Key.StartsWith("!");

                switch (key)
                {
                    case "createdby": // all queries are scoped to the current user by default but this could be used later
                        query = not
                            ? query.Where(c => c.CreatedBy != identity.Id)
                            : query.Where(c => c.CreatedBy == identity.Id);
                        break;
                    case "type":
                        query = not
                         ? query.Where(c => filter.Value.Trim().ToLower() != c.Type.ToLower())
                         : query.Where(c => filter.Value.Trim().ToLower() == c.Type.ToLower());
                        break;
                    default:
                        break;
                }
            }

            return query;
        }

        public IQueryable<UserEvent> SearchQuery(IQueryable<UserEvent> query)
        {
            if (string.IsNullOrWhiteSpace(Term))
                return query;

            var term = Term.ToLower().Trim();

            return query.Where(c =>
                c.CreatedBy.ToLower().Contains(term) ||
                c.Type.ToLower().Contains(term)
            );
        }

        public IOrderedQueryable<UserEvent> SortQuery(IQueryable<UserEvent> query)
        {
            if (string.IsNullOrWhiteSpace(Sort))
            {
                Sort = "created";
            }

            var sort = Sort.ToLower().Trim().Replace("-", "");
            var desc = Sort.StartsWith("-") ? true : false;

            var ordered = query.OrderBy(c => 0);

            switch (sort)
            {
                case "created":
                default:
                    ordered = desc
                        ? query.OrderByDescending(c => c.Created)
                        : query.OrderBy(c => c.Created);
                    break;
            }

            return ordered;
        }
    }
}
