/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Foundry.Buckets.Data;
using Foundry.Buckets.Data.Entities;
using Stack.Http.Identity;
using System.Linq;

namespace Foundry.Buckets.Security
{
    /// <summary>
    /// mediator class to determine bucket access for identities
    /// </summary>
    public class BucketPermissionMediator : PermissionMediator<Bucket>
    {
        readonly BucketsDbContext _db;

        /// <summary>
        /// create an instance of bucket permission mediator
        /// </summary>
        /// <param name="db"></param>
        /// <param name="identityResolver"></param>
        public BucketPermissionMediator(BucketsDbContext db, IStackIdentityResolver identityResolver)
            : base(identityResolver)
        {
            _db = db;
        }

        /// <summary>
        /// check if user can perform action
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public override bool CanPerformAction(Bucket entity, ActionType action)
        {
            if (IsAdministrator)
                return true;

            switch (action)
            {
                case ActionType.Read:
                    return entity.BucketSharingType == BucketSharingType.Public || entity.BucketAccounts.Any(ba => ba.AccountId == Identity.Id);
                case ActionType.Create:
                    return Identity != null;
                case ActionType.Update:
                    return entity.BucketAccounts.Any(ba => ba.AccountId.ToLower() == Identity.Id.ToLower() && (ba.BucketAccessType == BucketAccessType.Owner || ba.BucketAccessType == BucketAccessType.Manager));
                case ActionType.Delete:
                    return entity.BucketAccounts.Any(ba => ba.AccountId.ToLower() == Identity.Id.ToLower() && ba.BucketAccessType == BucketAccessType.Owner);
                default:
                    return false;
            }
        }

        /// <summary>
        /// process query for identity
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public override IQueryable<Bucket> Process(IQueryable<Bucket> query)
        {
            if (IsAdministrator)
                return query;

            if (Identity == null)
            {
                return query.Where(b => b.BucketSharingType == BucketSharingType.Public);
            }

            return query.Where(b => b.BucketSharingType == BucketSharingType.Public || b.BucketAccounts.Any(ba => ba.AccountId == Identity.Id));
        }
    }
}

