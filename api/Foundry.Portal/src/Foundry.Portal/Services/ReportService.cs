/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using AutoMapper;
using Microsoft.Extensions.Logging;
using Foundry.Portal.Data;
using System.Linq;
using Foundry.Portal.Reports;
using Foundry.Portal.ViewModels;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using System;
using System.Collections.Generic;

namespace Foundry.Portal.Services
{
    /// <summary>
    /// report service class
    /// </summary>
    public class ReportService : Service
    {
        SketchDbContext DbContext { get; }

        ReportFactory ReportFactory { get; }

        /// <summary>
        /// create an instance of report service
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="options"></param>
        /// <param name="userResolver"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="mapper"></param>
        /// <param name="reportFactory"></param>
        public ReportService(SketchDbContext dbContext, CoreOptions options, IStackIdentityResolver userResolver, ILoggerFactory loggerFactory, IMapper mapper, ReportFactory reportFactory)
            : base(options, userResolver, loggerFactory, mapper)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            ReportFactory = reportFactory ?? throw new ArgumentNullException(nameof(reportFactory));
        }

        /// <summary>
        /// select and run report
        /// </summary>
        /// <param name="slug"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public IReportModel Run(string slug, ReportDataFilter search)
        {
            if (!IsAdministrator)
                throw new EntityPermissionException("'" + slug + "' report requires elevated permissions.");

            var report = ReportFactory.Select(slug);

            report.Run(search);

            return report;
        }

        /// <summary>
        /// get all reports from report factory
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IReportModel> GetAll(bool? enabledOnly = true)
        {
            if (!IsAdministrator)
                throw new EntityPermissionException("Action requires elevated permissions.");

            if (enabledOnly.HasValue && enabledOnly.Value)
                return ReportFactory.GetAll().Where(r => r.IsEnabled);

            return ReportFactory.GetAll();
        }
    }
}
