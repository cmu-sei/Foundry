/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using IdentityModel.Client;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Stack.Http.Options;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Stack.Communication.Notifications
{
    public class CommunicationClient
    {
        AuthorizationOptions AuthorizationOptions { get; }
        CommunicationOptions CommunicationOptions { get; }

        public CommunicationClient(AuthorizationOptions authorizationOptions, CommunicationOptions communicationOptions)
        {
            AuthorizationOptions = authorizationOptions;
            CommunicationOptions = communicationOptions;
        }

        TokenResponse TokenResponse { get; set; }
        DateTime TokenExpires { get; set; }

        async Task<TokenResponse> GetTokenAsync(bool alwaysGetToken = false)
        {
            if (TokenResponse == null || TokenExpires > DateTime.UtcNow || alwaysGetToken)
            {
                var discoveryResponse = await DiscoveryClient.GetAsync(AuthorizationOptions.Authority);

                if (discoveryResponse.IsError)
                    throw new SecurityTokenException(discoveryResponse.Error);

                var client = new TokenClient(discoveryResponse.TokenEndpoint, AuthorizationOptions.ClientId, AuthorizationOptions.ClientSecret);
                var response = await client.RequestClientCredentialsAsync(AuthorizationOptions.AuthorizationScope);

                if (response.IsError)
                    throw new SecurityTokenException(response.Error);

                TokenResponse = response;
                TokenExpires = DateTime.UtcNow.AddSeconds(response.ExpiresIn);
            }

            return TokenResponse;
        }

        public async Task<bool> PostAsync(NotificationCreate notification, bool alwaysGetToken = false)
        {
            using (var client = new HttpClient { BaseAddress = new Uri(CommunicationOptions.CommunicationUrl) })
            {
                var responseToken = await GetTokenAsync(alwaysGetToken);
                client.SetBearerToken(responseToken.AccessToken);

                string json = JsonConvert.SerializeObject(notification);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.PostAsync("api/notifications", new StringContent(json, Encoding.UTF8, "application/json"));

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public async Task<bool> DeleteAsync(string id, bool alwaysGetToken = false)
        {
            using (var client = new HttpClient { BaseAddress = new Uri(CommunicationOptions.CommunicationUrl) })
            {
                var responseToken = await GetTokenAsync(alwaysGetToken);
                client.SetBearerToken(responseToken.AccessToken);

                var response = await client.DeleteAsync("api/notification/" + id);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return true;
                    default:
                        return false;
                }
            }
        }
    }
}

