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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Foundry.Buckets.Extensions
{
    /// <summary>
    /// statck identity extensions
    /// </summary>
    public static class IStackIdentityExtensions
    {
        /// <summary>
        /// determine bucket access for identity from file
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static List<string> SetAccess(this IStackIdentity identity, File file)
        {
            var bucketAccount = file.Bucket.BucketAccounts.SingleOrDefault(bs => bs.AccountId.ToLower() == identity.Id.ToLower());
            return SetAccess(identity, bucketAccount);
        }

        /// <summary>
        /// determine bucket access for identity from bucket
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="bucket"></param>
        /// <returns></returns>
        public static List<string> SetAccess(this IStackIdentity identity, Bucket bucket)
        {
            var bucketAccount = bucket.BucketAccounts.SingleOrDefault(bs => bs.AccountId.ToLower() == identity.Id.ToLower());
            return SetAccess(identity, bucketAccount);
        }

        /// <summary>
        /// determine bucket access for identity from bucket account
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="bucketAccount"></param>
        /// <returns></returns>
        public static List<string> SetAccess(this IStackIdentity identity, BucketAccount bucketAccount)
        {
            var access = new List<string>();
            if (identity != null)
            {
                if (identity.Permissions.Contains("administrator"))
                {
                    access.AddRange(new string[] { "edit", "delete" });
                }
                else if (bucketAccount != null)
                {
                    access.AddRange(SetAccess(bucketAccount));
                }
            }

            return access;
        }

        /// <summary>
        /// determine bucket access for identity from bucket account
        /// </summary>
        /// <param name="bucketAccount"></param>
        /// <returns></returns>
        public static List<string> SetAccess(this BucketAccount bucketAccount)
        {
            var access = new List<string>();
            if (bucketAccount != null)
            {
                if (bucketAccount.BucketAccessType == BucketAccessType.Owner)
                    access.AddRange(new string[] { "edit", "delete" });
                if (bucketAccount.BucketAccessType == BucketAccessType.Manager)
                    access.AddRange(new string[] { "edit" });
            }

            return access;
        }
    }
}

