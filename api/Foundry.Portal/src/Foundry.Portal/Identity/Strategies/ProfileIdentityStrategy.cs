/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using IdentityModel;
using Foundry.Portal.Cache;
using Foundry.Portal.Data;
using Foundry.Portal.Data.Entities;
using Foundry.Portal.Extensions;
using Stack.Http.Identity;

namespace Foundry.Portal.Identity
{
    public class ProfileIdentityStrategy : ISketchIdentityStrategy
    {
        public ClaimsPrincipal ClaimsPrincipal { get; set; }
        public SketchDbContext DbContext { get; }
        public string Subject { get; }
        public string Name { get; }
        public string ClientId { get; }

        IProfileCache ProfileCache { get; }

        public ProfileIdentityStrategy(SketchDbContext db, ClaimsPrincipal claimsPrincipal, IProfileCache profileCache)
        {
            DbContext = db;
            ProfileCache = profileCache;
            ClaimsPrincipal = claimsPrincipal;

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

            var pinned = DbContext.Applications.Where(a => a.IsPinned).ToList();

            if (pinned.Any())
            {
                foreach (var pin in pinned)
                {
                    DbContext.ProfileApplications.Add(new ProfileApplication() { ApplicationId = pin.Id, ProfileId = profile.Id });
                }

                DbContext.SaveChanges();
            }

            ProfileCache.Set(profile.GlobalId, profile);

            return ConvertToIdentity(profile);
        }

        public IStackIdentity Get()
        {
            var profile = ProfileCache.Get(Subject);

            if (profile == null)
            {
                profile = DbContext.Profiles.SingleOrDefault(p => p.GlobalId.ToLower() == Subject);

                if (profile == null) return null;
                if (profile.IsDisabled) return null;

                ProfileCache.Set(Subject, profile);
            }

            return ConvertToIdentity(profile);
        }

        public IStackIdentity Update()
        {
            var profile = DbContext.Profiles.SingleOrDefault(p => p.GlobalId == Subject);

            if (profile.Name != Name)
            {
                profile.Name = Name;
                DbContext.SaveChanges();
                ProfileCache.Remove(profile.GlobalId);
            }

            return ConvertToIdentity(profile);
        }

        IStackIdentity ConvertToIdentity(Profile profile)
        {
            if (profile == null) return null;

            return new ProfileIdentity
            {
                Id = profile.GlobalId,
                Profile = profile,
                Permissions = profile.Permissions.ToArray()
            };
        }
    }
}

