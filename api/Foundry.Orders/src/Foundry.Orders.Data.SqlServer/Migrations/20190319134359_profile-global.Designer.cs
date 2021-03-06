/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Foundry.Orders.Data;

namespace Foundry.Orders.Data.SqlServer.Migrations
{
    [DbContext(typeof(OrdersDbContext))]
    [Migration("20190319134359_profile-global")]
    partial class profileglobal
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Foundry.Orders.Data.Entities.AssessmentType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("AssessmentTypes");
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.Audience", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Audiences");
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.AudienceItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AudienceId");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("AudienceId");

                    b.ToTable("AudienceItems");
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.Branch", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Branches");
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.Classification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Classifications");
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.Comment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created");

                    b.Property<int>("CreatedById");

                    b.Property<string>("Message");

                    b.Property<int>("OrderId");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("OrderId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.ContentType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("ContentTypes");
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.EmbeddedTeam", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("EmbeddedTeams");
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.EventType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("HasDates");

                    b.Property<bool>("HasDuration");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("EventTypes");
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.Facility", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Facilities");
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.File", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created");

                    b.Property<int>("CreatedById");

                    b.Property<string>("Extension");

                    b.Property<string>("Name");

                    b.Property<int>("OrderId");

                    b.Property<string>("Type");

                    b.Property<string>("Url")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("OrderId");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.OperatingSystemType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("OperatingSystemTypes");
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AdversaryDetails");

                    b.Property<string>("AssessmentTypeOther");

                    b.Property<int?>("AudienceId");

                    b.Property<string>("AudienceItemOther");

                    b.Property<int>("AudienceNumber");

                    b.Property<int?>("BranchId");

                    b.Property<string>("BranchOther");

                    b.Property<int?>("ClassificationId");

                    b.Property<string>("ClassificationOther");

                    b.Property<string>("ContentDescription");

                    b.Property<int?>("ContentTypeId");

                    b.Property<DateTime>("Created");

                    b.Property<int>("CreatedById");

                    b.Property<string>("CyberThreats");

                    b.Property<string>("Description");

                    b.Property<DateTime?>("Due");

                    b.Property<double>("Duration");

                    b.Property<int>("DurationType");

                    b.Property<string>("Email");

                    b.Property<string>("EmbeddedTeamOther");

                    b.Property<string>("EnvironmentDetails");

                    b.Property<DateTime?>("EventEnd");

                    b.Property<string>("EventParticipants");

                    b.Property<DateTime?>("EventStart");

                    b.Property<int?>("EventTypeId");

                    b.Property<int?>("FacilityId");

                    b.Property<bool>("IsConfigurationNeeded");

                    b.Property<bool>("IsEmbeddedTeam");

                    b.Property<bool>("IsPrivate");

                    b.Property<string>("MissionProcedures");

                    b.Property<string>("NetworkDiagramUrl");

                    b.Property<string>("Objectives");

                    b.Property<string>("Onboarding");

                    b.Property<string>("OperatingSystemOther");

                    b.Property<string>("Phone");

                    b.Property<string>("Prerequisites");

                    b.Property<int?>("ProducerId");

                    b.Property<int?>("RankId");

                    b.Property<string>("Requestor");

                    b.Property<string>("RoleCrewPosition");

                    b.Property<string>("ScenarioAudienceDetails");

                    b.Property<string>("ScenarioAudienceProcedures");

                    b.Property<string>("ScenarioCommunications");

                    b.Property<string>("SecurityToolOther");

                    b.Property<string>("ServiceOther");

                    b.Property<string>("SimulatorOther");

                    b.Property<int>("Status");

                    b.Property<string>("Storyline");

                    b.Property<string>("SuccessIndicators");

                    b.Property<string>("SupportOther");

                    b.Property<string>("TerrainOther");

                    b.Property<string>("Theater");

                    b.Property<string>("ThreatOther");

                    b.Property<string>("ToolPreparation");

                    b.Property<string>("TrainingDescription");

                    b.Property<string>("Unit");

                    b.HasKey("Id");

                    b.HasIndex("AudienceId");

                    b.HasIndex("BranchId");

                    b.HasIndex("ClassificationId");

                    b.HasIndex("ContentTypeId");

                    b.HasIndex("CreatedById");

                    b.HasIndex("EventTypeId");

                    b.HasIndex("FacilityId");

                    b.HasIndex("ProducerId");

                    b.HasIndex("RankId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.OrderAssessmentType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AssessmentTypeId");

                    b.Property<int>("OrderId");

                    b.HasKey("Id");

                    b.HasIndex("AssessmentTypeId");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderAssessmentTypes");
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.OrderAudienceItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AudienceItemId");

                    b.Property<int>("OrderId");

                    b.HasKey("Id");

                    b.HasIndex("AudienceItemId");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderAudienceItems");
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.OrderEmbeddedTeam", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("EmbeddedTeamId");

                    b.Property<int>("OrderId");

                    b.HasKey("Id");

                    b.HasIndex("EmbeddedTeamId");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderEmbeddedTeams");
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.OrderOperatingSystemType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("OperatingSystemTypeId");

                    b.Property<int>("OrderId");

                    b.HasKey("Id");

                    b.HasIndex("OperatingSystemTypeId");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderOperatingSystemTypes");
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.OrderSecurityTool", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("OrderId");

                    b.Property<int>("SecurityToolId");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("SecurityToolId");

                    b.ToTable("OrderSecurityTools");
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.OrderService", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("OrderId");

                    b.Property<int>("ServiceId");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("ServiceId");

                    b.ToTable("OrderServices");
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.OrderSimulator", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("OrderId");

                    b.Property<int>("SimulatorId");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("SimulatorId");

                    b.ToTable("OrderSimulators");
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.OrderSupport", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("OrderId");

                    b.Property<int>("SupportId");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("SupportId");

                    b.ToTable("OrderSupports");
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.OrderTerrain", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("OrderId");

                    b.Property<int>("TerrainId");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("TerrainId");

                    b.ToTable("OrderTerrains");
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.OrderThreat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("OrderId");

                    b.Property<int>("ThreatId");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("ThreatId");

                    b.ToTable("OrderThreats");
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.Producer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Producers");
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.Profile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("GlobalId");

                    b.Property<bool>("IsAdministrator");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("GlobalId")
                        .IsUnique()
                        .HasFilter("[GlobalId] IS NOT NULL");

                    b.ToTable("Profiles");
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.Rank", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Abbreviation");

                    b.Property<int>("BranchId");

                    b.Property<string>("Grade");

                    b.Property<string>("InsigniaUrl");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("BranchId");

                    b.ToTable("Ranks");
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.SecurityTool", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("SecurityTools");
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.Service", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Services");
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.Simulator", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Simulators");
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.Support", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Supports");
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.Terrain", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Terrains");
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.Threat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Threats");
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.AudienceItem", b =>
                {
                    b.HasOne("Foundry.Orders.Data.Entities.Audience", "Audience")
                        .WithMany("AudienceItems")
                        .HasForeignKey("AudienceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.Comment", b =>
                {
                    b.HasOne("Foundry.Orders.Data.Entities.Profile", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Foundry.Orders.Data.Entities.Order", "Order")
                        .WithMany("Comments")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.File", b =>
                {
                    b.HasOne("Foundry.Orders.Data.Entities.Profile", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Foundry.Orders.Data.Entities.Order", "Order")
                        .WithMany("Files")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.Order", b =>
                {
                    b.HasOne("Foundry.Orders.Data.Entities.Audience", "Audience")
                        .WithMany("Orders")
                        .HasForeignKey("AudienceId");

                    b.HasOne("Foundry.Orders.Data.Entities.Branch", "Branch")
                        .WithMany("Orders")
                        .HasForeignKey("BranchId");

                    b.HasOne("Foundry.Orders.Data.Entities.Classification", "Classification")
                        .WithMany("Orders")
                        .HasForeignKey("ClassificationId");

                    b.HasOne("Foundry.Orders.Data.Entities.ContentType", "ContentType")
                        .WithMany("Orders")
                        .HasForeignKey("ContentTypeId");

                    b.HasOne("Foundry.Orders.Data.Entities.Profile", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Foundry.Orders.Data.Entities.EventType", "EventType")
                        .WithMany("Orders")
                        .HasForeignKey("EventTypeId");

                    b.HasOne("Foundry.Orders.Data.Entities.Facility", "Facility")
                        .WithMany("Orders")
                        .HasForeignKey("FacilityId");

                    b.HasOne("Foundry.Orders.Data.Entities.Producer", "Producer")
                        .WithMany("Orders")
                        .HasForeignKey("ProducerId");

                    b.HasOne("Foundry.Orders.Data.Entities.Rank", "Rank")
                        .WithMany("Orders")
                        .HasForeignKey("RankId");
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.OrderAssessmentType", b =>
                {
                    b.HasOne("Foundry.Orders.Data.Entities.AssessmentType", "AssessmentType")
                        .WithMany("OrderAssessmentTypes")
                        .HasForeignKey("AssessmentTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Foundry.Orders.Data.Entities.Order", "Order")
                        .WithMany("OrderAssessmentTypes")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.OrderAudienceItem", b =>
                {
                    b.HasOne("Foundry.Orders.Data.Entities.AudienceItem", "AudienceItem")
                        .WithMany("OrderAudienceItems")
                        .HasForeignKey("AudienceItemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Foundry.Orders.Data.Entities.Order", "Order")
                        .WithMany("OrderAudienceItems")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.OrderEmbeddedTeam", b =>
                {
                    b.HasOne("Foundry.Orders.Data.Entities.EmbeddedTeam", "EmbeddedTeam")
                        .WithMany("OrderEmbeddedTeams")
                        .HasForeignKey("EmbeddedTeamId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Foundry.Orders.Data.Entities.Order", "Order")
                        .WithMany("OrderEmbeddedTeams")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.OrderOperatingSystemType", b =>
                {
                    b.HasOne("Foundry.Orders.Data.Entities.OperatingSystemType", "OperatingSystemType")
                        .WithMany("OrderOperatingSystemTypes")
                        .HasForeignKey("OperatingSystemTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Foundry.Orders.Data.Entities.Order", "Order")
                        .WithMany("OrderOperatingSystemTypes")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.OrderSecurityTool", b =>
                {
                    b.HasOne("Foundry.Orders.Data.Entities.Order", "Order")
                        .WithMany("OrderSecurityTools")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Foundry.Orders.Data.Entities.SecurityTool", "SecurityTool")
                        .WithMany("OrderSecurityTools")
                        .HasForeignKey("SecurityToolId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.OrderService", b =>
                {
                    b.HasOne("Foundry.Orders.Data.Entities.Order", "Order")
                        .WithMany("OrderServices")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Foundry.Orders.Data.Entities.Service", "Service")
                        .WithMany("OrderServices")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.OrderSimulator", b =>
                {
                    b.HasOne("Foundry.Orders.Data.Entities.Order", "Order")
                        .WithMany("OrderSimulators")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Foundry.Orders.Data.Entities.Simulator", "Simulator")
                        .WithMany("OrderSimulators")
                        .HasForeignKey("SimulatorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.OrderSupport", b =>
                {
                    b.HasOne("Foundry.Orders.Data.Entities.Order", "Order")
                        .WithMany("OrderSupports")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Foundry.Orders.Data.Entities.Support", "Support")
                        .WithMany("OrderSupports")
                        .HasForeignKey("SupportId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.OrderTerrain", b =>
                {
                    b.HasOne("Foundry.Orders.Data.Entities.Order", "Order")
                        .WithMany("OrderTerrains")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Foundry.Orders.Data.Entities.Terrain", "Terrain")
                        .WithMany("OrderTerrains")
                        .HasForeignKey("TerrainId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.OrderThreat", b =>
                {
                    b.HasOne("Foundry.Orders.Data.Entities.Order", "Order")
                        .WithMany("OrderThreats")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Foundry.Orders.Data.Entities.Threat", "Threat")
                        .WithMany("OrderThreats")
                        .HasForeignKey("ThreatId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Orders.Data.Entities.Rank", b =>
                {
                    b.HasOne("Foundry.Orders.Data.Entities.Branch", "Branch")
                        .WithMany()
                        .HasForeignKey("BranchId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}

