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
using Foundry.Portal.ViewModels;
using Stack.Patterns.Service;
using System.Collections.Generic;
using System.Linq;

namespace Foundry.Portal.Mapping
{
    public class ProfileProfile : AutoMapper.Profile
    {
        public ProfileProfile()
        {
            CreateMap<Profile, ProfileDetail>()
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.ToRatingMetricDetail()))
                .AfterMap((src, dest, res) => {
                    dest.IsAdministrator = src.Permissions.HasFlag(SystemPermissions.Administrator);
                    dest.IsPowerUser = src.Permissions.HasFlag(SystemPermissions.PowerUser);
                    dest.AuthoredCount = src.Contents.Count();
                    dest.ContributionCount = src.ProfileContents.Count();

                    var identity = res.GetIdentity();
                    var id = res.GetId();

                    if (id == src.Id)
                    {
                        dest.KeyValues = src.KeyValues.Any()
                            ? src.KeyValues.OrderBy(kv => kv.Key).Select(kv => new ProfileDetailProfileKeyValue { Key = kv.Key, Value = kv.Value }).ToList()
                            : new List<ProfileDetailProfileKeyValue>();
                    }

                    if (identity != null && (id == src.Id || identity.Permissions.Contains(SystemPermissions.Administrator)))
                    {
                        dest.PlaylistCount = src.Playlists.Count(p => p.ProfileId == id);
                        dest.CanManage = true;
                    }
                    else
                    {
                        dest.PlaylistCount = src.Playlists.Count(p => p.IsPublic);
                        dest.CanManage = identity.Permissions.Contains(SystemPermissions.Administrator);
                    }
                });

            CreateMap<ProfileKeyValue, ProfileDetailProfileKeyValue>();

            CreateMap<Profile, ProfileSummary>()
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.ToRatingMetricDetail()))
                .AfterMap((src, dest, res) => {
                    dest.IsAdministrator = src.Permissions.HasFlag(SystemPermissions.Administrator);
                    dest.IsPowerUser = src.Permissions.HasFlag(SystemPermissions.PowerUser);
                    dest.AuthoredCount = src.Contents.Count();
                    dest.ContributionCount = src.ProfileContents.Count();

                    var identity = res.GetIdentity();
                    var id = res.GetId();

                    if (id == src.Id)
                    {
                        dest.KeyValues = src.KeyValues.Any()
                            ? src.KeyValues.OrderBy(kv => kv.Key).Select(kv => new ProfileSummaryProfileKeyValue { Key = kv.Key, Value = kv.Value }).ToList()
                            : new List<ProfileSummaryProfileKeyValue>();
                    }

                    if (identity != null && (id == src.Id || identity.Permissions.Contains(SystemPermissions.Administrator)))
                    {
                        dest.PlaylistCount = src.Playlists.Count(p => p.ProfileId == id);
                        dest.CanManage = true;
                    }
                    else
                    {
                        dest.PlaylistCount = src.Playlists.Count(p => p.IsPublic);
                        dest.CanManage = identity.Permissions.Contains(SystemPermissions.Administrator);
                    }
                });

            CreateMap<ProfileKeyValue, ProfileSummaryProfileKeyValue>();

            CreateMap<Profile, ProfileCreate>();
            CreateMap<Profile, ProfileUpdate>();
        }
    }
}
