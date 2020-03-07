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

namespace Foundry.Portal.DataSqlServer.Migrations
{
    public partial class slug : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Profiles",
                nullable: true, defaultValue: "_");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "ProfileContentAcheivements",
                nullable: true, defaultValue: "_");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Playlists",
                nullable: true, defaultValue: "_");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Notifications",
                nullable: true, defaultValue: "_");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Groups",
                nullable: true, defaultValue: "_");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Discussions",
                nullable: true, defaultValue: "_");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Contents",
                nullable: true, defaultValue: "_");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Clients",
                nullable: true, defaultValue: "_");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Channels",
                nullable: true, defaultValue: "_");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Achievements",
                nullable: true, defaultValue: "_");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "ProfileContentAcheivements");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Playlists");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Discussions");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Achievements");
        }
    }
}

