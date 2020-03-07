/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Foundry.Orders.Data.Entities;
using System;
using System.Collections.Generic;

namespace Foundry.Orders.ViewModels
{
    public class OrderDetail
    {        
        public int Id { get; set; }

        public bool IsPrivate { get; set; }

        public string Requestor { get; set; }

        public string BranchName { get; set; }

        public int? BranchId { get; set; }

        public string BranchOther { get; set; }
                
        public string RankName { get; set; }

        public int? RankId { get; set; }

        public string Unit { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
                
        public string ProducerName { get; set; }

        public int? ProducerId { get; set; }

        public string Description { get; set; }
                
        public string ContentTypeName { get; set; }

        public int? ContentTypeId { get; set; }

        public string Objectives { get; set; }

        public bool IsConfigurationNeeded { get; set; }

        public string ToolPreparation { get; set; }

        public string EnvironmentDetails { get; set; }

        public string AdversaryDetails { get; set; }

        public string ScenarioCommunications { get; set; }

        public string ScenarioAudienceDetails { get; set; }

        public string ScenarioAudienceProcedures { get; set; }

        public string Prerequisites { get; set; }

        public string Onboarding { get; set; }

        public double Duration { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DurationType DurationType { get; set; }

        public DateTime? Due { get; set; }
                
        public string AudienceName { get; set; }

        public int? AudienceId { get; set; }

        public List<OrderDetailAudienceItem> AudienceItems { get; set; } = new List<OrderDetailAudienceItem>();

        public string AudienceItemOther { get; set; }

        public int AudienceNumber { get; set; }

        public List<OrderDetailAssessmentType> AssessmentTypes { get; set; } = new List<OrderDetailAssessmentType>();

        public string AssessmentTypeOther { get; set; }

        public List<OrderDetailOperatingSystemType> OperatingSystemTypes { get; set; } = new List<OrderDetailOperatingSystemType>();

        public string OperatingSystemOther { get; set; }

        public List<OrderDetailSecurityTool> SecurityTools { get; set; } = new List<OrderDetailSecurityTool>();

        public string SecurityToolOther { get; set; }

        public List<OrderDetailService> Services { get; set; } = new List<OrderDetailService>();

        public string ServiceOther { get; set; }

        public List<OrderDetailSimulator> Simulators { get; set; } = new List<OrderDetailSimulator>();

        public string SimulatorOther { get; set; }

        public List<OrderDetailTerrain> Terrains { get; set; } = new List<OrderDetailTerrain>();

        public string TerrainOther { get; set; }

        public List<OrderDetailThreat> Threats { get; set; } = new List<OrderDetailThreat>();

        public string ThreatOther { get; set; }

        public List<OrderDetailSupport> Supports { get; set; } = new List<OrderDetailSupport>();

        public string SupportOther { get; set; }

        public bool IsEmbeddedTeam { get; set; }

        public List<OrderDetailEmbeddedTeam> EmbeddedTeams { get; set; } = new List<OrderDetailEmbeddedTeam>();

        public string EmbeddedTeamOther { get; set; }

        public string NetworkDiagramUrl { get; set; }

        public string CyberThreats { get; set; }

        public string SuccessIndicators { get; set; }

        public List<OrderDetailFile> Files { get; set; } = new List<OrderDetailFile>();
                
        public string EventTypeName { get; set; }

        public int? EventTypeId { get; set; }

        public string ContentDescription { get; set; }

        public string TrainingDescription { get; set; }

        public string EventParticipants { get; set; }

        public string Theater { get; set; }

        public string Storyline { get; set; }

        public string MissionProcedures { get; set; }
                
        public string ClassificationName { get; set; }

        public int? ClassificationId { get; set; }

        public string ClassificationOther { get; set; }
                
        public string FacilityName { get; set; }

        public int? FacilityId { get; set; }

        public DateTime? EventStart { get; set; }

        public DateTime? EventEnd { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public OrderStatus Status { get; set; }

        public List<OrderDetailComment> Comments { get; set; } = new List<OrderDetailComment>();

        public DateTime Created { get; set; }

        public int CreatedById { get; set; }
                
        public string CreatedByName { get; set; }
    }
}
