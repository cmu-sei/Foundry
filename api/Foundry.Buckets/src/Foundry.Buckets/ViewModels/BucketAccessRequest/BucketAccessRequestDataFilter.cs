/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Foundry.Buckets.Data.Entities;
using Stack.Http.Identity;
using Stack.Patterns.Service;
using System.Linq;

namespace Foundry.Buckets.ViewModels
{
    /// <summary>
    /// bucket access request data filter
    /// </summary>
    public class BucketAccessRequestDataFilter : DataFilter<BucketAccessRequest>
    {
        /// <summary>
        /// filter query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="identity"></param>
        /// <returns></returns>
        public override IQueryable<BucketAccessRequest> FilterQuery(IQueryable<BucketAccessRequest> query, IStackIdentity identity)
        {
            var keyValues = Filter.ToFilterKeyValues();

            foreach (var filter in keyValues)
            {
                var intValues = filter.ToIntValues();
                var key = filter.Key.Replace("!", "");
                var not = filter.Key.StartsWith("!");

                switch (key)
                {
                    case "type":
                        var bucketAccessRequestType = filter.ToEnumValues<BucketAccessRequestType>();
                        query = not
                            ? query.Where(bar => !bucketAccessRequestType.Contains(bar.Type))
                            : query.Where(bar => bucketAccessRequestType.Contains(bar.Type));
                        break;
                    default:
                        break;
                }
            }

            return query;
        }

        /// <summary>
        /// sort query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public override IOrderedQueryable<BucketAccessRequest> SortQuery(IQueryable<BucketAccessRequest> query)
        {
            if (string.IsNullOrWhiteSpace(Sort))
            {
                Sort = "recent";
            }

            var sort = Sort.ToLower().Trim().Replace("-", "");
            var desc = Sort.StartsWith("-") ? true : false;

            switch (sort)
            {
                case "recent":
                default:
                    return desc
                        ? query.OrderByDescending(b => b.Created)
                        : query.OrderBy(b => b.Created);
            }
        }
    }
}
