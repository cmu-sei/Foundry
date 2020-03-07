/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Foundry.Orders.Data.Entities
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        
        public bool IsPrivate { get; set; }

        public string Requestor { get; set; }

        [ForeignKey("BranchId")]
        public Branch Branch { get; set; }

        public int? BranchId { get; set; }

        public string BranchOther { get; set; }

        [ForeignKey("RankId")]
        public Rank Rank { get; set; }

        public int? RankId { get; set; }

        public string Unit { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string NetworkDiagramUrl { get; set; }

        [ForeignKey("ProducerId")]
        public Producer Producer { get; set; }

        public int? ProducerId { get; set; }
        
        public string Description { get; set; }

        [ForeignKey("ContentTypeId")]
        public ContentType ContentType { get; set; }

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

        public DurationType DurationType { get; set; }
        
        public DateTime? Due { get; set; }

        [ForeignKey("AudienceId")]
        public Audience Audience { get; set; }

        public int? AudienceId { get; set; }

        public ICollection<OrderAudienceItem> OrderAudienceItems { get; set; } = new HashSet<OrderAudienceItem>();

        public string AudienceItemOther { get; set; }

        public string RoleCrewPosition { get; set; }

        public int AudienceNumber { get; set; }

        public ICollection<OrderAssessmentType> OrderAssessmentTypes { get; set; } = new HashSet<OrderAssessmentType>();

        public string AssessmentTypeOther { get; set; }

        public ICollection<OrderOperatingSystemType> OrderOperatingSystemTypes { get; set; } = new HashSet<OrderOperatingSystemType>();

        public string OperatingSystemOther { get; set; }

        public ICollection<OrderSecurityTool> OrderSecurityTools { get; set; } = new HashSet<OrderSecurityTool>();

        public string SecurityToolOther { get; set; }

        public ICollection<OrderService> OrderServices { get; set; } = new HashSet<OrderService>();

        public string ServiceOther { get; set; }

        public ICollection<OrderSimulator> OrderSimulators { get; set; } = new HashSet<OrderSimulator>();

        public string SimulatorOther { get; set; }

        public ICollection<OrderTerrain> OrderTerrains { get; set; } = new HashSet<OrderTerrain>();

        public string TerrainOther { get; set; }

        public ICollection<OrderThreat> OrderThreats { get; set; } = new HashSet<OrderThreat>();

        public string ThreatOther { get; set; }

        public ICollection<OrderSupport> OrderSupports { get; set; } = new HashSet<OrderSupport>();

        public string SupportOther { get; set; }

        public bool IsEmbeddedTeam { get; set; }


        public ICollection<OrderEmbeddedTeam> OrderEmbeddedTeams { get; set; } = new HashSet<OrderEmbeddedTeam>();

        public string EmbeddedTeamOther { get; set; }


        public string CyberThreats { get; set; }

        public string SuccessIndicators { get; set; }

        public ICollection<File> Files { get; set; } = new HashSet<File>();

        [ForeignKey("EventTypeId")]
        public EventType EventType { get; set; }

        public int? EventTypeId { get; set; }

        public string ContentDescription { get; set; }

        public string TrainingDescription { get; set; }

        public string EventParticipants { get; set; }

        public string Theater { get; set; }

        public string Storyline { get; set; }

        public string MissionProcedures { get; set; }

        [ForeignKey("ClassificationId")]
        public Classification Classification { get; set; }

        public int? ClassificationId { get; set; }

        public string ClassificationOther { get; set; }

        [ForeignKey("FacilityId")]
        public Facility Facility { get; set; }

        public int? FacilityId { get; set; }

        public DateTime? EventStart { get; set; }

        public DateTime? EventEnd { get; set; }

        public OrderStatus Status { get; set; }

        public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

        public DateTime Created { get; set; }

        public int CreatedById { get; set; }

        [ForeignKey("CreatedById")]
        public Profile CreatedBy { get; set; }
    }

    public enum DurationType
    {
        NotSet,
        Hours,
        Days,
        Months,
        Weeks
    }

    public enum OrderStatus
    {
        Draft,
        Submitted,
        InProgress,
        NeedsInformation,
        Complete,
        Closed
    }
}
