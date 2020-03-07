/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Newtonsoft.Json;
using Foundry.Portal.Data;
using Foundry.Portal.Reports;
using Stack.Http.Options;
using Stack.Patterns.Service;
using System.Collections.Generic;
using System.Linq;

namespace Foundry.Portal.ViewModels
{
    public class LogInStatementReport : ReportModel<LogInStatement>, IReportModel
    {
        public const string AnalyticsUrl = "api/statements/logged-in";

        public LogInStatementReport(SketchDbContext dbContext, AuthorizationOptions authorizationOptions, AnalyticsOptions analyticsOptions)
            : base(dbContext, authorizationOptions, analyticsOptions)
        {
            DefaultSort = "-recent";
        }

        public override void Run(ReportDataFilter dataFilter)
        {
            DataSet = new DataSet();
            DataSet.AddColumn("User", "name", dataFilter.GetIsSortedBy("name"), dataFilter.GetSortDirection("name"));
            DataSet.AddColumn("Timestamp", "recent", dataFilter.GetIsSortedBy("recent"), dataFilter.GetSortDirection("recent"));

            var result = GetAnalyticsAsync(AnalyticsUrl).Result.ReadAsStringAsync().Result;
            var logInStatements = JsonConvert.DeserializeObject<List<LogInStatement>>(result);

            var globalIds = logInStatements.Select(r => r.Description.ToLower());
            var profiles = DbContext.Profiles.Where(c => globalIds.Contains(c.GlobalId.ToLower())).ToList();

            foreach (var statement in logInStatements)
            {
                var profile = profiles.FirstOrDefault(c => c.GlobalId.ToLower() == statement.Description.ToLower());

                if (profile == null)
                {
                    statement.ProfileName = statement.Name;
                    statement.ProfileId = 0;
                    statement.ProfileSlug = "_";
                }
                else
                {
                    statement.ProfileName = profile.Name;
                    statement.ProfileId = profile.Id;
                    statement.ProfileSlug = profile.Slug;
                }
            }

            var filtered = ApplyDataFilter(dataFilter, logInStatements.AsQueryable());

            foreach (var statement in filtered)
            {
                var row = new DataRow();

                if (statement.ProfileId > 0)
                    row.Values.Add(new DataValue(DataSet.Columns[0], row, statement.ProfileName, new DataValueLink("profile", statement.ProfileId, statement.ProfileSlug)));
                else
                    row.Values.Add(new DataValue(DataSet.Columns[0], row, statement.Name));

                row.Values.Add(new DataValue(DataSet.Columns[1], row, statement.Timestamp.ToString("M/d/yyyy h:mm tt")));

                DataSet.Rows.Add(row);
            }
        }

        public override IQueryable<LogInStatement> Filter(ReportDataFilter dataFilter, IQueryable<LogInStatement> query)
        {
            var keyValues = dataFilter.Filter.ToFilterKeyValues();

            foreach (var filter in keyValues)
            {
                var intValues = filter.ToIntValues();
                var key = filter.Key.Replace("!", "");
                var not = filter.Key.StartsWith("!");
            }

            return query;
        }

        public override IOrderedQueryable<LogInStatement> Sort(ReportDataFilter dataFilter, IQueryable<LogInStatement> query)
        {
            var sort = (dataFilter.Sort ?? DefaultSort).ToLower().Trim().Replace("-", "");
            var desc = dataFilter.Sort.StartsWith("-") ? true : false;

            switch (sort)
            {
                case "name":
                    return desc
                        ? query.OrderByDescending(c => c.Name)
                        : query.OrderBy(c => c.Name);
                case "recent":
                default:
                    return desc
                        ? query.OrderByDescending(c => c.Timestamp)
                        : query.OrderBy(c => c.Timestamp);
            }
        }

        public override string Name { get { return "User Login Report"; } }
        public override string Description { get { return "Generates User Login Report."; } }
    }
}

