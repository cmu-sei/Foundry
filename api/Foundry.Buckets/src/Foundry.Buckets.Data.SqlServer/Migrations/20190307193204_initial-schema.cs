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

namespace Foundry.Buckets.Data.SqlServer.Migrations
{
    public partial class initialschema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    GlobalId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    IsUploadOwner = table.Column<bool>(nullable: false),
                    IsAdministrator = table.Column<bool>(nullable: false),
                    IsApplication = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.GlobalId);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    Slug = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Buckets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GlobalId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Slug = table.Column<string>(maxLength: 250, nullable: true),
                    CreatedById = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    BucketSharingType = table.Column<int>(nullable: false),
                    RestrictedKey = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buckets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Buckets_Accounts_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Accounts",
                        principalColumn: "GlobalId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BucketAccessRequests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GlobalId = table.Column<string>(nullable: true),
                    AccountId = table.Column<string>(nullable: true),
                    BucketId = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    Expires = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BucketAccessRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BucketAccessRequests_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "GlobalId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BucketAccessRequests_Buckets_BucketId",
                        column: x => x.BucketId,
                        principalTable: "Buckets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BucketAccounts",
                columns: table => new
                {
                    BucketId = table.Column<int>(nullable: false),
                    AccountId = table.Column<string>(nullable: false),
                    IsDefault = table.Column<bool>(nullable: false),
                    BucketAccessType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BucketAccounts", x => new { x.AccountId, x.BucketId });
                    table.ForeignKey(
                        name: "FK_BucketAccounts_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "GlobalId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BucketAccounts_Buckets_BucketId",
                        column: x => x.BucketId,
                        principalTable: "Buckets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GlobalId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Slug = table.Column<string>(maxLength: 250, nullable: true),
                    BucketId = table.Column<int>(nullable: false),
                    Tags = table.Column<string>(nullable: true),
                    CurrentVersionNumber = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Files_Buckets_BucketId",
                        column: x => x.BucketId,
                        principalTable: "Buckets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FileTags",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FileId = table.Column<int>(nullable: false),
                    TagName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileTags_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FileTags_Tags_TagName",
                        column: x => x.TagName,
                        principalTable: "Tags",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FileVersions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsCurrent = table.Column<bool>(nullable: false),
                    GlobalId = table.Column<string>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    VersionNumber = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Extension = table.Column<string>(nullable: true),
                    ContentType = table.Column<string>(nullable: true),
                    Length = table.Column<long>(nullable: false),
                    Height = table.Column<int>(nullable: true),
                    Width = table.Column<int>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    FileId = table.Column<int>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileVersions_Accounts_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Accounts",
                        principalColumn: "GlobalId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FileVersions_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BucketAccessRequests_AccountId",
                table: "BucketAccessRequests",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BucketAccessRequests_BucketId",
                table: "BucketAccessRequests",
                column: "BucketId");

            migrationBuilder.CreateIndex(
                name: "IX_BucketAccounts_BucketId",
                table: "BucketAccounts",
                column: "BucketId");

            migrationBuilder.CreateIndex(
                name: "IX_Buckets_CreatedById",
                table: "Buckets",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Buckets_GlobalId",
                table: "Buckets",
                column: "GlobalId",
                unique: true,
                filter: "[GlobalId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Files_BucketId",
                table: "Files",
                column: "BucketId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_GlobalId",
                table: "Files",
                column: "GlobalId",
                unique: true,
                filter: "[GlobalId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FileTags_FileId",
                table: "FileTags",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_FileTags_TagName",
                table: "FileTags",
                column: "TagName");

            migrationBuilder.CreateIndex(
                name: "IX_FileVersions_CreatedById",
                table: "FileVersions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FileVersions_FileId",
                table: "FileVersions",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_FileVersions_GlobalId",
                table: "FileVersions",
                column: "GlobalId",
                unique: true,
                filter: "[GlobalId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BucketAccessRequests");

            migrationBuilder.DropTable(
                name: "BucketAccounts");

            migrationBuilder.DropTable(
                name: "FileTags");

            migrationBuilder.DropTable(
                name: "FileVersions");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "Buckets");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}

