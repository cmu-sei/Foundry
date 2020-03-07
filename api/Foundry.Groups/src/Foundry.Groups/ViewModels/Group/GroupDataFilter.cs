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
    /// group data filter
    /// </summary>
    public class GroupDataFilter : IDataFilter<Group>
    {
        public string Term { get; set; } = string.Empty;
        public int Skip { get; set; }
        public int Take { get; set; }
        public string Filter { get; set; }
        public string Sort { get; set; }

        /// <summary>
        /// TODO: move market group data filters in
        /// </summary>
        /// <param name="query"></param>
        /// <param name="identity"></param>
        /// <returns></returns>
        public IQueryable<Group> FilterQuery(IQueryable<Group> query, IStackIdentity identity)
        {
            var keyValues = Filter.ToFilterKeyValues();

            foreach (var filter in keyValues)
            {
                var key = filter.Key.Replace("!", "");
                var not = filter.Key.StartsWith("!");

                switch (key)
                {
                    case "root":
                        query = not
                            ? query.Where(g => !string.IsNullOrWhiteSpace(g.ParentId))
                            : query.Where(g => string.IsNullOrWhiteSpace(g.ParentId));
                        break;
                    case "member":
                    case "membership":
                        query = not
                            ? query.Where(g => !g.Members.Any(m => m.AccountId == identity.Id))
                            : query.Where(g => g.Members.Any(m => m.AccountId == identity.Id));
                        break;
                    case "managed+":
                        if (!identity.Permissions.Contains("administrator"))
                            goto case "managed";
                        break;
                    case "managed":
                        query = not
                            ? query.Where(g => !g.Members.Any(m => m.AccountId == identity.Id && (m.IsManager || m.IsOwner)))
                            : query.Where(g => g.Members.Any(m => m.AccountId == identity.Id && (m.IsManager || m.IsOwner)));
                        break;
                    case "available":
                        query = not
                            ? query.Where(g => g.Members.Any(m => m.AccountId == identity.Id))
                            : query.Where(g => !g.Members.Any(m => m.AccountId == identity.Id));
                        break;
                    default:
                        break;
                }
            }

            return query;
        }

        public IQueryable<Group> SearchQuery(IQueryable<Group> query)
        {
            if (string.IsNullOrWhiteSpace(Term))
                return query;

            var term = Term.ToLower().Trim();

            return query.Where(g =>
                g.Name.ToLower().Contains(term) ||
                (g.Description != null && g.Description.ToLower().Contains(term)));
        }

        public IOrderedQueryable<Group> SortQuery(IQueryable<Group> query)
        {
            if (string.IsNullOrWhiteSpace(Sort))
            {
                Sort = "alphabetic";
            }

            var sort = Sort.ToLower().Trim().Replace("-", "");
            var desc = Sort.StartsWith("-") ? true : false;

            var ordered = query.OrderBy(g => 0);

            switch (sort)
            {
                case "recent":
                    ordered = desc
                        ? query.OrderByDescending(g => g.Created)
                        : query.OrderBy(g => g.Created);
                    break;
                case "alphabetic":
                default:
                    ordered = desc
                        ? query.OrderByDescending(g => g.Name)
                        : query.OrderBy(g => g.Name);
                    break;
            }

            return ordered;
        }
    }
}

