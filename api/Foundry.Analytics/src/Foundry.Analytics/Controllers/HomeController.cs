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
using Foundry.Analytics.ViewModels;
using Foundry.Analytics.xApi;
using Stack.Http.Options;
using System;

namespace Foundry.Analytics.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        BrandingOptions _brandingOptions;
        LearningRecordStoreOptions _lrsOptions;

        public HomeController(BrandingOptions brandingOptions, LearningRecordStoreOptions lrsOptions)
        {
            _brandingOptions = brandingOptions ?? throw new ArgumentNullException(nameof(brandingOptions));
            _lrsOptions = lrsOptions ?? throw new ArgumentNullException(nameof(lrsOptions));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("/")]

        public IActionResult Index()
        {
            return View();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Error()
        {
            var feature = this.HttpContext.Features.Get<IExceptionHandlerFeature>();
            return View("~/Views/Shared/Error.cshtml", feature?.Error);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("api/options/{name}")]
        public IActionResult Options([FromRoute]string name)
        {
            if (name == "lrs")
                return Ok(_lrsOptions);

            if (name == "branding")
                return Ok(_brandingOptions);

            return NotFound(name + " not found");
        }


        /// <summary>
        /// gets the status and module information for the api
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("api/status")]
        [ProducesResponseType(typeof(Status), 200)]
        public IActionResult GetStatus()
        {
            var status = new Status("Foundry.Analytics");

            return Ok(status);
        }
    }
}
