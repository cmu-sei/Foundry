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
    public class ClientIdentityStrategy : ISketchIdentityStrategy
    {
        public ClaimsPrincipal ClaimsPrincipal { get; set; }
        public SketchDbContext DbContext { get; }
        public string Subject { get; }
        public string Name { get; }
        public string ClientId { get; }

        IClientCache ClientCache { get; }

        public ClientIdentityStrategy(SketchDbContext db, ClaimsPrincipal claimsPrincipal, IClientCache clientCache)
        {
            DbContext = db;
            ClientCache = clientCache;
            ClaimsPrincipal = claimsPrincipal;

            Subject = claimsPrincipal.FindFirst(JwtClaimTypes.Subject)?.Value;
            Name = claimsPrincipal.FindFirst(JwtClaimTypes.Name)?.Value;
            ClientId = claimsPrincipal.FindFirst(JwtClaimTypes.ClientId)?.Value;
        }

        public IStackIdentity Add()
        {
            var client = new Client
            {
                GlobalId = ClientId,
                Name = Name ?? ClientId
            };

            DbContext.Clients.Add(client);
            DbContext.SaveChanges();

            return ConvertToIdentity(client);
        }

        public IStackIdentity Get()
        {
            var client = ClientCache.Get(ClientId);

            if (client == null)
            {
                client = DbContext.Clients.SingleOrDefault(p => p.GlobalId == ClientId);

                ClientCache.Set(ClientId, client);
            }

            return ConvertToIdentity(client);
        }

        public IStackIdentity Update()
        {
            var client = DbContext.Clients.SingleOrDefault(p => p.GlobalId == ClientId);

            return ConvertToIdentity(client);
        }

        IStackIdentity ConvertToIdentity(Client client)
        {
            if (client == null) return null;

            return new ClientIdentity
            {
                Id = client.GlobalId,
                Client = client,
                Permissions = client.Permissions.ToArray()
            };
        }
    }
}

