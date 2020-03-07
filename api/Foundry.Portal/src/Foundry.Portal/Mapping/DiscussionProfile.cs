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
using System.Linq;

namespace Foundry.Portal.Mapping
{
    public class DiscussionProfile : AutoMapper.Profile
    {
        public DiscussionProfile()
        {
            CreateMap<Discussion, DiscussionDetail>();
            CreateMap<Comment, DiscussionDetailComment>()
                .ForMember(dest => dest.Votes, opt => opt.MapFrom(src => src.Votes.Select(v => v.Value).Sum()))
                .ForMember(dest => dest.Votes, opt => opt.MapFrom(src => src.Votes.Select(v => v.Value).Sum()))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Profile.Name))
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.ProfileId))
                .ForMember(dest => dest.AuthorGlobalId, opt => opt.MapFrom(src => src.Profile.GlobalId))
                .AfterMap((src, dest, res) => {
                    var identity = res.GetIdentity();
                    if (identity != null)
                    {
                        var id = identity.GetId();
                        dest.CanVote = !src.Votes.Where(o => o.ProfileId == id).Any();
                        dest.CanEdit = src.ProfileId == id || identity.Permissions.Contains(SystemPermissions.PowerUser);
                        dest.AuthorVote = src.Votes.Where(o => o.ProfileId == id).Select(v => v.Value).SingleOrDefault();
                    }
                });

            CreateMap<Comment, CommentCreate>();
            CreateMap<Comment, CommentUpdate>();
            CreateMap<CommentCreate, Comment>();
            CreateMap<CommentUpdate, Comment>();

            CreateMap<Content, DiscussionDetailContent>();
            CreateMap<Discussion, DiscussionSummary>();
        }
    }
}
