/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

//using Microsoft.EntityFrameworkCore;
//using Foundry.Portal.Api;
//using Foundry.Portal.Data;
//using Foundry.Portal.Data.Entities;
//using Foundry.Portal.Reports;
//using Stack.Http.Options;
//using Stack.Patterns.Service;
//using System.Linq;

//namespace Foundry.Portal.ViewModels.Report
//{
//    public class GroupPlaylistReport : ReportModel<GroupFollower>, IReportModel
//    {
//        public override string Name { get { return "Group Playlist Report"; } }

//        public override string Description { get { return "Generate a report about the playlists that groups follow."; } }

//        public GroupPlaylistReport(SketchDbContext dbContext, AuthorizationOptions authorizationOptions, AnalyticsOptions analyticsOptions, LrsOptions lrsOptions)
//            : base(dbContext, authorizationOptions, analyticsOptions, lrsOptions)
//        {
//            DefaultSort = "playlist";
//        }

//        public override void Run(ReportDataFilter dataFilter)
//        {
//            DataSet = new DataSet();
//            DataSet.AddColumn("Group", "group", dataFilter.GetIsSortedBy("group"), dataFilter.GetSortDirection("group"));
//            DataSet.AddColumn("Playlist", "playlist", dataFilter.GetIsSortedBy("playlist"), dataFilter.GetSortDirection("playlist"));

//            var query = DbContext.GroupFollowers
//                .Include(gf => gf.Group)
//                .Include(gf => gf.Playlist);

//            var playlists = query.ToList();

//            var filtered = ApplyDataFilter(dataFilter, query);

//            foreach (var follower in filtered)
//            {
//                var row = new DataRow();
//                row.Values.Add(new DataValue(DataSet.Columns[0], row, follower.Group.Name, new DataValueLink("group", follower.Group.Id, follower.Group.Slug)));
//                row.Values.Add(new DataValue(DataSet.Columns[1], row, follower.Playlist.Name, new DataValueLink("playlist", follower.Playlist.Id, follower.Playlist.Slug)));
//                DataSet.Rows.Add(row);
//            }
//        }

//        public override IQueryable<GroupFollower> Filter(ReportDataFilter dataFilter, IQueryable<GroupFollower> query)
//        {
//            var keyValues = dataFilter.Filter.ToFilterKeyValues();

//            foreach (var filter in keyValues)
//            {
//                var intValues = filter.ToIntValues();
//                var key = filter.Key.Replace("!", "");
//                var not = filter.Key.StartsWith("!");
//            }

//            return query;
//        }

//        public override IOrderedQueryable<GroupFollower> Sort(ReportDataFilter dataFilter, IQueryable<GroupFollower> query)
//        {
//            var sort = (dataFilter.Sort ?? DefaultSort).ToLower().Trim().Replace("-", "");
//            var desc = (dataFilter.Sort ?? "").StartsWith("-") ? true : false;

//            switch (sort)
//            {
//                case "group":
//                    return desc
//                        ? query.OrderByDescending(c => c.Group.Name)
//                        : query.OrderBy(c => c.Group.Name);
//                case "playlist":
//                default:
//                    return desc
//                        ? query.OrderByDescending(c => c.Playlist.Name)
//                        : query.OrderBy(c => c.Playlist.Name);
//            }
//        }
//    }
//}

