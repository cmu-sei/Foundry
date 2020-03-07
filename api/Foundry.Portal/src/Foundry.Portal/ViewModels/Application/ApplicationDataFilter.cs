/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Foundry.Portal.Data.Entities;
using Stack.Http.Identity;
using Stack.Patterns.Service;
using Stack.Patterns.Service.Models;
using System.Linq;

namespace Foundry.Portal.ViewModels
{
    public class ApplicationDataFilter : IDataFilter<Application>
    {
        public string Term { get; set; } = string.Empty;

        public int Skip { get; set; }

        public int Take { get; set; }

        public string Filter { get; set; }

        public string Sort { get; set; }

        public IQueryable<Application> FilterQuery(IQueryable<Application> query, IStackIdentity identity)
        {
            var keyValues = Filter.ToFilterKeyValues();

            foreach (var filter in keyValues)
            {
                var key = filter.Key.Replace("!", "");
                var not = filter.Key.StartsWith("!");

                switch (key)
                {
                    case "hidden":
                        query = not
                            ? query.Where(a => !a.IsHidden)
                            : query.Where(a => a.IsHidden);
                        break;
                    case "myapps":
                    default:
                        query = not
                            ? query.Where(a => !a.ProfileApplications.Any(pa => pa.ProfileId == identity.GetId()))
                            : query.Where(a => a.ProfileApplications.Any(pa => pa.ProfileId == identity.GetId()));
                        break;
                }
            }

            return query;
        }

        public IQueryable<Application> SearchQuery(IQueryable<Application> query)
        {
            if (string.IsNullOrWhiteSpace(Term))
                return query;

            var term = Term.ToLower().Trim();

            return query.Where(ach =>
                ach.Name.ToLower().Contains(term) ||
                (ach.Description != null && ach.Description.ToLower().Contains(term))
            );
        }

        public IOrderedQueryable<Application> SortQuery(IQueryable<Application> query)
        {
            if (string.IsNullOrWhiteSpace(Sort))
            {
                Sort = "alphabetic";
            }

            var sort = Sort.ToLower().Trim().Replace("-", "");
            var desc = Sort.StartsWith("-") ? true : false;

            switch (sort)
            {
                case "alphabetic":
                default:
                    return desc
                        ? query.OrderByDescending(c => c.Name)
                        : query.OrderBy(c => c.Name);
            }
        }
    }
}

