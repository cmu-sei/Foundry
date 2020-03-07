/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Foundry.Portal.Data;
using Foundry.Portal.Data.Entities;
using Foundry.Portal.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Portal.WebHooks
{
    /// <summary>
    /// sends domain events to all extensions that have a WebHookUrl set
    /// </summary>
    public class WebHookHandler : IWebHookHandler
    {
        IExtensionResolver _extensionResolver;
        readonly DbContextOptions _options;

        public WebHookHandler(IExtensionResolver extensionResolver, IConfiguration configuration)
        {
            _extensionResolver = extensionResolver ?? throw new ArgumentNullException("extensionResolver");
            _options = new DbContextOptionsBuilder().UseConfiguredDatabase(configuration).Options;
        }

        public async Task<IEnumerable<DomainEventResult>> Process(IDomainEvent @event)
        {
            var responses = new List<DomainEventResult>();

            var webHookTargets = await _extensionResolver.GetWebHookTargets();

            var functionalWebHookTargets = webHookTargets.Where(e => !string.IsNullOrWhiteSpace(e.Uri));

            using (var client = new HttpClient())
            {
                foreach (var extension in functionalWebHookTargets)
                {
                    var eventResponse = new DomainEventResult()
                    {
                        Start = DateTime.UtcNow,
                        Event = @event,
                        Listener = extension
                    };

                    try
                    {
                        string json = JsonConvert.SerializeObject(@event);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        eventResponse.Response = await client.PostAsync(extension.Uri, new StringContent(json, Encoding.UTF8, "application/json"));

                        switch (eventResponse.Response.StatusCode)
                        {
                            case HttpStatusCode.OK:
                            case HttpStatusCode.Created:
                                var content = await eventResponse.Response.Content.ReadAsStringAsync();
                                var webHookResponse = JsonConvert.DeserializeObject<WebHookResponse>(content);
                                if (webHookResponse != null)
                                {
                                    switch (@event.Type)
                                    {
                                        case DomainEventType.ContentAdd:
                                        case DomainEventType.ContentUpdate:
                                            await HandleContentResponse(webHookResponse, @event.Id);
                                            break;
                                        case DomainEventType.PlaylistAdd:
                                        case DomainEventType.PlaylistUpdate:
                                            await HandlePlaylistResponse(webHookResponse, @event.Id);
                                            break;
                                    }
                                }
                                break;
                            default:
                                throw new HttpRequestException(string.Format("Web Hook '{0}' returned unhandled code '{1}'",
                                    extension.Uri,
                                    eventResponse.Response.StatusCode));
                        }

                    }
                    catch (Exception ex)
                    {
                        eventResponse.Exception = ex;
                    }

                    eventResponse.Finish = DateTime.UtcNow;

                    responses.Add(eventResponse);
                }
            }

            return responses;
        }

        async Task<TEntity> GetByGlobalId<TEntity>(SketchDbContext db, Guid id)
            where TEntity : class, IEntityGlobal
        {
            var entity = await db.Set<TEntity>()
                    .Include("KeyValues")
                    .SingleOrDefaultAsync(c => c.GlobalId.ToLower() == id.ToString().ToLower());

            return entity;
        }

        async Task HandleContentResponse(WebHookResponse webHookResponse, Guid id)
        {
            using (var db = new SketchDbContext(_options))
            {
                var content = await GetByGlobalId<Content>(db, id);

                if (content != null)
                {
                    var keyValue = content.KeyValues
                        .SingleOrDefault(kv => kv.Key == webHookResponse.Key) ?? new ContentKeyValue();

                    keyValue.Key = webHookResponse.Key.ToLower();
                    keyValue.Value = webHookResponse.Value;

                    await db.SaveChangesAsync();
                }
            }
        }

        async Task HandlePlaylistResponse(WebHookResponse webHookResponse, Guid id)
        {
            using (var db = new SketchDbContext(_options))
            {
                var playlist = await GetByGlobalId<Playlist>(db, id);

                if (playlist != null)
                {
                    var keyValue = playlist.KeyValues
                        .SingleOrDefault(kv => kv.Key == webHookResponse.Key) ?? new PlaylistKeyValue();

                    keyValue.Key = webHookResponse.Key.ToLower();
                    keyValue.Value = webHookResponse.Value;

                    await db.SaveChangesAsync();
                }
            }
        }
    }
}
