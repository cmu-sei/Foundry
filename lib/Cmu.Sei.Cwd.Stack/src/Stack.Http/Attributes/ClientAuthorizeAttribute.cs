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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Net;
using System.Security.Claims;

namespace Stack.Http.Attributes
{
    public class ClientAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        readonly string[] _allowedClientIds;

        public ClientAuthorizeAttribute(params string[] allowedClientIds)
        {
            _allowedClientIds = allowedClientIds ?? new string[] { };
        }

        string GetClientId(ClaimsPrincipal user)
        {
            string globalId = user.FindFirst(JwtClaimTypes.Subject)?.Value;
            string name = user.FindFirst(JwtClaimTypes.Name)?.Value;
            string clientId = user.FindFirst(JwtClaimTypes.ClientId)?.Value;

            // no clientId present
            if (string.IsNullOrWhiteSpace(clientId))
                return null;

            // clientId present but no profile globalId and name defined
            if (string.IsNullOrWhiteSpace(globalId) && string.IsNullOrWhiteSpace(name))
                return clientId;

            return null;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (user.Identity.IsAuthenticated)
            {
                var clientId = GetClientId(context.HttpContext.User);

                if (clientId == null)
                {
                    context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
                    return;
                }

                if (_allowedClientIds.Any() && !_allowedClientIds.Contains(clientId))
                {
                    context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
                    return;
                }
            }

            return;
        }
    }
}
