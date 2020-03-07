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
    public class GroupRequestProfile : Profile
    {
        /// <summary>
        /// create a new instance of group request profile
        /// </summary>
        public GroupRequestProfile()
        {
            CreateMap<GroupRequest, GroupRequestDetail>()
                .ForMember(dest => dest.ChildGroupName, opt => opt.MapFrom(src => src.ChildGroup.Name))
                .ForMember(dest => dest.ChildGroupSlug, opt => opt.MapFrom(src => src.ChildGroup.Slug))
                .ForMember(dest => dest.ParentGroupName, opt => opt.MapFrom(src => src.ParentGroup.Name))
                .ForMember(dest => dest.ParentGroupSlug, opt => opt.MapFrom(src => src.ParentGroup.Slug))
                .AfterMap((src, dest, res) => {
                    var identity = res.GetIdentity();

                    if (identity != null)
                    {
                        dest.Actions = identity.SetActions(src);
                    }
                });
        }
    }
}

