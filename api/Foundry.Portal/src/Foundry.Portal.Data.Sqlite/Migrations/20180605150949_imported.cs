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

namespace Foundry.Portal.Data.SqliteMigrations
{
    public partial class imported : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Imported",
                table: "Playlists",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImportedBy",
                table: "Playlists",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Imported",
                table: "Groups",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImportedBy",
                table: "Groups",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Imported",
                table: "Contents",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImportedBy",
                table: "Contents",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Imported",
                table: "Channels",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImportedBy",
                table: "Channels",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Imported",
                table: "Playlists");

            migrationBuilder.DropColumn(
                name: "ImportedBy",
                table: "Playlists");

            migrationBuilder.DropColumn(
                name: "Imported",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "ImportedBy",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "Imported",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "ImportedBy",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "Imported",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "ImportedBy",
                table: "Channels");
        }
    }
}

