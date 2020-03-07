/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using AutoMapper;
using Foundry.Groups.Data;
using Foundry.Groups.ViewModels;
using System.Linq;

namespace Foundry.Groups.Mapping
{
    public class GroupProfile : Profile
    {
        /// <summary>
        /// create a new instance of group profile
        /// </summary>
        public GroupProfile()
        {
            CreateMap<Group, GroupDetail>()
                .ForMember(dest => dest.ParentName, opt => opt.MapFrom(src => src.Parent.Name))
                .ForMember(dest => dest.ParentSlug, opt => opt.MapFrom(src => src.Parent.Slug))
                .AfterMap((src, dest, res) => {
                    var identity = res.GetIdentity();

                    if (identity != null)
                    {
                        dest.Actions = identity.SetActions(src);
                        dest.Roles = identity.SetRoles(src);
                    }

                    dest.Counts.Members = src.Members.Count();
                    dest.Counts.MemberRequests = src.MemberRequests.Count();
                    dest.Counts.Children = src.Children.Count();
                });

            CreateMap<Group, GroupSummary>()
                .ForMember(dest => dest.ParentName, opt => opt.MapFrom(src => src.Parent.Name))
                .ForMember(dest => dest.ParentSlug, opt => opt.MapFrom(src => src.Parent.Slug))
                .AfterMap((src, dest, res) => {
                    var identity = res.GetIdentity();

                    if (identity != null)
                    {
                        dest.Actions = identity.SetActions(src);
                        dest.Roles = identity.SetRoles(src);
                    }

                    dest.Counts.Members = src.Members.Count();
                    dest.Counts.MemberRequests = src.MemberRequests.Count();
                    dest.Counts.Children = src.Children.Count();
                });
        }
    }
}

