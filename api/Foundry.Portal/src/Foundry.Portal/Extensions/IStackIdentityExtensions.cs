/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Foundry.Portal.Identity;
using Stack.Http.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foundry.Portal
{
    public static class IStackIdentityExtensions
    {
        public static string GetName(this IStackIdentity identity)
        {
            if (identity != null)
            {
                if (identity is ProfileIdentity profileIdentity)
                    return profileIdentity.Profile.Name;

                if (identity is ClientIdentity clientIdentity)
                    return clientIdentity.Client.Name;
            }

            return string.Empty;
        }

        public static int GetId(this IStackIdentity identity)
        {
            if (identity != null)
            {
                if (identity is ProfileIdentity profileIdentity)
                    return profileIdentity.Profile.Id;

                if (identity is ClientIdentity clientIdentity)
                    return clientIdentity.Client.Id;
            }

            return 0;
        }
    }
}

