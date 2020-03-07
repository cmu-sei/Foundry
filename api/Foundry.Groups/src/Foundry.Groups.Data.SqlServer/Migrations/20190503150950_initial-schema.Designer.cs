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
using Foundry.Groups.Data;

namespace Foundry.Groups.Data.SqlServer.Migrations
{
    [DbContext(typeof(GroupsDbContext))]
    [Migration("20190503150950_initial-schema")]
    partial class initialschema
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Foundry.Groups.Data.Account", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<bool>("IsAdministrator");

                    b.Property<string>("Name");

                    b.Property<string>("Slug");

                    b.Property<DateTime?>("Updated");

                    b.HasKey("Id");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("Foundry.Groups.Data.Group", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("Description");

                    b.Property<string>("Key");

                    b.Property<string>("LogoUrl");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("ParentId");

                    b.Property<string>("Slug")
                        .IsRequired();

                    b.Property<string>("Summary");

                    b.Property<DateTime?>("Updated");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("Foundry.Groups.Data.GroupRequest", b =>
                {
                    b.Property<string>("ParentGroupId");

                    b.Property<string>("ChildGroupId");

                    b.Property<DateTime>("Created");

                    b.Property<int>("Status");

                    b.Property<DateTime?>("Updated");

                    b.HasKey("ParentGroupId", "ChildGroupId");

                    b.HasIndex("ChildGroupId");

                    b.ToTable("GroupRequests");
                });

            modelBuilder.Entity("Foundry.Groups.Data.Member", b =>
                {
                    b.Property<string>("GroupId");

                    b.Property<string>("AccountId");

                    b.Property<DateTime>("Created");

                    b.Property<bool>("IsManager");

                    b.Property<bool>("IsOwner");

                    b.Property<DateTime?>("Updated");

                    b.HasKey("GroupId", "AccountId");

                    b.HasIndex("AccountId");

                    b.ToTable("Members");
                });

            modelBuilder.Entity("Foundry.Groups.Data.MemberRequest", b =>
                {
                    b.Property<string>("GroupId");

                    b.Property<string>("AccountId");

                    b.Property<DateTime>("Created");

                    b.Property<int>("Status");

                    b.Property<DateTime?>("Updated");

                    b.HasKey("GroupId", "AccountId");

                    b.HasIndex("AccountId");

                    b.ToTable("MemberRequests");
                });

            modelBuilder.Entity("Foundry.Groups.Data.Group", b =>
                {
                    b.HasOne("Foundry.Groups.Data.Group", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");
                });

            modelBuilder.Entity("Foundry.Groups.Data.GroupRequest", b =>
                {
                    b.HasOne("Foundry.Groups.Data.Group", "ChildGroup")
                        .WithMany()
                        .HasForeignKey("ChildGroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Foundry.Groups.Data.Group", "ParentGroup")
                        .WithMany()
                        .HasForeignKey("ParentGroupId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Groups.Data.Member", b =>
                {
                    b.HasOne("Foundry.Groups.Data.Account", "Account")
                        .WithMany("Members")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Foundry.Groups.Data.Group", "Group")
                        .WithMany("Members")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Groups.Data.MemberRequest", b =>
                {
                    b.HasOne("Foundry.Groups.Data.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Foundry.Groups.Data.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
