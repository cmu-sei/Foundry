/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Foundry.Portal.Data;
using Stack.Http.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Foundry.Portal.Reports
{
    public class ReportFactory
    {
        SketchDbContext DbContext { get; }
        AuthorizationOptions AuthorizationOptions { get; }
        AnalyticsOptions AnalyticsOptions { get; }

        public ReportFactory(SketchDbContext dbContext, AuthorizationOptions authorizationOptions, AnalyticsOptions analyticsOptions)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            AuthorizationOptions = authorizationOptions ?? throw new ArgumentNullException(nameof(authorizationOptions));
            AnalyticsOptions = analyticsOptions ?? throw new ArgumentNullException(nameof(analyticsOptions));
        }

        public IReportModel Select(string slug)
        {
            var reports = GetAll();

            var report = reports.SingleOrDefault(r => r.Slug == slug.ToLower());

            if (report == null)
                throw new InvalidOperationException("Report '" + slug + "' was not found.");

            return (IReportModel)Activator.CreateInstance(report.GetType(), DbContext, AuthorizationOptions, AnalyticsOptions);
        }

        public IEnumerable<IReportModel> GetAll()
        {
            var reports = new List<IReportModel>();
            var type = typeof(IReportModel);

            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes());

            var reportModelTypes = types
                .Where(t => type.IsAssignableFrom(t) && !t.IsAbstract)
                .ToList();

            reportModelTypes.ForEach(t =>
                reports.Add((IReportModel)Activator.CreateInstance(t, DbContext, AuthorizationOptions, AnalyticsOptions)));

            return reports.OrderBy(r => r.Name);
        }
    }
}

