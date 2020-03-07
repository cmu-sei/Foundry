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
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Foundry.Portal.DataSqlServer.Migrations
{
    public partial class removechannel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Achievements_Channels_ChannelId",
                table: "Achievements");

            migrationBuilder.DropForeignKey(
                name: "FK_Contents_Channels_ChannelId",
                table: "Contents");

            migrationBuilder.DropTable(
                name: "ChannelKeyValue");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Channels");

            migrationBuilder.DropIndex(
                name: "IX_Contents_ChannelId",
                table: "Contents");

            migrationBuilder.DropIndex(
                name: "IX_Achievements_ChannelId",
                table: "Achievements");

            migrationBuilder.DropColumn(
                name: "ChannelId",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "ChannelId",
                table: "Achievements");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChannelId",
                table: "Contents",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChannelId",
                table: "Achievements",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Channels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Access = table.Column<int>(nullable: false),
                    Copyright = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    GlobalId = table.Column<string>(nullable: false),
                    Imported = table.Column<DateTime>(nullable: true),
                    ImportedBy = table.Column<string>(nullable: true),
                    LogoUrl = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    RatingAverage = table.Column<double>(nullable: false),
                    RatingMedian = table.Column<int>(nullable: false),
                    RatingTotal = table.Column<int>(nullable: false),
                    Slug = table.Column<string>(nullable: true),
                    Summary = table.Column<string>(nullable: true),
                    TrailerUrl = table.Column<string>(nullable: true),
                    Updated = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChannelKeyValue",
                columns: table => new
                {
                    ChannelId = table.Column<int>(nullable: false),
                    Key = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelKeyValue", x => new { x.ChannelId, x.Key });
                    table.ForeignKey(
                        name: "FK_ChannelKeyValue_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChannelId = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Order = table.Column<int>(nullable: false),
                    Permissions = table.Column<int>(nullable: false),
                    ProfileId = table.Column<int>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contents_ChannelId",
                table: "Contents",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_ChannelId",
                table: "Achievements",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_Channels_Name",
                table: "Channels",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_ChannelId",
                table: "Subscriptions",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_ProfileId",
                table: "Subscriptions",
                column: "ProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Achievements_Channels_ChannelId",
                table: "Achievements",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Contents_Channels_ChannelId",
                table: "Contents",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

