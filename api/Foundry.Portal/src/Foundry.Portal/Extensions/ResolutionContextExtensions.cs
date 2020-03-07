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
using Foundry.Portal.Identity;
using Stack.Http.Identity;
using System;

namespace Foundry.Portal.Extensions
{
    /// <summary>
    /// ResolutionContext extensions for AutoMapper
    /// </summary>
    public static class ResolutionContextExtensions
    {
        public const string ResolutionContextKey = "Identity";

        /// <summary>
        /// get identity from Resolution Context using the defined key
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        public static IStackIdentity GetIdentity(this ResolutionContext res, string resolutionContextKey = null)
        {
            var key = resolutionContextKey ?? ResolutionContextKey;

            if (res.Items.ContainsKey(key))
                return res.Items[key] as IStackIdentity;

            return null;
        }

        /// <summary>
        /// get profile id or client id or 0
        /// </summary>
        /// <param name="res"></param>
        /// <param name="resolutionContextKey"></param>
        /// <returns></returns>
        public static int GetId(this ResolutionContext res, string resolutionContextKey = null)
        {
            var key = resolutionContextKey ?? ResolutionContextKey;

            if (res.Items.ContainsKey(key))
            {
                var identity = res.Items[key] as IStackIdentity;

                if (identity is ProfileIdentity profile)
                    return profile?.Profile?.Id ?? 0;

                if (identity is ClientIdentity client)
                    return client?.Client?.Id ?? 0;
            }

            return 0;
        }

        /// <summary>
        /// get profile GUID or client GUID or empty GUID
        /// </summary>
        /// <param name="res"></param>
        /// <param name="resolutionContextKey"></param>
        /// <returns></returns>
        public static string GetGlobalId(this ResolutionContext res, string resolutionContextKey = null)
        {
            var key = resolutionContextKey ?? ResolutionContextKey;

            if (res.Items.ContainsKey(key))
                return (res.Items[key] as IStackIdentity)?.Id ?? null;

            return Guid.Empty.ToString();
        }
    }
}
