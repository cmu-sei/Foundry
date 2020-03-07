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
using Foundry.Portal.Extensions;
using Foundry.Portal.Reports;
using Foundry.Portal.Services;
using Foundry.Portal.ViewModels;
using Stack.Http.Attributes;
using Stack.Http.Identity.Attributes;
using System;

namespace Foundry.Portal.Api.Controllers
{
    /// <summary>
    /// report controller
    /// </summary>
    [SecurityHeaders]
    [StackAuthorize]
    public class ReportController : ApiController
    {
        ReportService _reportService;

        /// <summary>
        /// creates an instance of the report controller
        /// </summary>
        /// <param name="reportService"></param>
        /// <param name="options"></param>
        /// <param name="mill"></param>
        public ReportController(ReportService reportService, CoreOptions options, ILoggerFactory mill)
            : base(options, mill)
        {
            _reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
        }

        /// <summary>
        /// generate report
        /// </summary>
        /// <param name="name">name of the report</param>
        /// <param name="search">data filter</param>
        /// <returns></returns>
        [Route("api/report/{name}")]
        [HttpGet]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(IReportModel), 200)]
        public IActionResult Report([FromRoute] string name, [FromQuery]ReportDataFilter search = null)
        {
            var report = _reportService.Run(name, search);
            return Ok(report);
        }

        /// <summary>
        /// get all reports
        /// </summary>
        /// <returns></returns>
        [Route("api/reports")]
        [HttpGet]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(IReportModel[]), 200)]
        public IActionResult Reports()
        {
            var reports = _reportService.GetAll();
            return Ok(reports);
        }

        /// <summary>
        /// export report as the request type
        /// </summary>
        /// <param name="name">name of the report</param>
        /// <param name="type">export file type</param>
        /// <param name="search">data filter</param>
        /// <returns></returns>
        [Route("api/report/{name}/export/{type}")]
        [HttpPost]
        [JsonExceptionFilter]
        [ResponseCache(NoStore = true)]
        [ProducesResponseType(typeof(IReportModel), 200)]
        public IActionResult Export([FromRoute] string name, [FromRoute] string type, [FromBody]ReportDataFilter search = null)
        {
            return _reportService.Run(name, search).ToFileContentResult(type);
        }
    }
}

