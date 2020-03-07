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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Buckets.ViewModels
{
    /// <summary>
    /// base data filter class
    /// </summary>
    /// <typeparam name="TEntityModel"></typeparam>
    public abstract class DataFilter<TEntityModel> : IDataFilter<TEntityModel>
        where TEntityModel : class
    {
        /// <summary>
        /// sort value
        /// </summary>
        public string Sort { get; set; } = "alphabetic";

        /// <summary>
        /// search term
        /// </summary>
        public string Term { get; set; } = string.Empty;

        /// <summary>
        /// skip records
        /// </summary>
        public int Skip { get; set; }

        /// <summary>
        /// take records
        /// </summary>
        public int Take { get; set; }

        /// <summary>
        /// filter key=value
        /// </summary>
        public string Filter { get; set; } = string.Empty;

        /// <summary>
        /// process query with filter
        /// </summary>
        /// <param name="query"></param>
        /// <param name="identity"></param>
        /// <returns></returns>
        public virtual IQueryable<TEntityModel> FilterQuery(IQueryable<TEntityModel> query, IStackIdentity identity)
        {
            return query;
        }

        /// <summary>
        /// process query with search
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual IQueryable<TEntityModel> SearchQuery(IQueryable<TEntityModel> query)
        {
            return query;
        }

        /// <summary>
        /// process query with sort
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public abstract IOrderedQueryable<TEntityModel> SortQuery(IQueryable<TEntityModel> query);
    }
}

