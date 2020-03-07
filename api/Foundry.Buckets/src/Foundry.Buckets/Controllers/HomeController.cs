/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Foundry.Buckets.Attributes;
using Foundry.Buckets.Options;
using Foundry.Buckets.ViewModels;
using Stack.Data.Options;
using Stack.Http.Identity;
using Stack.Http.Models;
using Stack.Http.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Foundry.Buckets.Controllers
{
    /// <summary>
    /// home view endpoints
    /// </summary>
    [SecurityHeaders]
    public class HomeController : Controller
    {
        BrandingOptions _brandingOptions;
        DatabaseOptions _databaseOptions;
        UrlsOptions _urlsOptions;
        StorageOptions _storageOptions;
        StorageLocalOptions _storageLocalOptions;
        IStackIdentityResolver _identityResolver;
        Stack.Http.Options.AuthorizationOptions _authorizationOptions;

        /// <summary>
        /// create an instance of home controller
        /// </summary>
        /// <param name="brandingOptions"></param>
        /// <param name="databaseOptions"></param>
        /// <param name="urlsOptions"></param>
        /// <param name="storageOptions"></param>
        /// <param name="storageLocalOptions"></param>
        /// <param name="authorizationOptions"></param>
        /// <param name="identityResolver"></param>
        public HomeController(
            BrandingOptions brandingOptions,
            DatabaseOptions databaseOptions,
            UrlsOptions urlsOptions,
            StorageOptions storageOptions,
            StorageLocalOptions storageLocalOptions,
            Stack.Http.Options.AuthorizationOptions authorizationOptions,
            IStackIdentityResolver identityResolver)
        {
            _brandingOptions = brandingOptions ?? throw new ArgumentNullException(nameof(brandingOptions));
            _databaseOptions = databaseOptions ?? throw new ArgumentNullException(nameof(databaseOptions));
            _urlsOptions = urlsOptions ?? throw new ArgumentNullException(nameof(urlsOptions));
            _storageOptions = storageOptions ?? throw new ArgumentNullException(nameof(storageOptions));
            _storageLocalOptions = storageLocalOptions ?? throw new ArgumentNullException(nameof(storageLocalOptions));
            _identityResolver = identityResolver ?? throw new ArgumentNullException(nameof(identityResolver));
            _authorizationOptions = authorizationOptions ?? throw new ArgumentNullException(nameof(authorizationOptions));
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
                ApiStatus = new ApiStatus("Foundry.Buckets"),
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
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GetStatus()
        {
            var status = new ApiStatus("Foundry.Buckets", "Foundry.Buckets.Data");

            return Ok(status);
        }

        /// <summary>
        /// get api configuration
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/configuration")]
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

            items.Add(new ConfigurationItem("Urls", new Dictionary<string, object> {
                { "SketchMarket", _urlsOptions.SketchMarket }
            }));

            items.Add(new ConfigurationItem("Storage", new Dictionary<string, object> {
                { "MaxFileBytes", _storageOptions.MaxFileBytes },
                { "StorageType", _storageOptions.StorageType }
            }));

            items.Add(new ConfigurationItem("Storage Local", new Dictionary<string, object> {
                { "Path", _storageLocalOptions.Path }
            }));

            return items.OrderBy(i => i.Name).ToList();
        }
    }
}

