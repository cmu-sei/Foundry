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
    public class PlaylistFollowerReport : ReportModel<Follower>, IReportModel
    {
        public override string Name { get { return "Playlist Follower Report"; } }

        public override string Description { get { return "Generate a report about the playlist followers."; } }

        public PlaylistFollowerReport(SketchDbContext dbContext, AuthorizationOptions authorizationOptions, AnalyticsOptions analyticsOptions)
            : base(dbContext, authorizationOptions, analyticsOptions)
        {
            DefaultSort = "playlist";
        }

        public override void Run(ReportDataFilter dataFilter)
        {
            DataSet = new DataSet();
            DataSet.AddColumn("Playlist", "playlist", dataFilter.GetIsSortedBy("playlist"), dataFilter.GetSortDirection("playlist"));
            DataSet.AddColumn("Follower", "follower", dataFilter.GetIsSortedBy("follower"), dataFilter.GetSortDirection("follower"));
            DataSet.AddColumn("Type");

            var query = DbContext.Playlists
                //.Include(s => s.GroupFollowers)
                //.Include("GroupFollowers.Group")
                .Include(s => s.ProfileFollowers)
                .Include("ProfileFollowers.Profile");

            var playlists = query.ToList();

            var followers = new List<Follower>();

            foreach (var playlist in playlists)
            {
                //foreach (var gf in playlist.GroupFollowers)
                //{
                //    followers.Add(new Follower
                //    {
                //        PlaylistId = playlist.Id,
                //        PlaylistName = playlist.Name,
                //        PlaylistSlug = playlist.Slug,
                //        FollowerName = gf.Group.Name,
                //        Type = "Group",
                //        FollowerId = gf.GroupId,
                //        FollowerType = "group",
                //        FollowerSlug = gf.Group.Slug
                //    });
                //}

                foreach (var pf in playlist.ProfileFollowers)
                {
                    followers.Add(new Follower
                    {
                        PlaylistId = playlist.Id,
                        PlaylistName = playlist.Name,
                        PlaylistSlug = playlist.Slug,
                        FollowerName = pf.Profile.Name,
                        Type = "User",
                        FollowerId = pf.Profile.Id,
                        FollowerType = "profile",
                        FollowerSlug = pf.Profile.Slug
                    });
                }
            }

            var filtered = ApplyDataFilter(dataFilter, followers.AsQueryable());

            foreach (var follower in filtered)
            {
                var row = new DataRow();
                row.Values.Add(new DataValue(DataSet.Columns[0], row, follower.PlaylistName,
                    new DataValueLink("playlist", follower.PlaylistId, follower.PlaylistSlug)));

                row.Values.Add(new DataValue(DataSet.Columns[1], row, follower.FollowerName,
                    new DataValueLink(follower.FollowerType, follower.FollowerId, follower.FollowerSlug)));

                row.Values.Add(new DataValue(DataSet.Columns[2], row, follower.Type));

                DataSet.Rows.Add(row);
            }
        }

        public override IQueryable<Follower> Filter(ReportDataFilter dataFilter, IQueryable<Follower> query)
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

        public override IOrderedQueryable<Follower> Sort(ReportDataFilter dataFilter, IQueryable<Follower> query)
        {
            var sort = (dataFilter.Sort ?? DefaultSort).ToLower().Trim().Replace("-", "");
            var desc = (dataFilter.Sort ?? "").StartsWith("-") ? true : false;

            switch (sort)
            {
                case "follower":
                    return desc
                        ? query.OrderByDescending(c => c.FollowerName)
                        : query.OrderBy(c => c.FollowerName);
                case "playlist":
                default:
                    return desc
                        ? query.OrderByDescending(c => c.PlaylistName)
                        : query.OrderBy(c => c.PlaylistName);
            }
        }
    }
}
