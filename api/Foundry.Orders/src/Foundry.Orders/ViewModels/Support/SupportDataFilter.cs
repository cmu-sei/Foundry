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
    public class SupportDataFilter : IDataFilter<Support>
    {
        public string Term { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public string Filter { get; set; }
        public string Sort { get; set; }

        public IQueryable<Support> FilterQuery(IQueryable<Support> query, IStackIdentity identity)
        {
            return query;
        }

        public IQueryable<Support> SearchQuery(IQueryable<Support> query)
        {
            return query;
        }

        public IOrderedQueryable<Support> SortQuery(IQueryable<Support> query)
        {
            return query.OrderBy(o => o.Id); //.Name == "Other").OrderBy(o => o.Name);
        }
    }
}
