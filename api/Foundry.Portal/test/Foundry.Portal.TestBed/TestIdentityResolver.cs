/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Foundry.Portal.Data;
using Foundry.Portal.Data.Entities;
using Foundry.Portal.Identity;
using Stack.Http.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foundry.Portal.TestBed
{
    public class TestIdentityResolver : IStackIdentityResolver
    {
        Profile _profile;

        public TestIdentityResolver(Profile profile)
        {
            _profile = profile;
        }

        public async Task<IStackIdentity> GetIdentityAsync()
        {
            var permissions = new List<string>();

            if (_profile.Permissions.HasFlag(SystemPermissions.Administrator))
                permissions.Add(SystemPermissions.Administrator.ToString().ToLower());

            if (_profile.Permissions.HasFlag(SystemPermissions.PowerUser))
                permissions.Add(SystemPermissions.PowerUser.ToString().ToLower());

            return new ProfileIdentity { Id = _profile.GlobalId, Permissions = permissions.ToArray(), Profile = _profile };
        }
    }
}
