/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Foundry.Orders.Options;
using Stack.Http.Options;
using System;

namespace Foundry.Orders.Controllers
{
    /// <summary>
    /// home
    /// </summary>
    [SecurityHeaders]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        ClientSettings _settings;

        /// <summary>
        /// create instance
        /// </summary>
        /// <param name="brandingOptions"></param>
        /// <param name="clientSettings"></param>
        /// <param name="fileOptions"></param>
        public HomeController(BrandingOptions brandingOptions, ClientSettings clientSettings, FileOptions fileOptions)
        {
            _settings = clientSettings ?? throw new ArgumentNullException("clientSettings");
            _settings.Branding = brandingOptions;
            _settings.FileStorage = fileOptions;
        }

        /// <summary>
        /// home page
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            ViewBag.ClientSettings = JsonConvert.SerializeObject(
                _settings,
                Formatting.None,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }
            );
            return View();
        }

        public IActionResult Error()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            return View("~/Views/Shared/Error.cshtml", feature?.Error);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.AppName = _settings.Branding.ApplicationName;
        }
    }
}

