/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Foundry.Portal.Data.Entities;
using Foundry.Portal.Extensions;
using Foundry.Portal.Services;
using Foundry.Portal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Foundry.Portal.Mapping
{
    public class PlaylistProfile : AutoMapper.Profile
    {
        public PlaylistProfile()
        {
            CreateMap<Playlist, PlaylistSummary>()
                .ForMember(dest => dest.SectionCount, opt => opt.MapFrom(src => src.Sections == null ? 0 : src.Sections.Count))
                .ForMember(dest => dest.ContentCount, opt => opt.MapFrom(src => src.Sections == null ? 0 : src.Sections.SelectMany(s => s.SectionContents).Count()))
                .ForMember(dest => dest.ProfileFollowerCount, opt => opt.MapFrom(src => src.ProfileFollowers == null ? 0 : src.ProfileFollowers.Count))
                .ForMember(dest => dest.GroupFollowerCount, opt => opt.MapFrom(src => src.PlaylistGroups == null ? 0 : src.PlaylistGroups.Count))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => PlaylistService.CalculateRatingMetric(src)))
                .ForMember(dest => dest.Tags, opt =>
                    opt.MapFrom(src => src.PlaylistTags.Any()
                        ? src.PlaylistTags.OrderBy(ct => ct.Tag.Name).Select(t => new PlaylistSummaryTag { Name = t.Tag.Name, Slug = t.Tag.Slug }).ToList()
                        : new List<PlaylistSummaryTag>()))
                .ForMember(dest => dest.KeyValues, opt =>
                    opt.MapFrom(src => src.KeyValues.Any()
                        ? src.KeyValues.OrderBy(kv => kv.Key).Select(kv => new PlaylistSummaryPlaylistKeyValue { Key = kv.Key, Value = kv.Value }).ToList()
                        : new List<PlaylistSummaryPlaylistKeyValue>()))
                .AfterMap((src, dest, res) => {
                    var profile = res.GetIdentity();
                    if (profile != null)
                    {
                        var id = profile.GetId();
                        dest.IsPublic = src.IsPublic;
                        dest.IsFollowing = src.IsPublic && src.ProfileId != id && src.ProfileFollowers.Any(f => f.ProfileId == id);
                        dest.CanFollow = src.IsPublic && src.ProfileId != id && !src.ProfileFollowers.Any(f => f.ProfileId == id);
                        dest.IsOwner = src.CanEdit(profile);
                        dest.CanEdit = src.CanEdit(profile);
                    }
                });

            CreateMap<Playlist, PlaylistDetail>()
                .ForMember(dest => dest.ProfileFollowerCount, opt => opt.MapFrom(src => src.ProfileFollowers == null ? 0 : src.ProfileFollowers.Count))
                .ForMember(dest => dest.GroupFollowerCount, opt => opt.MapFrom(src => src.PlaylistGroups == null ? 0 : src.PlaylistGroups.Count))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => PlaylistService.CalculateRatingMetric(src)))
                .ForMember(dest => dest.SectionCount, opt => opt.MapFrom(src => src.Sections == null ? 0 : src.Sections.Count))
                .ForMember(dest => dest.ContentCount, opt => opt.MapFrom(src => src.Sections == null ? 0 : src.Sections.SelectMany(s => s.SectionContents).Count()))
                .ForMember(dest => dest.ProfileName, opt => opt.MapFrom(src => src.Profile == null ? "" : src.Profile.Name))
                .ForMember(dest => dest.ProfileSlug, opt => opt.MapFrom(src => src.Profile == null ? "" : src.Profile.Slug))
                .ForMember(dest => dest.Tags, opt =>
                    opt.MapFrom(src => src.PlaylistTags.Any()
                        ? src.PlaylistTags.OrderBy(ct => ct.Tag.Name).Select(t => new PlaylistDetailTag { Name = t.Tag.Name, Slug = t.Tag.Slug }).ToList()
                        : new List<PlaylistDetailTag>()))
                .ForMember(dest => dest.KeyValues, opt =>
                    opt.MapFrom(src => src.KeyValues.Any()
                        ? src.KeyValues.OrderBy(kv => kv.Key).Select(kv => new PlaylistDetailPlaylistKeyValue { Key = kv.Key, Value = kv.Value }).ToList()
                        : new List<PlaylistDetailPlaylistKeyValue>()))
                .ForMember(dest => dest.ProfileFollowers, opt =>
                    opt.MapFrom(src => src.ProfileFollowers.Any()
                        ? src.ProfileFollowers.Select(t => new PlaylistDetailProfileFollower { ProfileId = t.ProfileId, ProfileName = (t.Profile == null ? string.Empty : t.Profile.Name) }).ToList()
                        : new List<PlaylistDetailProfileFollower>()))
                .ForMember(dest => dest.GroupFollowers, opt =>
                    opt.MapFrom(src => src.PlaylistGroups.Any()
                        ? src.PlaylistGroups.Select(t => new PlaylistDetailGroupFollower { GroupId = t.GroupId, GroupName = t.GroupName }).ToList()
                        : new List<PlaylistDetailGroupFollower>()))
                .AfterMap((src, dest, res) => {

                    var profile = res.GetIdentity();
                    if (profile != null)
                    {
                        var id = profile.GetId();
                        dest.IsPublic = src.IsPublic;
                        dest.IsFollowing = src.IsPublic && src.ProfileId != id && src.ProfileFollowers.Any(f => f.ProfileId == id);
                        dest.CanFollow = src.IsPublic && src.ProfileId != id && !src.ProfileFollowers.Any(f => f.ProfileId == id);
                        dest.IsOwner = src.CanEdit(profile);
                        dest.CanEdit = src.CanEdit(profile);

                        var profileFollower = src.ProfileFollowers.SingleOrDefault(i => i.ProfileId == id);

                        if (profileFollower != null)
                        {
                            dest.UserRating = profileFollower.Rating;
                        }
                    }
                });

            CreateMap<Section, PlaylistDetailSection>()
                .ForMember(dest => dest.Contents, opt =>
                    opt.MapFrom(src => src.SectionContents.Any()
                        ? src.SectionContents.Select(sc => new PlaylistDetailSectionContent
                        {
                            Id = sc.ContentId,
                            Name = sc.Content == null ? "" : sc.Content.Name,
                            Description = sc.Content == null ? "" : sc.Content.Description,
                            Url = sc.Content == null ? "" : sc.Content.Url,
                            Type = sc.Content == null ? Data.ContentType.NotSet : sc.Content.Type,
                            LogoUrl = sc.Content == null ? "" : sc.Content.LogoUrl,
                            Slug = sc.Content == null ? "" : sc.Content.Slug,
                            Order = sc.Order,
                            StartDate = sc.Content == null || !sc.Content.Start.HasValue ? "" : sc.Content.Start.AsDateString(),
                            StartTime = sc.Content == null || !sc.Content.Start.HasValue ? "" : sc.Content.Start.AsTimeString(),
                            EndDate = sc.Content == null || !sc.Content.End.HasValue ? "" : sc.Content.End.AsDateString(),
                            EndTime = sc.Content == null || !sc.Content.End.HasValue ? "" : sc.Content.End.AsTimeString(),
                            Created = sc.Content == null ? DateTime.UtcNow : sc.Content.Created,
                            CreatedBy = sc.Content == null ? "" : sc.Content.CreatedBy,
                            Tags = sc.Content.ContentTags == null
                                ? new List<PlaylistDetailSectionContentTag>()
                                : sc.Content.ContentTags.Select(t => new PlaylistDetailSectionContentTag
                                {
                                    Name = t.Tag.Name,
                                    Slug = t.Tag.Slug
                                }).ToList()
                        }).ToList()
                        : new List<PlaylistDetailSectionContent>()));
        }
    }
}
