/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Newtonsoft.Json;
using Foundry.Portal.ViewModels;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Foundry.Portal.WebHooks
{
    /// <summary>
    /// Queries the ApiUrl to retrieve Extensions
    /// </summary>
    public class ExtensionResolver : IExtensionResolver
    {
        public ExtensionOptions _extensionOptions { get; set; }

        public ExtensionResolver(ExtensionOptions extensionOptions)
        {
            _extensionOptions = extensionOptions ?? throw new ArgumentNullException("extensionOptions");
        }

        /// <summary>
        /// get all extensions
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<IdentityClientModel>> GetPublishedClients()
        {
            var identityClients = new List<IdentityClientModel>();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync(_extensionOptions.ApiUrl);
                if (response.IsSuccessStatusCode)
                {
                    identityClients = JsonConvert.DeserializeObject<List<IdentityClientModel>>(await response.Content.ReadAsStringAsync());
                }
            }

            return identityClients;
        }

        /// <summary>
        /// get all extensions webhook listeners
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<WebHookTarget>> GetWebHookTargets()
        {
            var clients = new List<WebHookTarget>();

            using (var client = new HttpClient())
            {
                //TODO: Pass a client token for auth

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync(_extensionOptions.TargetsUrl);
                if (response.IsSuccessStatusCode)
                {
                    clients = JsonConvert.DeserializeObject<List<WebHookTarget>>(await response.Content.ReadAsStringAsync());
                }
            }

            return clients;
        }
    }
}
