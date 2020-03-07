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
using Stack.Http;
using Xunit;

namespace Foundry.Groups.Tests
{
    /// <summary>
    /// Important note: Fixtures can be shared across assemblies, but collection definitions must be in the same assembly as the test that uses them.
    /// </summary>
    [CollectionDefinition("AutoMapper")]
    public class AutoMapperCollection : ICollectionFixture<AutoMapperFixture>
    {
    }


    public class AutoMapperFixture : IDisposable
    {

        public AutoMapperFixture()
        {
            var type = typeof(AutoMapper.Profile);

            AutoMapper.Mapper.Initialize(configuration =>
            {
                type.ProcessTypeOf("Foundry.Groups", (profile) => {
                    var instance = Activator.CreateInstance(profile) as AutoMapper.Profile;
                    configuration.AddProfile(instance);
                });
            });
        }

        public void Dispose()
        {

        }
    }
}

