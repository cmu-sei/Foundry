/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Foundry.Portal.Events;
using Foundry.Portal.Services;
using Foundry.Portal.ViewModels;
using Stack.Data.Options;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Http.Models;
using Stack.Http.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Portal.Api.Controllers
{
    public class HomeController : ApiController
    {
        RabbitMQOptions RabbitMQOptions { get; }
        AnalyticsOptions AnalyticsOptions { get; }
        AuthorizationOptions AuthorizationOptions { get; }
        DatabaseOptions DatabaseOptions { get; }
        IStackIdentityResolver IdentityResolver { get; }
        ContentService ContentService { get; }
        IDomainEventDelegator DomainEventDelegator { get; }
        BrandingOptions BrandingOptions { get; }

        /// <summary>
        /// creates an instance of the home controller
        /// </summary>
        /// <param name="options"></param>
        /// <param name="brandingOptions"></param>
        /// <param name="databaseOptions"></param>
        /// <param name="authorizationOptions"></param>
        /// <param name="analyticsOptions"></param>
        /// <param name="rabbitMQOptions"></param>
        /// <param name="mill"></param>
        /// <param name="contentService"></param>
        /// <param name="identityResolver"></param>
        /// <param name="domainEventDelegator"></param>
        public HomeController(
            CoreOptions options,
            BrandingOptions brandingOptions,
            DatabaseOptions databaseOptions,
            AuthorizationOptions authorizationOptions,
            AnalyticsOptions analyticsOptions,
            RabbitMQOptions rabbitMQOptions,
            ILoggerFactory mill,
            ContentService contentService,
            IStackIdentityResolver identityResolver,
            IDomainEventDelegator domainEventDelegator)
            : base(options, mill)
        {
            BrandingOptions = brandingOptions ?? throw new ArgumentNullException(nameof(brandingOptions));
            RabbitMQOptions = rabbitMQOptions ?? throw new ArgumentNullException(nameof(rabbitMQOptions));
            AnalyticsOptions = analyticsOptions ?? throw new ArgumentNullException(nameof(analyticsOptions));
            AuthorizationOptions = authorizationOptions ?? throw new ArgumentNullException(nameof(authorizationOptions));
            DatabaseOptions = databaseOptions ?? throw new ArgumentNullException(nameof(databaseOptions));
            IdentityResolver = identityResolver ?? throw new ArgumentNullException(nameof(identityResolver));
            ContentService = contentService ?? throw new ArgumentNullException(nameof(contentService));
            DomainEventDelegator = domainEventDelegator ?? throw new ArgumentNullException(nameof(domainEventDelegator));
        }

        /// <summary>
        /// root
        /// </summary>
        /// <returns></returns>
        [HttpGet("/")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index()
        {
            var model = new HomeModel
            {
                ApplicationName = BrandingOptions.ApplicationName,
                ApiStatus = GetStatus(),
                Configuration = GetConfiguration()
            };

            return View(model);
        }

        /// <summary>
        /// gets the status and module information for the api
        /// </summary>
        /// <returns></returns>
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [Route("api/status")]
        [HttpGet]
        [ProducesResponseType(typeof(ApiStatus), 200)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Status()
        {
            return Ok(GetStatus());
        }

        /// <summary>
        /// get api configuration
        /// </summary>
        /// <returns></returns>
        [Route("api/configuration")]
        [HttpGet]
        [ProducesResponseType(typeof(ApiStatus), 200)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Configuration()
        {
            var identity = await IdentityResolver.GetIdentityAsync();
            if (identity.Permissions.Contains("administrator"))
            {
                return Ok(GetConfiguration());
            }

            throw new EntityPermissionException("Action requires elevated permissions.");
        }

        ApiStatus GetStatus()
        {
            return new ApiStatus("Foundry.Portal.Api");
        }

        List<ConfigurationItem> GetConfiguration()
        {
            var items = new List<ConfigurationItem>
                {
                    new ConfigurationItem("Database", new Dictionary<string, object> {
                    { "Provider", DatabaseOptions.Provider },
                    { "Auto Migrate", DatabaseOptions.AutoMigrate },
                    { "Dev Mode Recreate", DatabaseOptions.DevModeRecreate }
                }),

                    new ConfigurationItem("Authorization", new Dictionary<string, object> {
                    { "Authority", AuthorizationOptions.Authority },
                    { "Scope", AuthorizationOptions.AuthorizationScope },
                    { "Client Id", AuthorizationOptions.ClientId },
                    { "Client Name", AuthorizationOptions.ClientName }
                }),

                    new ConfigurationItem("Buckets", new Dictionary<string, object> {
                    { "Url", CoreOptions.Buckets.Url }
                }),

                    new ConfigurationItem("Communication", new Dictionary<string, object> {
                    { "Url", CoreOptions.Communication.Url },
                    { "Source Id", CoreOptions.Communication.SourceId },
                    { "Source Url", CoreOptions.Communication.SourceUrl },
                    { "Client Secret Set", string.IsNullOrWhiteSpace(CoreOptions.Communication.ClientSecret) ? "No" : "Yes" }
                }),

                    new ConfigurationItem("Domain Event Dispatcher", new Dictionary<string, object> {
                    { "Handler", CoreOptions.DomainEventDispatcher.Handler }
                }),

                    new ConfigurationItem("Analytics", new Dictionary<string, object> {
                    { "Url", AnalyticsOptions.Url },
                    { "Source Url", AnalyticsOptions.SourceUrl },
                    { "Source Id", AnalyticsOptions.SourceId },
                    { "Exercise Leaderboard Url", AnalyticsOptions.ExerciseLeaderboardUrl },
                    { "STEP Authority", AnalyticsOptions.STEPAuthority },
                    { "STEP Client Id", AnalyticsOptions.STEPClientId },
                    { "STEP Scope", AnalyticsOptions.STEPScope }
                }),

                    new ConfigurationItem("Rabbit MQ", new Dictionary<string, object> {
                    { "Max Attempts", RabbitMQOptions.MaxAttempts },
                    { "Delay", RabbitMQOptions.Delay },
                    { "HostName", RabbitMQOptions.HostName },
                    { "Queue Name", RabbitMQOptions.QueueName },
                    { "Queue Durable", RabbitMQOptions.QueueDurable },
                    { "Queue Exclusive", RabbitMQOptions.QueueExclusive },
                    { "Queue Auto Delete", RabbitMQOptions.QueueAutoDelete },
                    { "Publish Mandatory", RabbitMQOptions.PublishMandatory }
                })
                };

            return items.OrderBy(i => i.Name).ToList();
        }
    }
}

