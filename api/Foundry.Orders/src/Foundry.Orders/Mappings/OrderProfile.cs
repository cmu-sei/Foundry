/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Foundry.Orders.Data.Entities;
using Foundry.Orders.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Foundry.Orders.Mappings
{
    public class OrderProfile : AutoMapper.Profile
    {
        List<TJoinModel> MapCollection<TJoinModel, TJoinEntity>(ICollection<TJoinEntity> collection, Func<TJoinEntity, TJoinModel> create)
            where TJoinModel : class, IJoinModel, new()
        {
            var list = new List<TJoinModel>();

            if (collection != null && collection.Any())
            {
                list = collection.Select(x => create(x)).ToList();
            }

            return list;            
        }

        public OrderProfile()
        {
            CreateMap<Order, OrderDetail>()
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch == null ? "" : src.Branch.Name))
                .ForMember(dest => dest.RankName, opt => opt.MapFrom(src => src.Rank == null ? "" : src.Rank.Name))
                .ForMember(dest => dest.AudienceName, opt => opt.MapFrom(src => src.Audience == null ? "" : src.Audience.Name))
                .ForMember(dest => dest.ProducerName, opt => opt.MapFrom(src => src.Producer == null ? "" : src.Producer.Name))
                .ForMember(dest => dest.ContentTypeName, opt => opt.MapFrom(src => src.ContentType == null ? "" : src.ContentType.Name))
                .ForMember(dest => dest.EventTypeName, opt => opt.MapFrom(src => src.EventType == null ? "" : src.EventType.Name))
                .ForMember(dest => dest.ClassificationName, opt => opt.MapFrom(src => src.Classification == null ? "" : src.Classification.Name))
                .ForMember(dest => dest.FacilityName, opt => opt.MapFrom(src => src.Facility == null ? "" : src.Facility.Name))
                .AfterMap((src, dest, res) => {
                    dest.AudienceItems = MapCollection(src.OrderAudienceItems, x => new OrderDetailAudienceItem { Id = x.Id, Name = x.AudienceItem.Name });
                    dest.AssessmentTypes = MapCollection(src.OrderAssessmentTypes, x => new OrderDetailAssessmentType { Id = x.Id, Name = x.AssessmentType.Name });
                    dest.EmbeddedTeams = MapCollection(src.OrderEmbeddedTeams, x => new OrderDetailEmbeddedTeam { Id = x.Id, Name = x.EmbeddedTeam.Name });
                    dest.OperatingSystemTypes = MapCollection(src.OrderOperatingSystemTypes, x => new OrderDetailOperatingSystemType { Id = x.Id, Name = x.OperatingSystemType.Name });
                    dest.Threats = MapCollection(src.OrderThreats, x => new OrderDetailThreat { Id = x.Id, Name = x.Threat.Name });
                    dest.SecurityTools = MapCollection(src.OrderSecurityTools, x => new OrderDetailSecurityTool { Id = x.Id, Name = x.SecurityTool.Name });
                    dest.Services = MapCollection(src.OrderServices, x => new OrderDetailService { Id = x.Id, Name = x.Service.Name });
                    dest.Simulators = MapCollection(src.OrderSimulators, x => new OrderDetailSimulator { Id = x.Id, Name = x.Simulator.Name });
                    dest.Supports = MapCollection(src.OrderSupports, x => new OrderDetailSupport { Id = x.Id, Name = x.Support.Name });
                    dest.Terrains = MapCollection(src.OrderTerrains, x => new OrderDetailTerrain { Id = x.Id, Name = x.Terrain.Name });
                    dest.Files = MapCollection(src.Files, x => new OrderDetailFile { Id = x.Id, Name = x.Name, Url = x.Url });
                });

            CreateMap<Order, OrderEdit>()
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch == null ? "" : src.Branch.Name))
                .ForMember(dest => dest.RankName, opt => opt.MapFrom(src => src.Rank == null ? "" : src.Rank.Name))
                .ForMember(dest => dest.AudienceName, opt => opt.MapFrom(src => src.Audience == null ? "" : src.Audience.Name))
                .ForMember(dest => dest.ProducerName, opt => opt.MapFrom(src => src.Producer == null ? "" : src.Producer.Name))
                .ForMember(dest => dest.ContentTypeName, opt => opt.MapFrom(src => src.ContentType == null ? "" : src.ContentType.Name))
                .ForMember(dest => dest.EventTypeName, opt => opt.MapFrom(src => src.EventType == null ? "" : src.EventType.Name))
                .ForMember(dest => dest.ClassificationName, opt => opt.MapFrom(src => src.Classification == null ? "" : src.Classification.Name))
                .ForMember(dest => dest.FacilityName, opt => opt.MapFrom(src => src.Facility == null ? "" : src.Facility.Name))
                .AfterMap((src, dest, res) => {
                    dest.AudienceItems = src.OrderAudienceItems.Select(x => x.AudienceItemId).ToList();
                    dest.AssessmentTypes = src.OrderAssessmentTypes.Select(x => x.AssessmentTypeId).ToList();
                    dest.EmbeddedTeams = src.OrderEmbeddedTeams.Select(x => x.EmbeddedTeamId).ToList();
                    dest.OperatingSystemTypes = src.OrderOperatingSystemTypes.Select(x => x.OperatingSystemTypeId).ToList();
                    dest.Threats = src.OrderThreats.Select(x => x.ThreatId).ToList();
                    dest.SecurityTools = src.OrderSecurityTools.Select(x => x.SecurityToolId).ToList();
                    dest.Services = src.OrderServices.Select(x => x.ServiceId).ToList();
                    dest.Simulators = src.OrderSimulators.Select(x => x.SimulatorId).ToList();
                    dest.Supports = src.OrderSupports.Select(x => x.SupportId).ToList();
                    dest.Terrains = src.OrderTerrains.Select(x => x.TerrainId).ToList();
                });

            CreateMap<File, OrderDetailFile>()
                .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedBy == null ? "" : src.CreatedBy.Name));

            CreateMap<File, OrderEditFile>();

            CreateMap<Comment, OrderDetailComment>()
                .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedBy == null ? "" : src.CreatedBy.Name));

            CreateMap<Order, OrderSummary>()
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch == null ? "" : src.Branch.Name))
                .ForMember(dest => dest.RankName, opt => opt.MapFrom(src => src.Rank == null ? "" : src.Rank.Name))
                .ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => src.Comments == null ? 0 : src.Comments.Count()));
        }
    }
}
