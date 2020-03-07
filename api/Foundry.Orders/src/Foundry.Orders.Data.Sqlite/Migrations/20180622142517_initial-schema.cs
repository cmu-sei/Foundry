/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Foundry.Orders.Data.Sqlite.Migrations
{
    public partial class initialschema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssessmentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Audiences",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audiences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Classifications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmbeddedTeams",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmbeddedTeams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HasDates = table.Column<bool>(nullable: false),
                    HasDuration = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Facilities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facilities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OperatingSystemTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperatingSystemTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Producers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GlobalId = table.Column<string>(nullable: true),
                    IsAdministrator = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SecurityTools",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityTools", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Simulators",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Simulators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Supports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Terrains",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Terrains", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Threats",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Threats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AudienceItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AudienceId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AudienceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AudienceItems_Audiences_AudienceId",
                        column: x => x.AudienceId,
                        principalTable: "Audiences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ranks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Abbreviation = table.Column<string>(nullable: true),
                    BranchId = table.Column<int>(nullable: false),
                    Grade = table.Column<string>(nullable: true),
                    InsigniaUrl = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ranks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ranks_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AdversaryDetails = table.Column<string>(nullable: true),
                    AssessmentTypeOther = table.Column<string>(nullable: true),
                    AudienceId = table.Column<int>(nullable: true),
                    AudienceItemOther = table.Column<string>(nullable: true),
                    AudienceNumber = table.Column<int>(nullable: false),
                    BranchId = table.Column<int>(nullable: true),
                    BranchOther = table.Column<string>(nullable: true),
                    ClassificationId = table.Column<int>(nullable: true),
                    ClassificationOther = table.Column<string>(nullable: true),
                    ContentDescription = table.Column<string>(nullable: true),
                    ContentTypeId = table.Column<int>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    CyberThreats = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Due = table.Column<DateTime>(nullable: true),
                    Duration = table.Column<double>(nullable: false),
                    DurationType = table.Column<int>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    EmbeddedTeamOther = table.Column<string>(nullable: true),
                    EnvironmentDetails = table.Column<string>(nullable: true),
                    EventEnd = table.Column<DateTime>(nullable: true),
                    EventParticipants = table.Column<string>(nullable: true),
                    EventStart = table.Column<DateTime>(nullable: true),
                    EventTypeId = table.Column<int>(nullable: true),
                    FacilityId = table.Column<int>(nullable: true),
                    IsConfigurationNeeded = table.Column<bool>(nullable: false),
                    IsEmbeddedTeam = table.Column<bool>(nullable: false),
                    IsPrivate = table.Column<bool>(nullable: false),
                    MissionProcedures = table.Column<string>(nullable: true),
                    NetworkDiagramUrl = table.Column<string>(nullable: true),
                    Objectives = table.Column<string>(nullable: true),
                    Onboarding = table.Column<string>(nullable: true),
                    OperatingSystemOther = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Prerequisites = table.Column<string>(nullable: true),
                    ProducerId = table.Column<int>(nullable: true),
                    RankId = table.Column<int>(nullable: true),
                    Requestor = table.Column<string>(nullable: true),
                    RoleCrewPosition = table.Column<string>(nullable: true),
                    ScenarioAudienceDetails = table.Column<string>(nullable: true),
                    ScenarioAudienceProcedures = table.Column<string>(nullable: true),
                    ScenarioCommunications = table.Column<string>(nullable: true),
                    SecurityToolOther = table.Column<string>(nullable: true),
                    ServiceOther = table.Column<string>(nullable: true),
                    SimulatorOther = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Storyline = table.Column<string>(nullable: true),
                    SuccessIndicators = table.Column<string>(nullable: true),
                    SupportOther = table.Column<string>(nullable: true),
                    TerrainOther = table.Column<string>(nullable: true),
                    Theater = table.Column<string>(nullable: true),
                    ThreatOther = table.Column<string>(nullable: true),
                    ToolPreparation = table.Column<string>(nullable: true),
                    TrainingDescription = table.Column<string>(nullable: true),
                    Unit = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Audiences_AudienceId",
                        column: x => x.AudienceId,
                        principalTable: "Audiences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Classifications_ClassificationId",
                        column: x => x.ClassificationId,
                        principalTable: "Classifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_ContentTypes_ContentTypeId",
                        column: x => x.ContentTypeId,
                        principalTable: "ContentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Profiles_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_EventTypes_EventTypeId",
                        column: x => x.EventTypeId,
                        principalTable: "EventTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Producers_ProducerId",
                        column: x => x.ProducerId,
                        principalTable: "Producers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Ranks_RankId",
                        column: x => x.RankId,
                        principalTable: "Ranks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    OrderId = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Profiles_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    Extension = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OrderId = table.Column<int>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Files_Profiles_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Files_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderAssessmentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AssessmentTypeId = table.Column<int>(nullable: false),
                    OrderId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderAssessmentTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderAssessmentTypes_AssessmentTypes_AssessmentTypeId",
                        column: x => x.AssessmentTypeId,
                        principalTable: "AssessmentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderAssessmentTypes_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderAudienceItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AudienceItemId = table.Column<int>(nullable: false),
                    OrderId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderAudienceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderAudienceItems_AudienceItems_AudienceItemId",
                        column: x => x.AudienceItemId,
                        principalTable: "AudienceItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderAudienceItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderEmbeddedTeams",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmbeddedTeamId = table.Column<int>(nullable: false),
                    OrderId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderEmbeddedTeams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderEmbeddedTeams_EmbeddedTeams_EmbeddedTeamId",
                        column: x => x.EmbeddedTeamId,
                        principalTable: "EmbeddedTeams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderEmbeddedTeams_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderOperatingSystemTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OperatingSystemTypeId = table.Column<int>(nullable: false),
                    OrderId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderOperatingSystemTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderOperatingSystemTypes_OperatingSystemTypes_OperatingSystemTypeId",
                        column: x => x.OperatingSystemTypeId,
                        principalTable: "OperatingSystemTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderOperatingSystemTypes_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderSecurityTools",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(nullable: false),
                    SecurityToolId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderSecurityTools", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderSecurityTools_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderSecurityTools_SecurityTools_SecurityToolId",
                        column: x => x.SecurityToolId,
                        principalTable: "SecurityTools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderServices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(nullable: false),
                    ServiceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderServices_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderServices_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderSimulators",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(nullable: false),
                    SimulatorId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderSimulators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderSimulators_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderSimulators_Simulators_SimulatorId",
                        column: x => x.SimulatorId,
                        principalTable: "Simulators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderSupports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(nullable: false),
                    SupportId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderSupports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderSupports_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderSupports_Supports_SupportId",
                        column: x => x.SupportId,
                        principalTable: "Supports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderTerrains",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(nullable: false),
                    TerrainId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderTerrains", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderTerrains_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderTerrains_Terrains_TerrainId",
                        column: x => x.TerrainId,
                        principalTable: "Terrains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderThreats",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(nullable: false),
                    ThreatId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderThreats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderThreats_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderThreats_Threats_ThreatId",
                        column: x => x.ThreatId,
                        principalTable: "Threats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AudienceItems_AudienceId",
                table: "AudienceItems",
                column: "AudienceId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CreatedById",
                table: "Comments",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_OrderId",
                table: "Comments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_CreatedById",
                table: "Files",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Files_OrderId",
                table: "Files",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAssessmentTypes_AssessmentTypeId",
                table: "OrderAssessmentTypes",
                column: "AssessmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAssessmentTypes_OrderId",
                table: "OrderAssessmentTypes",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAudienceItems_AudienceItemId",
                table: "OrderAudienceItems",
                column: "AudienceItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAudienceItems_OrderId",
                table: "OrderAudienceItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderEmbeddedTeams_EmbeddedTeamId",
                table: "OrderEmbeddedTeams",
                column: "EmbeddedTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderEmbeddedTeams_OrderId",
                table: "OrderEmbeddedTeams",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOperatingSystemTypes_OperatingSystemTypeId",
                table: "OrderOperatingSystemTypes",
                column: "OperatingSystemTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderOperatingSystemTypes_OrderId",
                table: "OrderOperatingSystemTypes",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_AudienceId",
                table: "Orders",
                column: "AudienceId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_BranchId",
                table: "Orders",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ClassificationId",
                table: "Orders",
                column: "ClassificationId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ContentTypeId",
                table: "Orders",
                column: "ContentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CreatedById",
                table: "Orders",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_EventTypeId",
                table: "Orders",
                column: "EventTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_FacilityId",
                table: "Orders",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ProducerId",
                table: "Orders",
                column: "ProducerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_RankId",
                table: "Orders",
                column: "RankId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderSecurityTools_OrderId",
                table: "OrderSecurityTools",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderSecurityTools_SecurityToolId",
                table: "OrderSecurityTools",
                column: "SecurityToolId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderServices_OrderId",
                table: "OrderServices",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderServices_ServiceId",
                table: "OrderServices",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderSimulators_OrderId",
                table: "OrderSimulators",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderSimulators_SimulatorId",
                table: "OrderSimulators",
                column: "SimulatorId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderSupports_OrderId",
                table: "OrderSupports",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderSupports_SupportId",
                table: "OrderSupports",
                column: "SupportId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTerrains_OrderId",
                table: "OrderTerrains",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTerrains_TerrainId",
                table: "OrderTerrains",
                column: "TerrainId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderThreats_OrderId",
                table: "OrderThreats",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderThreats_ThreatId",
                table: "OrderThreats",
                column: "ThreatId");

            migrationBuilder.CreateIndex(
                name: "IX_Ranks_BranchId",
                table: "Ranks",
                column: "BranchId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "OrderAssessmentTypes");

            migrationBuilder.DropTable(
                name: "OrderAudienceItems");

            migrationBuilder.DropTable(
                name: "OrderEmbeddedTeams");

            migrationBuilder.DropTable(
                name: "OrderOperatingSystemTypes");

            migrationBuilder.DropTable(
                name: "OrderSecurityTools");

            migrationBuilder.DropTable(
                name: "OrderServices");

            migrationBuilder.DropTable(
                name: "OrderSimulators");

            migrationBuilder.DropTable(
                name: "OrderSupports");

            migrationBuilder.DropTable(
                name: "OrderTerrains");

            migrationBuilder.DropTable(
                name: "OrderThreats");

            migrationBuilder.DropTable(
                name: "AssessmentTypes");

            migrationBuilder.DropTable(
                name: "AudienceItems");

            migrationBuilder.DropTable(
                name: "EmbeddedTeams");

            migrationBuilder.DropTable(
                name: "OperatingSystemTypes");

            migrationBuilder.DropTable(
                name: "SecurityTools");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "Simulators");

            migrationBuilder.DropTable(
                name: "Supports");

            migrationBuilder.DropTable(
                name: "Terrains");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Threats");

            migrationBuilder.DropTable(
                name: "Audiences");

            migrationBuilder.DropTable(
                name: "Classifications");

            migrationBuilder.DropTable(
                name: "ContentTypes");

            migrationBuilder.DropTable(
                name: "Profiles");

            migrationBuilder.DropTable(
                name: "EventTypes");

            migrationBuilder.DropTable(
                name: "Facilities");

            migrationBuilder.DropTable(
                name: "Producers");

            migrationBuilder.DropTable(
                name: "Ranks");

            migrationBuilder.DropTable(
                name: "Branches");
        }
    }
}

