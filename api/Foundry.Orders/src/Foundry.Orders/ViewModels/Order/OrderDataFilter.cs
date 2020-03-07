/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Foundry.Orders.Data.Entities;
using Stack.Http.Identity;
using Stack.Patterns.Service.Models;
using Stack.Patterns.Service;
using System.Linq;

namespace Foundry.Orders.ViewModels
{
    public class OrderDataFilter : IDataFilter<Order>
    {
        public string Term { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public string Filter { get; set; }
        public string Sort { get; set; }

        public IQueryable<Order> FilterQuery(IQueryable<Order> query, IStackIdentity identity)
        {
            var keyValues = Filter.ToFilterKeyValues();

            foreach (var filter in keyValues)
            {
                var orderStatusValues = filter.ToEnumValues<OrderStatus>();
                var intValues = filter.ToIntValues();
                var key = filter.Key.Replace("!", "");
                var not = filter.Key.StartsWith("!");

                switch (key)
                {
                    case "status":
                        query = not
                            ? query.Where(x => !orderStatusValues.Contains(x.Status))
                            : query.Where(x => orderStatusValues.Contains(x.Status));
                        break;

                    default:
                        break;
                }
            }

            return query;
        }

        public IQueryable<Order> SearchQuery(IQueryable<Order> query)
        {
            if (!string.IsNullOrWhiteSpace(Term))
            {
                var term = Term.ToLower().Trim();
                query = query.Where(x => x.Description.ToLower().Contains(term));
            }

            return query;
        }

        public IOrderedQueryable<Order> SortQuery(IQueryable<Order> query)
        {
            if (string.IsNullOrWhiteSpace(Sort))
            {
                Sort = "recent";
            }

            var sort = Sort.ToLower().Trim().Replace("-", "");
            var desc = Sort.StartsWith("-") ? true : false;

            var ordered = query.OrderBy(c => 0);

            switch (sort)
            {
                case "due":
                    ordered = desc
                        ? query.OrderByDescending(c => c.Due)
                        : query.OrderBy(c => c.Due);
                    break;
                case "requestor":
                    ordered = desc
                        ? query.OrderByDescending(c => c.Requestor)
                        : query.OrderBy(c => c.Requestor);
                    break;
                case "status":
                    ordered = desc
                        ? query.OrderByDescending(c => (int)c.Status)
                        : query.OrderBy(c => (int)c.Status);
                    break;
                case "recent":
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

