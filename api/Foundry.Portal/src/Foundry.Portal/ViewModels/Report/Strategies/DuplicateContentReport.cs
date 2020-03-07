/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.EntityFrameworkCore;
using Foundry.Portal.Data;
using Foundry.Portal.Reports;
using Stack.Http.Options;
using Stack.Patterns.Service;
using System.Collections.Generic;
using System.Linq;

namespace Foundry.Portal.ViewModels
{
    public class DuplicateContentReport : ReportModel<ContentDuplicate>, IReportModel
    {
        public override string Name { get { return "Duplicate Content Report"; } }

        public override string Description { get { return "Generate a report about duplicate content."; } }

        public DuplicateContentReport(SketchDbContext dbContext, AuthorizationOptions authorizationOptions, AnalyticsOptions analyticsOptions)
            : base(dbContext, authorizationOptions, analyticsOptions)
        {
            DefaultSort = "count";
        }

        public override void Run(ReportDataFilter dataFilter)
        {
            DataSet = new DataSet();
            DataSet.AddColumn("Url", "url", dataFilter.GetIsSortedBy("url"), dataFilter.GetSortDirection("url"));
            DataSet.AddColumn("Count", "count", dataFilter.GetIsSortedBy("count"), dataFilter.GetSortDirection("count"));
            DataSet.AddColumn("Content");

            var urls = DbContext.Contents
                .Where(c => !string.IsNullOrWhiteSpace(c.Url))
                .GroupBy(c => c.Url.ToLower().Trim())
                .Where(x => x.Count() > 1)
                .Select(x => x.Key.ToLower().Trim())
                .OrderBy(x => x).ToArray();

            var contents = DbContext.Contents
                .Where(c => urls.Contains(c.Url.ToLower().Trim()))
                .ToList();

            var list = new List<ContentDuplicate>();

            foreach (var url in urls)
            {
                var matches = contents.Where(c => c.Url.ToLower().Trim() == url);

                var duplicate = new ContentDuplicate()
                {
                    Url = url,
                    Count = matches.Count(),
                    ContentNames = string.Join(", ", matches.Select(c => string.Format("{0} [{1}]", c.Name, c.GlobalId)))
                };

                list.Add(duplicate);
            }

            var filtered = ApplyDataFilter(dataFilter, list.AsQueryable());

            foreach (var duplicate in filtered)
            {
                var row = new DataRow();
                row.Values.Add(new DataValue(DataSet.Columns[0], row, duplicate.Url));
                row.Values.Add(new DataValue(DataSet.Columns[1], row, duplicate.Count));
                row.Values.Add(new DataValue(DataSet.Columns[2], row, duplicate.ContentNames));

                DataSet.Rows.Add(row);
            }
        }

        public override IQueryable<ContentDuplicate> Filter(ReportDataFilter dataFilter, IQueryable<ContentDuplicate> query)
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

        public override IOrderedQueryable<ContentDuplicate> Sort(ReportDataFilter dataFilter, IQueryable<ContentDuplicate> query)
        {
            var sort = (dataFilter.Sort ?? DefaultSort).ToLower().Trim().Replace("-", "");
            var desc = (dataFilter.Sort ?? "").StartsWith("-") ? true : false;

            switch (sort)
            {
                case "url":
                    return desc
                        ? query.OrderByDescending(c => c.Url)
                        : query.OrderBy(c => c.Url);
                case "count":
                default:
                    return desc
                        ? query.OrderByDescending(c => c.Count)
                        : query.OrderBy(c => c.Count);
            }
        }
    }
}
