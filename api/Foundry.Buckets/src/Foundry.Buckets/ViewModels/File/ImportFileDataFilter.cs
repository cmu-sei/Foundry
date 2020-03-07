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
using Stack.Patterns.Service.Models;
using System.Linq;

namespace Foundry.Buckets.ViewModels
{
    public class ImportFileDataFilter : IDataFilter<ImportFileSummary>
    {
        public string Sort { get; set; } = "alphabetic";

        public string Term { get; set; } = string.Empty;

        public int Skip { get; set; }

        public int Take { get; set; }

        public string Filter { get; set; } = string.Empty;

        public IQueryable<ImportFileSummary> FilterQuery(IQueryable<ImportFileSummary> query, IStackIdentity identity)
        {
            return query;
        }

        public IQueryable<ImportFileSummary> SearchQuery(IQueryable<ImportFileSummary> query)
        {
            if (string.IsNullOrWhiteSpace(Term))
                return query;

            var term = Term.ToLower().Trim();

            return query.Where(f => f.Name.ToLower().Contains(term));
        }

        public IOrderedQueryable<ImportFileSummary> SortQuery(IQueryable<ImportFileSummary> query)
        {
            if (string.IsNullOrWhiteSpace(Sort))
            {
                Sort = "name";
            }

            var sort = Sort.ToLower().Trim().Replace("-", "");
            var desc = Sort.StartsWith("-") ? true : false;

            switch (sort)
            {
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

