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
using Stack.Http.Attributes;
using Stack.Http.Identity.Attributes;
using Stack.Http.Options;
using System;
using System.Threading.Tasks;

namespace Foundry.Portal.Api.Controllers
{
    /// <summary>
    /// dashboard api endpoints
    /// </summary>
    [SecurityHeaders]
    [StackAuthorize]
    public class DashboardController : ApiController
    {
        RabbitMQOptions _rabbitMQOptions;
        AnalyticsOptions _analyticsOptions;
        AuthorizationOptions _authorizationOptions;
        DatabaseOptions _databaseOptions;
        ContentService _contentService;
        readonly IDomainEventDelegator _domainEventDelegator;

        /// <summary>
        /// creates an instance of the DashboardController
        /// </summary>
        /// <param name="options"></param>
        /// <param name="databaseOptions"></param>
        /// <param name="authorizationOptions"></param>
        /// <param name="analyticsOptions"></param>
        /// <param name="rabbitMQOptions"></param>
        /// <param name="mill"></param>
        /// <param name="contentService"></param>
        /// <param name="domainEventDelegator"></param>
        public DashboardController(
            CoreOptions options,
            DatabaseOptions databaseOptions,
            AuthorizationOptions authorizationOptions,
            AnalyticsOptions analyticsOptions,
            RabbitMQOptions rabbitMQOptions,
            ILoggerFactory mill,
            ContentService contentService,
            IDomainEventDelegator domainEventDelegator)
            : base(options, mill)
        {
            _rabbitMQOptions = rabbitMQOptions ?? throw new ArgumentNullException(nameof(rabbitMQOptions));
            _analyticsOptions = analyticsOptions ?? throw new ArgumentNullException(nameof(analyticsOptions));
            _authorizationOptions = authorizationOptions ?? throw new ArgumentNullException(nameof(authorizationOptions));
            _databaseOptions = databaseOptions ?? throw new ArgumentNullException(nameof(databaseOptions));
            _contentService = contentService ?? throw new ArgumentNullException("contentService");
            _domainEventDelegator = domainEventDelegator;
        }

        /// <summary>
        /// get dashboard tokens
        /// </summary>
        /// <returns></returns>
        [Route("api/dashboard/tags")]
        [HttpGet]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(string[]), 200)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GetDashboardTags()
        {
            var model = await _contentService.GetDashboardTags();

            return Ok(model.ToArray());
        }

        /// <summary>
        /// get dashboard values
        /// </summary>
        /// <returns></returns>
        [Route("api/dashboard/values")]
        [HttpGet]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(DashboardValue[]), 200)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GetDashboardValues()
        {
            var model = await _contentService.GetDashboardValues();

            return Ok(model);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}

