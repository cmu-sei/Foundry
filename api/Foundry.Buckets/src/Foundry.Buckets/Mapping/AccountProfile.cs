/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using AutoMapper;
using Foundry.Buckets.Data.Entities;
using Foundry.Buckets.Extensions;
using Foundry.Buckets.ViewModels;
using System.Linq;

namespace Foundry.Buckets.Mapping
{
    /// <summary>
    /// auto mapper profile for accounts
    /// </summary>
    public class AccountProfile : Profile
    {
        /// <summary>
        /// create an instance of bucket source profile
        /// </summary>
        public AccountProfile()
        {
            CreateMap<Account, AccountDetail>()
                .AfterMap((src, dest, res) => {
                    var identity = res.GetIdentity();

                    if (identity != null)
                    {
                        foreach (var ba in src.BucketAccounts)
                        {
                            if (identity.Permissions.Contains("administrator") || ba.AccountId == identity.Id)
                            {
                                dest.Buckets.Add(new AccountDetailBucket
                                {
                                    Access = ba.SetAccess(),
                                    BucketAccessType = ba.BucketAccessType,
                                    Id = ba.BucketId,
                                    IsDefault = ba.IsDefault,
                                    BucketSharingType = ba.Bucket.BucketSharingType,
                                    Name = ba.Bucket.Name,
                                    Slug = ba.Bucket.Slug,
                                });
                            }
                        }
                    }
                });

            CreateMap<Account, AccountSummary>()
                .AfterMap((src, dest, res) => {
                    var identity = res.GetIdentity();

                    if (identity != null)
                    {
                        dest.BucketCount = src.BucketAccounts.Count();
                    }
                });
        }
    }
}

