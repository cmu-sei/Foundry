/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Foundry.Communications.Data.Entities;
using Stack.Http.Identity;
using Stack.Patterns.Service;
using Stack.Patterns.Service.Models;
using System.Linq;

namespace Foundry.Communications.ViewModels
{
    /// <summary>
    /// notification data filter
    /// </summary>
    public class NotificationDataFilter : IDataFilter<Notification>
    {
        public string Term { get; set; } = string.Empty;
        public int Skip { get; set; }
        public int Take { get; set; }
        public string Filter { get; set; }
        public string Sort { get; set; }

        /// <summary>
        /// filter query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="identity"></param>
        /// <returns></returns>
        public IQueryable<Notification> FilterQuery(IQueryable<Notification> query, IStackIdentity identity)
        {
            var keyValues = Filter.ToFilterKeyValues();

            foreach (var filter in keyValues)
            {
                var key = filter.Key.Replace("!", "");
                var not = filter.Key.StartsWith("!");

                if (not && key == "read") key = "unread";
                else if (not && key == "unread") key = "read";

                switch (key)
                {
                    case "type":
                        query = not
                            ? query.Where(n => n.Values.Any(v => v.Key == key && v.Value.ToLower() != filter.Value.ToLower()))
                            : query.Where(n => n.Values.Any(v => v.Key == key && v.Value.ToLower() == filter.Value.ToLower()));
                        break;
                    case "read":
                        query = query.Where(n =>
                            n.Recipients.Where(r => r.TargetId.ToLower() == identity.Id.ToLower())
                            .All(r => r.Read.HasValue && !r.Deleted.HasValue));
                        break;
                    default:
                    case "unread":
                        query = query.Where(n =>
                            n.Recipients.Where(r => r.TargetId.ToLower() == identity.Id.ToLower())
                            .All(r => !r.Read.HasValue && !r.Deleted.HasValue));
                        break;
                }
            }

            return query;
        }

        /// <summary>
        /// search query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IQueryable<Notification> SearchQuery(IQueryable<Notification> query)
        {
            if (!string.IsNullOrWhiteSpace(Term))
            {
                var term = Term.ToLower().Replace(" ", "");

                query = query.Where(n =>
                    (n.Body != null && n.Body.ToLower().Replace(" ", "").Contains(term)) ||
                    (n.Subject != null && n.Subject.ToLower().Replace(" ", "").Contains(term))
                );

            }
            return query;
        }

        /// <summary>
        /// sort query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IOrderedQueryable<Notification> SortQuery(IQueryable<Notification> query)
        {
            if (string.IsNullOrWhiteSpace(Sort))
            {
                Sort = "-recent";
            }

            var sort = Sort.ToLower().Trim().Replace("-", "");
            var desc = Sort.StartsWith("-") ? true : false;

            switch (sort)
            {
                case "subject":
                    return desc
                        ? query.OrderByDescending(c => c.Subject)
                        : query.OrderBy(c => c.Subject);
                case "body":
                    return desc
                        ? query.OrderByDescending(c => c.Body)
                        : query.OrderBy(c => c.Body);
                case "priority":
                    return desc
                        ? query.OrderByDescending(c => c.Priority).ThenBy(c => c.Created)
                        : query.OrderBy(c => c.Priority).ThenBy(c => c.Created);
                case "recent":
                default:
                    return desc
                        ? query.OrderByDescending(c => c.Created)
                        : query.OrderBy(c => c.Created);
            }
        }
    }
}
