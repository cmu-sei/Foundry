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

namespace Foundry.Portal.Data.SqliteMigrations
{
    public partial class updatingPublisher : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contents_Groups_PublisherId",
                table: "Contents");

            migrationBuilder.DropForeignKey(
                name: "FK_Playlists_Groups_PublisherId",
                table: "Playlists");

            migrationBuilder.DropIndex(
                name: "IX_Playlists_PublisherId",
                table: "Playlists");

            migrationBuilder.DropIndex(
                name: "IX_Contents_PublisherId",
                table: "Contents");

            migrationBuilder.AlterColumn<string>(
                name: "PublisherId",
                table: "Playlists",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublisherName",
                table: "Playlists",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublisherSlug",
                table: "Playlists",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PublisherId",
                table: "Contents",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "Contents",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublisherName",
                table: "Contents",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublisherSlug",
                table: "Contents",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contents_GroupId",
                table: "Contents",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contents_Groups_GroupId",
                table: "Contents",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contents_Groups_GroupId",
                table: "Contents");

            migrationBuilder.DropIndex(
                name: "IX_Contents_GroupId",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "PublisherName",
                table: "Playlists");

            migrationBuilder.DropColumn(
                name: "PublisherSlug",
                table: "Playlists");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "PublisherName",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "PublisherSlug",
                table: "Contents");

            migrationBuilder.AlterColumn<int>(
                name: "PublisherId",
                table: "Playlists",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PublisherId",
                table: "Contents",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Playlists_PublisherId",
                table: "Playlists",
                column: "PublisherId");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_PublisherId",
                table: "Contents",
                column: "PublisherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contents_Groups_PublisherId",
                table: "Contents",
                column: "PublisherId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Playlists_Groups_PublisherId",
                table: "Playlists",
                column: "PublisherId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

