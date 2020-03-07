/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net;

namespace Stack.Http.Identity.Attributes
{
    /// <summary>
    /// stack authorize with IIdentityResolver integration
    /// </summary>
    public class StackAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        StackAuthorizeType StackAuthorizeType { get; } = StackAuthorizeType.RequireAll;
        string[] RequiresPermission { get; } = new string[] { };

        /// <summary>
        /// create an instance of stack authorize
        /// </summary>
        public StackAuthorizeAttribute() { }

        /// <summary>
        /// crate an instance of stack authorize with permission checks
        /// </summary>
        /// <param name="stackAuthorizeType"></param>
        /// <param name="requiresPermission"></param>
        public StackAuthorizeAttribute(StackAuthorizeType stackAuthorizeType, params string[] requiresPermission)
        {
            StackAuthorizeType = stackAuthorizeType;
            RequiresPermission = requiresPermission;
        }

        /// <summary>
        /// standardize array by removing empty and duplicates then order
        /// </summary>
        /// <param name="values"></param>
        /// <returns>array of standardized strings</returns>
        static string[] Standardize(string[] values)
        {
            return (values ?? new string[] { })
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .Select(v => v.ToLower().Trim())
                .Distinct()
                .OrderBy(v => v).ToArray();
        }

        /// <summary>
        /// determine if identity has what is needed
        /// </summary>
        /// <param name="stackAuthorizeType"></param>
        /// <param name="needs"></param>
        /// <param name="has"></param>
        /// <returns>true if identity meets requirement</returns>
        static bool IsValid(StackAuthorizeType stackAuthorizeType, string[] needs, string[] has)
        {
            var needsPermissions = Standardize(needs);
            var hasPermissions = Standardize(has);

            if (needsPermissions.Any())
            {
                var required = stackAuthorizeType == StackAuthorizeType.RequireAll ? needsPermissions.Count() : 1;
                return hasPermissions.Intersect(needsPermissions).Count() >= required;
            }

            return true;
        }

        /// <summary>
        /// authorization check
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            // handled by AuthorizeAttribute
            if (!user.Identity.IsAuthenticated)
                return;

            var identityResolver = context.HttpContext.RequestServices.GetService<IStackIdentityResolver>();

            var identity = identityResolver.GetIdentityAsync().Result;

            if (identity == null)
            {
                context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
                return;
            }

            if (!IsValid(StackAuthorizeType, RequiresPermission, identity.Permissions))
            {
                context.Result = new StatusCodeResult((int)HttpStatusCode.Unauthorized);
                return;
            }
        }
    }

    /// <summary>
    /// stack authorize types
    /// </summary>
    public enum StackAuthorizeType
    {
        /// <summary>
        /// requires all listed permissions
        /// </summary>
        RequireAll,
        /// <summary>
        /// requires any listed permissions
        /// </summary>
        RequireAny
    }
}

