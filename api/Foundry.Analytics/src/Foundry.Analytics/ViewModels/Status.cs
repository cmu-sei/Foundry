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
using System.Reflection;
using System.Runtime.Loader;

namespace Foundry.Analytics.ViewModels
{
    public class Status
    {
        public Status() { }

        public Status(params string[] assemblyNames)
        {
            var names = assemblyNames ?? new string[] { };

            foreach (var name in names)
            {
                Modules.Add(GetAssemblyStatusVersion(name));
            }
        }
        StatusModule GetAssemblyStatusVersion(string assemblyName)
        {
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(assemblyName));

            var statusVersion = new StatusModule()
            {
                Name = assemblyName,
                Version = assembly.GetVersion(),
                Build = assembly.GetBuildDateTime()
            };

            return statusVersion;
        }

        public bool Available { get; set; } = true;

        public List<StatusModule> Modules { get; set; } = new List<StatusModule>();
    }

    public class StatusModule
    {
        public string Name { get; set; }

        public string Version { get; set; }

        public DateTime Build { get; set; }
    }
}

