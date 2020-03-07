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
using Foundry.Portal.Data.Entities;
using Foundry.Portal.Extensions;
using Foundry.Portal.Security;
using Foundry.Portal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Foundry.Portal.Mapping
{
    public class ContentProfile : AutoMapper.Profile
    {
        public ContentProfile()
        {
            CreateMap<Content, ContentSummary>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author == null ? "" : src.Author.Name))
                .ForMember(dest => dest.AuthorSlug, opt => opt.MapFrom(src => src.Author == null ? "" : src.Author.Slug))
                .ForMember(dest => dest.Difficulty, opt => opt.MapFrom(src => src.ToDifficultyMetricDetail()))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.ToRatingMetricDetail()))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.Start.AsDateString()))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.Start.AsTimeString()))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.End.AsDateString()))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.End.AsTimeString()))
                .ForMember(dest => dest.IsFlagged, opt => opt.MapFrom(src => src.FlagCount > 0))
                .ForMember(dest => dest.Tags, opt =>
                    opt.MapFrom(src => src.ContentTags.Any()
                        ? src.ContentTags.OrderBy(ct => ct.Tag.Name).Select(t => new ContentSummaryTag { Name = t.Tag.Name, Slug = t.Tag.Slug }).ToList()
                        : new List<ContentSummaryTag>()))
                .ForMember(dest => dest.KeyValues, opt =>
                    opt.MapFrom(src => src.KeyValues.Any()
                        ? src.KeyValues.OrderBy(kv => kv.Key).Select(kv => new ContentSummaryContentKeyValue { Key = kv.Key, Value = kv.Value }).ToList()
                        : new List<ContentSummaryContentKeyValue>()))
                .AfterMap((src, dest, res) => {
                    var identity = res.GetIdentity();
                    if (identity != null)
                    {
                        var profileContent = src.ProfileContents.SingleOrDefault(i => i.ProfileId == identity.GetId());

                        if (profileContent != null)
                        {
                            dest.IsFlagged = profileContent.Flagged.HasValue && profileContent.FlagStatus == FlagStatusType.Pending;
                            dest.IsBookmarked = profileContent.Bookmarked.HasValue;
                        }

                        dest.CanAccess = true;
                        dest.CanEdit = ContentPermissions.CanEdit(identity, src);

                        if (dest.CanEdit) dest.Access.Add("edit");
                    }

                    dest.LaunchUrl = string.Format("launch/{0}/{1}", src.GlobalId, res.GetGlobalId());

                    if (src.Created.AddDays(14) > DateTime.UtcNow) dest.Labels.Add("new");
                    if (dest.Rating.Average > 3 && dest.Rating.Total > 3) dest.Labels.Add("popular");
                    if (dest.IsRecommended) dest.Labels.Add("recommended");
                    if (dest.IsFlagged) dest.Labels.Add("flagged");
                    if (dest.IsBookmarked) dest.Labels.Add("bookmarked");
                });

            CreateMap<Content, ContentDetail>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author == null ? "" : src.Author.Name))
                .ForMember(dest => dest.AuthorSlug, opt => opt.MapFrom(src => src.Author == null ? "" : src.Author.Slug))
                .ForMember(dest => dest.Difficulty, opt => opt.MapFrom(src => src.ToDifficultyMetricDetail()))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.ToRatingMetricDetail()))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.Start.AsDateString()))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.Start.AsTimeString()))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.End.AsDateString()))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.End.AsTimeString()))
                .ForMember(dest => dest.IsFlagged, opt => opt.MapFrom(src => src.FlagCount > 0))
                .ForMember(dest => dest.Tags, opt =>
                    opt.MapFrom(src => src.ContentTags.Any()
                        ? src.ContentTags.OrderBy(ct => ct.Tag.Name).Select(t => new ContentDetailTag { Name = t.Tag.Name, Slug = t.Tag.Slug, TagType = t.Tag.TagType }).ToList()
                        : new List<ContentDetailTag>()))
                .ForMember(dest => dest.KeyValues, opt =>
                    opt.MapFrom(src => src.KeyValues.Any()
                        ? src.KeyValues.OrderBy(kv => kv.Key).Select(kv => new ContentDetailContentKeyValue { Key = kv.Key, Value = kv.Value }).ToList()
                        : new List<ContentDetailContentKeyValue>()))
                .AfterMap((src, dest, res) => {

                    dest.Flags = src.ProfileContents.Where(pc => pc.Flagged.HasValue).Select((pc) =>
                    {
                        return new ContentDetailFlag
                        {
                            ProfileId = pc.ProfileId,
                            Comment = pc.FlaggedComment,
                            FlagStatus = pc.FlagStatus,
                            ProfileName = pc.Profile != null ? pc.Profile.Name : ""
                        };

                    }).ToList();

                    if (src.SectionContents.Any())
                    {
                        dest.Playlists = src.SectionContents.Select(sc =>
                        {
                            var playlist = sc.Section.Playlist;
                            return new ContentDetailPlaylist
                            {
                                Id = playlist.Id,
                                Slug = playlist.Slug,
                                Name = playlist.Name
                            };
                        }).OrderBy(p => p.Name).ToList();
                    }

                    var profile = res.GetIdentity();
                    if (profile != null)
                    {
                        var profileContent = src.ProfileContents.SingleOrDefault(i => i.ProfileId == profile.GetId());

                        if (profileContent != null)
                        {
                            dest.IsFlagged = profileContent.Flagged.HasValue && profileContent.FlagStatus == FlagStatusType.Pending;
                            dest.IsBookmarked = profileContent.Bookmarked.HasValue;
                            dest.UserDifficulty = profileContent.Difficulty;
                            dest.UserRating = profileContent.Rating;
                        }

                        dest.CanAccess = true;
                        dest.CanEdit = ContentPermissions.CanEdit(profile, src);

                        if (dest.CanEdit) dest.Access.Add("edit");
                    }

                    dest.LaunchUrl = string.Format("launch/{0}/{1}", src.GlobalId, res.GetGlobalId());

                    if (src.Created.AddDays(14) > DateTime.UtcNow) dest.Labels.Add("new");
                    if (dest.Rating.Average > 3 && dest.Rating.Total > 3) dest.Labels.Add("popular");
                    if (dest.IsRecommended) dest.Labels.Add("recommended");
                    if (dest.IsFlagged) dest.Labels.Add("flagged");
                    if (dest.IsBookmarked) dest.Labels.Add("bookmarked");
                });

            CreateMap<ProfileContent, ContentDetailProfileContent>();
            CreateMap<Discussion, ContentDetailDiscussion>();

            CreateMap<Content, ContentCreate>();
            CreateMap<ContentCreate, Content>();

            CreateMap<Content, ContentUpdate>();
            CreateMap<ContentUpdate, Content>();
            CreateMap<ContentDetail, ContentUpdate>();

            CreateMap<Content, ContentExport>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => string.Join(",", src.ContentTags.Select(t => t.Tag.Name))));
        }
    }
}

