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
using Foundry.Orders.Data;
using System;
using System.Security.Claims;


namespace Foundry.Orders.Identity
{
    public class ClientIdentityStrategy : ISketchIdentityStrategy
    {
        public ClaimsPrincipal ClaimsPrincipal { get; set; }
        public OrdersDbContext DbContext { get; }
        public string Subject { get; }
        public string Name { get; }
        public string ClientId { get; }

        public ClientIdentityStrategy(OrdersDbContext db, ClaimsPrincipal claimsPrincipal)
        {
            DbContext = db ?? throw new ArgumentNullException(nameof(db));
            ClaimsPrincipal = claimsPrincipal ?? throw new ArgumentNullException(nameof(claimsPrincipal));

            Subject = claimsPrincipal.FindFirst(JwtClaimTypes.Subject)?.Value;
            Name = claimsPrincipal.FindFirst(JwtClaimTypes.Name)?.Value;
            ClientId = claimsPrincipal.FindFirst(JwtClaimTypes.ClientId)?.Value;
        }

        public IStackIdentity Add()
        {
            throw new NotImplementedException();
        }

        public IStackIdentity Get()
        {
            throw new NotImplementedException();
        }

        public IStackIdentity Update()
        {
            throw new NotImplementedException();
        }
    }
}

