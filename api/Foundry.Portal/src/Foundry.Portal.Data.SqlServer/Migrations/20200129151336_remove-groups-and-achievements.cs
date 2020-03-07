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
    public partial class removegroupsandachievements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contents_Groups_GroupId",
                table: "Contents");

            migrationBuilder.DropTable(
                name: "GroupFollowers");

            migrationBuilder.DropTable(
                name: "GroupKeyValue");

            migrationBuilder.DropTable(
                name: "Memberships");

            migrationBuilder.DropTable(
                name: "ProfileAchievements");

            migrationBuilder.DropTable(
                name: "ProfileContentAcheivements");

            migrationBuilder.DropTable(
                name: "Achievements");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Contents_GroupId",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Contents");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "Contents",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    GlobalId = table.Column<string>(nullable: true),
                    Imported = table.Column<DateTime>(nullable: true),
                    ImportedBy = table.Column<string>(nullable: true),
                    LogoUrl = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    RatingAverage = table.Column<double>(nullable: false),
                    RatingMedian = table.Column<int>(nullable: false),
                    RatingTotal = table.Column<int>(nullable: false),
                    Slug = table.Column<string>(nullable: true),
                    Summary = table.Column<string>(nullable: true),
                    ThumbnailUrl = table.Column<string>(nullable: true),
                    Updated = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Achievements",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContentId = table.Column<int>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    GlobalId = table.Column<string>(nullable: true),
                    GroupId = table.Column<int>(nullable: true),
                    LogoUrl = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Slug = table.Column<string>(nullable: true),
                    Updated = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Achievements_Contents_ContentId",
                        column: x => x.ContentId,
                        principalTable: "Contents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Achievements_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GroupFollowers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    GroupId = table.Column<int>(nullable: false),
                    PlaylistId = table.Column<int>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupFollowers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupFollowers_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupFollowers_Playlists_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "Playlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupKeyValue",
                columns: table => new
                {
                    GroupId = table.Column<int>(nullable: false),
                    Key = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupKeyValue", x => new { x.GroupId, x.Key });
                    table.ForeignKey(
                        name: "FK_GroupKeyValue_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Memberships",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GroupId = table.Column<int>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    Permissions = table.Column<int>(nullable: false),
                    ProfileId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Memberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Memberships_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Memberships_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileAchievements",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AchievementId = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    ProfileId = table.Column<int>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileAchievements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileAchievements_Achievements_AchievementId",
                        column: x => x.AchievementId,
                        principalTable: "Achievements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileAchievements_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileContentAcheivements",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AchievementId = table.Column<int>(nullable: false),
                    ContentInstanceId = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    GlobalId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Slug = table.Column<string>(nullable: true),
                    Updated = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileContentAcheivements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileContentAcheivements_Achievements_AchievementId",
                        column: x => x.AchievementId,
                        principalTable: "Achievements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileContentAcheivements_ProfileContents_ContentInstanceId",
                        column: x => x.ContentInstanceId,
                        principalTable: "ProfileContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contents_GroupId",
                table: "Contents",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_ContentId",
                table: "Achievements",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_GroupId",
                table: "Achievements",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupFollowers_GroupId",
                table: "GroupFollowers",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupFollowers_PlaylistId",
                table: "GroupFollowers",
                column: "PlaylistId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_GlobalId",
                table: "Groups",
                column: "GlobalId",
                unique: true,
                filter: "[GlobalId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_Name",
                table: "Groups",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_GroupId",
                table: "Memberships",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_ProfileId",
                table: "Memberships",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileAchievements_AchievementId",
                table: "ProfileAchievements",
                column: "AchievementId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileAchievements_ProfileId",
                table: "ProfileAchievements",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileContentAcheivements_AchievementId",
                table: "ProfileContentAcheivements",
                column: "AchievementId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileContentAcheivements_ContentInstanceId",
                table: "ProfileContentAcheivements",
                column: "ContentInstanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contents_Groups_GroupId",
                table: "Contents",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

