/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Foundry.Groups.Services;
using Foundry.Groups.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Stack.Communication.Notifications;
using Stack.Data.Options;
using Stack.DomainEvents;
using Stack.Http.Identity;
using Stack.Http.Models;
using Stack.Http.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Foundry.Groups.Controllers
{
    /// <summary>
    /// home view endpoints
    /// </summary>
    public class HomeController : Controller
    {
        BrandingOptions _brandingOptions;
        DatabaseOptions _databaseOptions;
        MigrationService _migrationService;
        IStackIdentityResolver _identityResolver;
        Stack.Http.Options.AuthorizationOptions _authorizationOptions;
        CommunicationOptions _communicationOptions;
        DomainEventDispatcherOptions _domainEventDispatcherOptions;

        /// <summary>
        /// create an instance of home controller
        /// </summary>
        /// <param name="brandingOptions"></param>
        /// <param name="databaseOptions"></param>
        /// <param name="authorizationOptions"></param>
        /// <param name="communicationOptions"></param>
        /// <param name="domainEventDispatcherOptions"></param>
        /// <param name="migrationService"></param>
        /// <param name="identityResolver"></param>
        public HomeController(
            BrandingOptions brandingOptions,
            DatabaseOptions databaseOptions,
            Stack.Http.Options.AuthorizationOptions authorizationOptions,
            CommunicationOptions communicationOptions,
            DomainEventDispatcherOptions domainEventDispatcherOptions,
            MigrationService migrationService,
            IStackIdentityResolver identityResolver)
        {
            _brandingOptions = brandingOptions ?? throw new ArgumentNullException(nameof(brandingOptions));
            _databaseOptions = databaseOptions ?? throw new ArgumentNullException(nameof(databaseOptions));
            _migrationService = migrationService ?? throw new ArgumentNullException(nameof(migrationService));
            _identityResolver = identityResolver ?? throw new ArgumentNullException(nameof(identityResolver));
            _authorizationOptions = authorizationOptions ?? throw new ArgumentNullException(nameof(authorizationOptions));
            _communicationOptions = communicationOptions;
            _domainEventDispatcherOptions = domainEventDispatcherOptions;
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
                ApplicationName = _brandingOptions.ApplicationName,
                ApiStatus = new ApiStatus("Foundry.Groups"),
                Configuration = GetConfiguration()
            };

            return View(model);
        }

        /// <summary>
        /// error page
        /// </summary>
        /// <returns></returns>
        [HttpGet("/error")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Error()
        {
            var feature = this.HttpContext.Features.Get<IExceptionHandlerFeature>();
            return View("~/Views/Shared/Error.cshtml", feature?.Error);
        }

        /// <summary>
        /// gets the status and module information for the api
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("api/status")]
        [ProducesResponseType(typeof(ApiStatus), 200)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Status()
        {
            var status = new ApiStatus("Foundry.Groups");
            return Ok(status);
        }

        /// <summary>
        /// get api configuration
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/configuration")]
        [ProducesResponseType(typeof(List<ConfigurationItem>), 200)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Configuration()
        {
            var items = GetConfiguration();
            return Ok(items);
        }

        List<ConfigurationItem> GetConfiguration()
        {
            var items = new List<ConfigurationItem>();

            items.Add(new ConfigurationItem("Database", new Dictionary<string, object> {
                    { "Provider", _databaseOptions.Provider },
                    { "Auto Migrate", _databaseOptions.AutoMigrate },
                    { "Dev Mode Recreate", _databaseOptions.DevModeRecreate }
                }));

            items.Add(new ConfigurationItem("Authorization", new Dictionary<string, object> {
                    { "Authority", _authorizationOptions.Authority },
                    { "Scope", _authorizationOptions.AuthorizationScope },
                    { "Client Id", _authorizationOptions.ClientId },
                    { "Client Name", _authorizationOptions.ClientName }
                }));

            items.Add(new ConfigurationItem("Communication", new Dictionary<string, object> {
                { "Communication Url", _communicationOptions.CommunicationUrl },
                { "Client Url", _communicationOptions.ClientUrl },
                { "Client Id", _communicationOptions.ClientId },
                { "Client Secret Set", string.IsNullOrWhiteSpace(_communicationOptions.ClientSecret) ? "No" : "Yes" }
            }));

            items.Add(new ConfigurationItem("Domain Event Dispatcher", new Dictionary<string, object> {
                { "Handler", _domainEventDispatcherOptions.Handler }
            }));

            return items.OrderBy(i => i.Name).ToList();
        }
    }
}
