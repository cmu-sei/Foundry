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
    public class ViewStatementReport : ReportModel<ViewStatement>, IReportModel
    {
        public const string AnalyticsUrl = "api/statements/viewed";

        public ViewStatementReport(SketchDbContext dbContext, AuthorizationOptions authorizationOptions, AnalyticsOptions analyticsOptions)
            : base(dbContext, authorizationOptions, analyticsOptions)
        {
            DefaultSort = "-top";
        }

        public override void Run(ReportDataFilter dataFilter)
        {
            DataSet = new DataSet();
            DataSet.AddColumn("Content", "name", dataFilter.GetIsSortedBy("name"), dataFilter.GetSortDirection("name"));
            DataSet.AddColumn("Count", "top", dataFilter.GetIsSortedBy("top"), dataFilter.GetSortDirection("top"));

            var result = GetAnalyticsAsync(AnalyticsUrl).Result.ReadAsStringAsync().Result;

            var viewStatements = JsonConvert.DeserializeObject<List<ViewStatement>>(result);

            var globalIds = viewStatements.Select(r => r.Description.ToLower());
            var contents = DbContext.Contents.Where(c => globalIds.Contains(c.GlobalId.ToLower())).ToList();

            foreach (var statement in viewStatements)
            {
                var content = contents.FirstOrDefault(c => c.GlobalId.ToLower() == statement.Description.ToLower());

                if (content == null)
                {
                    statement.ContentName = statement.Name;
                    statement.ContentId = 0;
                    statement.ContentSlug = "_";
                }
                else
                {
                    statement.ContentName = content.Name;
                    statement.ContentId = content.Id;
                    statement.ContentSlug = content.Slug;
                }
            }

            var filtered = ApplyDataFilter(dataFilter, viewStatements.AsQueryable());

            foreach (var launch in filtered)
            {
                var row = new DataRow();

                if (launch.ContentId > 0)
                    row.Values.Add(new DataValue(DataSet.Columns[0], row, launch.ContentName, new DataValueLink("content", launch.ContentId, launch.ContentSlug)));
                else
                    row.Values.Add(new DataValue(DataSet.Columns[0], row, launch.Name));

                row.Values.Add(new DataValue(DataSet.Columns[1], row, launch.Count));

                DataSet.Rows.Add(row);
            }
        }

        public override IQueryable<ViewStatement> Filter(ReportDataFilter dataFilter, IQueryable<ViewStatement> query)
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

        public override IOrderedQueryable<ViewStatement> Sort(ReportDataFilter dataFilter, IQueryable<ViewStatement> query)
        {
            var sort = (dataFilter.Sort ?? DefaultSort).ToLower().Trim().Replace("-", "");
            var desc = dataFilter.Sort.StartsWith("-") ? true : false;

            switch (sort)
            {
                case "name":
                    return desc
                        ? query.OrderByDescending(c => c.Name)
                        : query.OrderBy(c => c.Name);
                case "top":
                default:
                    return desc
                        ? query.OrderByDescending(c => c.Count)
                        : query.OrderBy(c => c.Count);
            }
        }

        public override string Name { get { return "Content View Report"; } }
        public override string Description { get { return "Generates Content View Report."; } }
    }
}

