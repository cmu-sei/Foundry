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
using Foundry.Portal.ViewModels;

namespace Foundry.Portal.Mapping
{
    public class ProfileContentProfile : AutoMapper.Profile
    {
        public ProfileContentProfile()
        {
            CreateMap<ProfileContent, ProfileContentSummary>()
                .ForMember(dest => dest.ContentName, opt => opt.MapFrom(src => src.Content == null ? "" : src.Content.Name))
                .ForMember(dest => dest.ProfileName, opt => opt.MapFrom(src => src.Profile == null ? "" : src.Profile.Name))
                .AfterMap((src, dest, res) => {
                    var content = src.Content;
                    var profile = src.Profile;

                    if (content == null || profile == null)
                        return;

                    dest.LaunchUrl = string.Format("launch/{0}/{1}", content.GlobalId, profile.GlobalId);
                });
        }
    }
}
