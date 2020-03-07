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
using Stack.Http.Identity;
using Foundry.Orders.Cache;
using Foundry.Orders.Data;
using Foundry.Orders.Data.Entities;
using System;
using System.Linq;
using System.Security.Claims;

namespace Foundry.Orders.Identity
{
    public class ProfileIdentityStrategy : ISketchIdentityStrategy
    {
        public ClaimsPrincipal ClaimsPrincipal { get; set; }
        public OrdersDbContext DbContext { get; }
        public IProfileCache AccountCache { get; }
        public string Subject { get; }
        public string Name { get; }
        public string ClientId { get; }

        public ProfileIdentityStrategy(OrdersDbContext db, IProfileCache accountCache, ClaimsPrincipal claimsPrincipal)
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
            var profile = new Profile
            {
                GlobalId = Subject,
                Name = Name ?? "Anonymous"
            };

            DbContext.Profiles.Add(profile);
            DbContext.SaveChanges();

            return ToIdentity(profile);
        }

        public IStackIdentity Get()
        {
            var account = AccountCache.Get(Subject);

            if (account == null)
            {
                account = DbContext.Profiles.SingleOrDefault(p => p.GlobalId.ToLower() == Subject);

                if (account == null)
                    return null;
            }

            return ToIdentity(account);
        }

        public IStackIdentity Update()
        {
            var profile = DbContext.Profiles.SingleOrDefault(p => p.GlobalId.ToLower() == Subject);

            if (profile.Name != Name)
            {
                profile.Name = Name;
                DbContext.SaveChanges();
                AccountCache.Remove(profile.GlobalId);
            }

            return ToIdentity(profile);
        }

        IStackIdentity ToIdentity(Profile account)
        {
            if (account == null)
                return null;

            var identity = new ProfileIdentity() { Id = Subject, ClientId = ClientId, Subject = Subject, Profile = account, Name = Name };

            if (account.IsAdministrator)
            {
                identity.Permissions = new string[] { "administrator" };
            }

            return identity;
        }
    }
}

