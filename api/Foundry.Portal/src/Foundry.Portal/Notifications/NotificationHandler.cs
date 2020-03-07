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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Foundry.Portal.Data;
using Foundry.Portal.Events;
using Foundry.Portal.ViewModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Stack.Http.Options;

namespace Foundry.Portal.Notifications
{
    public class NotificationHandler : INotificationHandler
    {
        DbContextOptions DbContextOptions { get; }
        CommunicationOptions CommunicationOptions { get; }
        AuthorizationOptions AuthorizationOptions { get; }
        TokenResponse TokenResponse { get; set; }
        DateTime TokenExpires { get; set; }

        public NotificationHandler(IConfiguration configuration, AuthorizationOptions authorizationOptions, CommunicationOptions communicationOptions)
        {
            DbContextOptions = new DbContextOptionsBuilder().UseConfiguredDatabase(configuration).Options;
            AuthorizationOptions = authorizationOptions;
            CommunicationOptions = communicationOptions;
        }

        public async Task<IEnumerable<DomainEventResult>> Process(IDomainEvent e)
        {
            var results = new List<DomainEventResult>();

            using (var db = new SketchDbContext(DbContextOptions))
            {
                var result = new DomainEventResult()
                {
                    Start = DateTime.UtcNow,
                    Event = e
                };

                try
                {
                    var factory = new NotificationFactory(db, CommunicationOptions);

                    var strategy = factory.GetNotificationStrategy(e);

                    var notification = await strategy.Build();

                    switch (e.Type)
                    {
                        case DomainEventType.ContentDelete:
                            await DeleteNotificationsAsync(notification.GlobalId);
                            break;
                        default:
                            break;
                    }

                    await PostNotificationAsync(notification);
                }
                catch (Exception ex)
                {
                    result.Exception = ex;
                }

                result.Finish = DateTime.UtcNow;
                results.Add(result);
            }

            return results;
        }

        async Task<TokenResponse> GetTokenAsync(bool always = false)
        {
            if (TokenResponse == null || TokenExpires > DateTime.UtcNow || always)
            {
                var discoveryClient = await DiscoveryClient.GetAsync(AuthorizationOptions.Authority);

                if (discoveryClient.IsError)
                    throw new SecurityTokenException(discoveryClient.Error);

                var client = new TokenClient(discoveryClient.TokenEndpoint, AuthorizationOptions.ClientId, AuthorizationOptions.ClientSecret);
                var response = await client.RequestClientCredentialsAsync(AuthorizationOptions.AuthorizationScope);

                if (response.IsError)
                    throw new SecurityTokenException(response.Error);

                TokenResponse = response;
                TokenExpires = DateTime.UtcNow.AddSeconds(response.ExpiresIn);
            }

            return TokenResponse;
        }

        public async Task<bool> PostNotificationAsync(NotificationCreate notification)
        {
            using (var client = new HttpClient { BaseAddress = new Uri(CommunicationOptions.Url) })
            {
                client.SetBearerToken((await GetTokenAsync(true)).AccessToken);

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

        async Task<bool> DeleteNotificationsAsync(string id)
        {
            using (var client = new HttpClient { BaseAddress = new Uri(CommunicationOptions.Url) })
            {
                client.SetBearerToken((await GetTokenAsync(true)).AccessToken);

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
