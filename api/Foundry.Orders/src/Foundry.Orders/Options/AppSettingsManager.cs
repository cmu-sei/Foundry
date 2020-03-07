/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;

namespace Foundry.Orders.Options
{
    public static class JsonAppSettings
    {
        public static void Merge(string path, string sourceFile, string destFile)
        {
            string source = Path.Combine(path, sourceFile);
            string destination = Path.Combine(path, destFile);

            JObject jsrc = JObject.Parse(File.ReadAllText(source));
            string[] canonical = jsrc.Descendants().Where(o=>o.Type == JTokenType.Property)
                .Select(o=>o.Path).ToArray();

            JObject jdst = (File.Exists(destination))
                ? JObject.Parse(File.ReadAllText(destination))
                : new JObject();
            string[] custom = jdst.Descendants().Where(o=>o.Type == JTokenType.Property)
                .Select(o=>o.Path).ToArray();

            string[] newOptions = canonical.Except(custom).ToArray();
            if (newOptions.Length > 0)
            {
                JsonMergeSettings mergeSettings = new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Union
                };

                jdst.Merge(jsrc, mergeSettings);
                File.WriteAllText(destination, jdst.ToString());
                
                Console.WriteLine($"Merged options into {Path.GetFileName(destination)}:");
                foreach (string prop in newOptions)
                    Console.WriteLine(prop);
            }
        }
    }
}

