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
using System.Linq;

namespace Foundry.Orders.ViewModels
{
    public class RankDataFilter : IDataFilter<Rank>
    {
        public string Term { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public string Filter { get; set; }
        public string Sort { get; set; }

        public IQueryable<Rank> FilterQuery(IQueryable<Rank> query, IStackIdentity identity)
        {
            return query;
        }

        public IQueryable<Rank> SearchQuery(IQueryable<Rank> query)
        {
            return query;
        }

        public IOrderedQueryable<Rank> SortQuery(IQueryable<Rank> query)
        {
            // padding numbers after -
            // replace "W" with arbitrary "N" for sort to get proper sorting before "O"

            return query.OrderBy(r => r.Grade.Contains("GS")
                    ? (r.Grade.Length > 4 ? r.Grade : r.Grade.Replace("-", "-0"))
                    : (r.Grade.Length > 3 ? r.Grade.Replace("W", "N") : r.Grade.Replace("W", "N").Replace("-", "-0")));
        }
    }
}

