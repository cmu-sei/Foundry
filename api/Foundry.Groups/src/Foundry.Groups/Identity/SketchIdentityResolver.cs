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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Foundry.Groups.Data;
using Stack.Http.Identity;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Foundry.Groups.Identity
{
    /// <summary>
    /// resolves authenticated user based on claims in the request
    /// </summary>
    public class SketchIdentityResolver : HttpIdentityResolver
    {
        GroupsDbContext DbContext { get; }
        ILogger<SketchIdentityResolver> Logger { get; }

        /// <summary>
        /// creates an instance of SketchIdentityResolver
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="dbContext"></param>
        /// <param name="cache"></param>
        /// <param name="logger"></param>
        public SketchIdentityResolver(IHttpContextAccessor httpContextAccessor, GroupsDbContext dbContext, ILogger<SketchIdentityResolver> logger)
            : base(httpContextAccessor)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// get identity
        /// </summary>
        /// <returns></returns>
        public override async Task<IStackIdentity> GetIdentityAsync()
        {
            var resolver = GetStrategy(HttpContextAccessor.HttpContext.User);

            return resolver?.Get() ?? resolver?.Add() ?? resolver?.Update() ?? null;
        }

        ISketchIdentityStrategy GetStrategy(ClaimsPrincipal claimsPrincipal)
        {
            var clientId = claimsPrincipal.FindFirst(JwtClaimTypes.ClientId)?.Value;

            if (string.IsNullOrWhiteSpace(clientId))
                return null;

            var subject = claimsPrincipal.FindFirst(JwtClaimTypes.Subject)?.Value;

            if (string.IsNullOrWhiteSpace(subject))
                return new ClientIdentityStrategy(DbContext, claimsPrincipal);

            return new ProfileIdentityStrategy(DbContext, claimsPrincipal);
        }
    }
}

