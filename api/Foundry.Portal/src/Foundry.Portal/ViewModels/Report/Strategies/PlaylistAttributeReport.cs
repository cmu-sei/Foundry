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
using Foundry.Portal.Data.Entities;
using Foundry.Portal.Reports;
using Stack.Http.Options;
using Stack.Patterns.Service;
using System.Collections.Generic;
using System.Linq;

namespace Foundry.Portal.ViewModels
{
    public class PlaylistAttributeReport : ReportModel<Playlist>, IReportModel
    {
        public override string Name { get { return "Playlist Attribute Report"; } }

        public override string Description { get { return "Generate a report about playlist details."; } }

        public PlaylistAttributeReport(SketchDbContext dbContext, AuthorizationOptions authorizationOptions, AnalyticsOptions analyticsOptions)
            : base(dbContext, authorizationOptions, analyticsOptions)
        {
            DefaultSort = "playlist";
        }

        public override void Run(ReportDataFilter dataFilter)
        {
            DataSet = new DataSet();
            DataSet.AddColumn("Playlist", "playlist", dataFilter.GetIsSortedBy("playlist"), dataFilter.GetSortDirection("playlist"));
            DataSet.AddColumn("Sponsor", "group", dataFilter.GetIsSortedBy("group"), dataFilter.GetSortDirection("group"));
            DataSet.AddColumn("Sections", "sections", dataFilter.GetIsSortedBy("sections"), dataFilter.GetSortDirection("sections"));
            DataSet.AddColumn("Content", "content", dataFilter.GetIsSortedBy("content"), dataFilter.GetSortDirection("content"));
            DataSet.AddColumn("Group Followers", "groupfollowers", dataFilter.GetIsSortedBy("groupfollowers"), dataFilter.GetSortDirection("groupfollowers"));
            DataSet.AddColumn("Followers", "followers", dataFilter.GetIsSortedBy("followers"), dataFilter.GetSortDirection("followers"));
            DataSet.AddColumn("Creator", "profile", dataFilter.GetIsSortedBy("profile"), dataFilter.GetSortDirection("profile"));
            DataSet.AddColumn("Public");
            DataSet.AddColumn("Featured");
            DataSet.AddColumn("Recommended");

            var query = DbContext.Playlists
                .Include(s => s.Sections)
                .Include("Sections.SectionContents")
                .Include(s => s.ProfileFollowers)
                .Include(s => s.Profile);

            var playlists = ApplyDataFilter(dataFilter, query);

            foreach (var playlist in playlists)
            {
                var row = new DataRow();

                row.Values.Add(new DataValue(DataSet.Columns[0], row, playlist.Name, new DataValueLink("playlist", playlist.Id, playlist.Slug)));

                row.Values.Add(new DataValue(DataSet.Columns[2], row, playlist.Sections.Count()));
                row.Values.Add(new DataValue(DataSet.Columns[3], row, playlist.Sections.SelectMany(s => s.SectionContents).Count()));

                row.Values.Add(new DataValue(DataSet.Columns[5], row, playlist.ProfileFollowers.Count()));
                row.Values.Add(new DataValue(DataSet.Columns[6], row, playlist.Profile.Name, new DataValueLink("profile", playlist.Profile.Id, playlist.Profile.Slug)));

                row.Values.Add(new DataValue(DataSet.Columns[7], row, playlist.IsPublic ? "?" : ""));
                row.Values.Add(new DataValue(DataSet.Columns[8], row, playlist.IsFeatured ? "?" : ""));
                row.Values.Add(new DataValue(DataSet.Columns[9], row, playlist.IsRecommended ? "?" : ""));

                DataSet.Rows.Add(row);
            }
        }

        public override IQueryable<Playlist> Filter(ReportDataFilter dataFilter, IQueryable<Playlist> query)
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

        public override IOrderedQueryable<Playlist> Sort(ReportDataFilter dataFilter, IQueryable<Playlist> query)
        {
            var sort = (dataFilter.Sort ?? DefaultSort).ToLower().Trim().Replace("-", "");
            var desc = (dataFilter.Sort ?? "").StartsWith("-") ? true : false;

            switch (sort)
            {
                case "group":
                    return desc
                        ? query.OrderByDescending(p => p.PublisherName)
                        : query.OrderBy(p => p.PublisherName);
                case "sections":
                    return desc
                        ? query.OrderByDescending(p => p.Sections.Count())
                        : query.OrderBy(p => p.Sections.Count());
                case "content":
                    return desc
                        ? query.OrderByDescending(p => p.Sections.SelectMany(s => s.SectionContents).Count())
                        : query.OrderBy(p => p.Sections.SelectMany(s => s.SectionContents).Count());
                case "followers":
                    return desc
                        ? query.OrderByDescending(p => p.ProfileFollowers.Count())
                        : query.OrderBy(p => p.ProfileFollowers.Count());
                case "profile":
                    return desc
                        ? query.OrderByDescending(p => p.Profile.Name)
                        : query.OrderBy(p => p.Profile.Name);
                case "playlist":
                default:
                    return desc
                        ? query.OrderByDescending(p => p.Name)
                        : query.OrderBy(p => p.Name);
            }
        }
    }
}

