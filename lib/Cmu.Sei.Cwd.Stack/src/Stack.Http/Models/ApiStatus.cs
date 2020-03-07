/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;

namespace Stack.Http.Models
{
    public class ApiStatus
    {
        string[] AssemblyNames { get; }
        public ApiStatus() { }

        public ApiStatus(params string[] assemblyNames)
        {
            AssemblyNames = assemblyNames ?? new string[] { };
        }

        ApiStatusModule GetAssemblyStatusVersion(string assemblyName)
        {
            var statusVersion = new ApiStatusModule()
            {
                Name = assemblyName
            };

            try
            {
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(assemblyName));

                statusVersion.Version = assembly.GetVersion();
                statusVersion.Build = assembly.GetBuildDateTime();
            }
            catch
            {
                statusVersion.Name = string.Format("Could not load assembly '{0}'", assemblyName);
            }

            return statusVersion;
        }

        public bool Available { get; set; } = true;

        public IEnumerable<ApiStatusModule> Modules
        {
            get
            {
                var modules = new List<ApiStatusModule>();
                foreach (var name in AssemblyNames)
                {
                    modules.Add(GetAssemblyStatusVersion(name));
                }
                return modules;
            }
        }
    }
}
