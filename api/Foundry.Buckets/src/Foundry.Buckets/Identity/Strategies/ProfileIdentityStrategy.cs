/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using IdentityModel;
using Foundry.Buckets.Cache;
using Foundry.Buckets.Data;
using Foundry.Buckets.Data.Entities;
using Stack.Http.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Foundry.Buckets.Identity
{
    public class ProfileIdentityStrategy : ISketchIdentityStrategy
    {
        public ClaimsPrincipal ClaimsPrincipal { get; set; }
        public BucketsDbContext DbContext { get; }

        public IAccountCache AccountCache { get; }
        public string Subject { get; }
        public string Name { get; }
        public string ClientId { get; }

        public ProfileIdentityStrategy(BucketsDbContext db, IAccountCache accountCache, ClaimsPrincipal claimsPrincipal)
        {
            DbContext = db ?? throw new ArgumentNullException(nameof(db));
            AccountCache = accountCache ?? throw new ArgumentNullException(nameof(accountCache));
            ClaimsPrincipal = claimsPrincipal ?? throw new ArgumentNullException(nameof(claimsPrincipal));

            Subject = claimsPrincipal.FindFirst(JwtClaimTypes.Subject)?.Value;
            Name = claimsPrincipal.FindFirst(JwtClaimTypes.Name)?.Value;
            ClientId = claimsPrincipal.FindFirst(JwtClaimTypes.ClientId)?.Value;
        }

        public IStackIdentity Add()
        {
            var account = new Account
            {
                GlobalId = Subject.ToLower(),
                Name = Name ?? "Anonymous"
            };

            DbContext.Accounts.Add(account);
            DbContext.SaveChanges();

            return ToIdentity(account);
        }

        public IStackIdentity Get()
        {
            var account = AccountCache.GetOrCreate(Subject);

            if (account == null)
            {
                account = DbContext.Accounts.SingleOrDefault(p => p.GlobalId.ToLower() == Subject.ToLower());

                if (account == null)
                    return null;
            }

            return ToIdentity(account);
        }

        public IStackIdentity Update()
        {
            var account = DbContext.Accounts.SingleOrDefault(p => p.GlobalId.ToLower() == Subject.ToLower());

            if (account.Name != Name)
            {
                account.Name = Name;
                DbContext.SaveChanges();
                AccountCache.Remove(account.GlobalId);
            }

            return ToIdentity(account);
        }

        IStackIdentity ToIdentity(Account account)
        {
            var permissions = new List<string>();
            if (account.IsAdministrator) permissions.Add("administrator");

            return new ProfileIdentity { Id = Subject, Name = Name, Subject = Subject, ClientId = ClientId, Permissions = permissions.ToArray() };
        }
    }
}

