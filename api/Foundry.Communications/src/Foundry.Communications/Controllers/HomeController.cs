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
using Foundry.Communications.ViewModels;
using Stack.Data.Options;
using Stack.Http.Exceptions;
using Stack.Http.Models;
using Stack.Http.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Foundry.Communications.Controllers
{
    /// <summary>
    /// home controller
    /// </summary>
    [AllowAnonymous]
    public class HomeController : Controller
    {
        BrandingOptions BrandingOptions { get; }
        DatabaseOptions DatabaseOptions { get; }
        Stack.Http.Options.AuthorizationOptions AuthorizationOptions { get; }

        /// <summary>
        /// create an instance of home controller
        /// </summary>
        /// <param name="brandingOptions"></param>
        /// <param name="databaseOptions"></param>
        /// <param name="authorizationOptions"></param>
        public HomeController(BrandingOptions brandingOptions, DatabaseOptions databaseOptions, Stack.Http.Options.AuthorizationOptions authorizationOptions)
        {
            BrandingOptions = brandingOptions ?? throw new ArgumentNullException(nameof(brandingOptions));
            DatabaseOptions = databaseOptions ?? throw new ArgumentNullException(nameof(databaseOptions));
            AuthorizationOptions = authorizationOptions ?? throw new ArgumentNullException(nameof(authorizationOptions));
        }

        /// <summary>
        /// home page
        /// </summary>
        /// <returns></returns>
        [HttpGet("/")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// error page
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Error()
        {
            var feature = this.HttpContext.Features.Get<IExceptionHandlerFeature>();
            return View("~/Views/Shared/Error.cshtml", feature?.Error);
        }

        /// <summary>
        /// get option by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("api/options/{name}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GetOptionByName([FromRoute]string name)
        {
            if (name == "branding")
                return Ok(BrandingOptions);

            return NotFound(name + " not found");
        }

        /// <summary>
        /// gets the status and module information for the api
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("api/status")]
        [ProducesResponseType(typeof(ApiStatus), 200)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GetStatus()
        {
            var status = new ApiStatus("Foundry.Communications");

            return Ok(status);
        }


        /// <summary>
        /// get api configuration
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/configuration")]
        [ProducesResponseType(typeof(List<ConfigurationItem>), 200)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GetConfiguration()
        {
            var items = new List<ConfigurationItem>();

            items.Add(new ConfigurationItem("Database", new Dictionary<string, object> {
                    { "Provider", DatabaseOptions.Provider },
                    { "Auto Migrate", DatabaseOptions.AutoMigrate },
                    { "Dev Mode Recreate", DatabaseOptions.DevModeRecreate }
                }));

            items.Add(new ConfigurationItem("Authorization", new Dictionary<string, object> {
                    { "Authority", AuthorizationOptions.Authority },
                    { "Scope", AuthorizationOptions.AuthorizationScope },
                    { "Client Id", AuthorizationOptions.ClientId },
                    { "Client Name", AuthorizationOptions.ClientName }
                }));

            return Ok(items.OrderBy(i => i.Name));

            throw new EntityPermissionException("Action requires elevated permissions.");
        }
    }
}
