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
using IdentityModel.Client;
using Foundry.Analytics.Options;
using Stack.Http.Identity;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Foundry.Analytics.Services
{
    public class IntegrationsService : Service
    {
        IntegrationsOptions _integrationsOptions;

        public IntegrationsService(IStackIdentityResolver identityResolver, IMapper mapper, IntegrationsOptions integrationsOptions)
            : base(identityResolver, mapper)
        {
            _integrationsOptions = integrationsOptions ?? throw new ArgumentNullException(nameof(integrationsOptions));
        }

        public async Task<string> GetExerciseLeaderboardResults(string exerciseGuid, int numRecords)
        {
            string data = string.Empty;
            TokenResponse tokenResponse = await RequestSTEPTokenAsync();

            using (var client = new HttpClient())
            {
                client.SetBearerToken(tokenResponse.AccessToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string requestUri = new Uri(string.Format("{0}/{1}/{2}", _integrationsOptions.ExerciseLeaderboardUrl, exerciseGuid, numRecords)).ToString();

                var response = await client.GetAsync(requestUri);
                data = await response.Content.ReadAsStringAsync();
            }

            return data;
        }

        async Task<TokenResponse> RequestSTEPTokenAsync()
        {
            var disco = await DiscoveryClient.GetAsync(_integrationsOptions.STEPAuthority);
            if (disco.IsError) throw new Exception(disco.Error);

            var client = new TokenClient(disco.TokenEndpoint, _integrationsOptions.STEPClientId, _integrationsOptions.STEPClientSecret, null, AuthenticationStyle.PostValues);

            return await client.RequestResourceOwnerPasswordAsync(_integrationsOptions.STEPUserName, _integrationsOptions.STEPPassword, _integrationsOptions.STEPScope);
        }
    }
}
