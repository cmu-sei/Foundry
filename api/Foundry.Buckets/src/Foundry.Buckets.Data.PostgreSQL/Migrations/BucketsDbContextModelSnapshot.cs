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
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Foundry.Buckets.Data;

namespace Foundry.Buckets.Data.PostgreSQL.Migrations
{
    [DbContext(typeof(BucketsDbContext))]
    partial class BucketsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Foundry.Buckets.Data.Entities.Account", b =>
                {
                    b.Property<string>("GlobalId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsAdministrator");

                    b.Property<bool>("IsApplication");

                    b.Property<bool>("IsUploadOwner");

                    b.Property<string>("Name");

                    b.HasKey("GlobalId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("Foundry.Buckets.Data.Entities.Bucket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BucketSharingType");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedById");

                    b.Property<string>("Description")
                        .HasMaxLength(1000);

                    b.Property<string>("GlobalId");

                    b.Property<string>("Name")
                        .HasMaxLength(250);

                    b.Property<string>("RestrictedKey");

                    b.Property<string>("Slug")
                        .HasMaxLength(250);

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("GlobalId")
                        .IsUnique();

                    b.ToTable("Buckets");
                });

            modelBuilder.Entity("Foundry.Buckets.Data.Entities.BucketAccessRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AccountId");

                    b.Property<int>("BucketId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedById");

                    b.Property<DateTime?>("Expires");

                    b.Property<string>("GlobalId");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("BucketId");

                    b.ToTable("BucketAccessRequests");
                });

            modelBuilder.Entity("Foundry.Buckets.Data.Entities.BucketAccount", b =>
                {
                    b.Property<string>("AccountId");

                    b.Property<int>("BucketId");

                    b.Property<int>("BucketAccessType");

                    b.Property<bool>("IsDefault");

                    b.HasKey("AccountId", "BucketId");

                    b.HasIndex("BucketId");

                    b.ToTable("BucketAccounts");
                });

            modelBuilder.Entity("Foundry.Buckets.Data.Entities.File", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BucketId");

                    b.Property<DateTime>("Created");

                    b.Property<int>("CurrentVersionNumber");

                    b.Property<string>("Description")
                        .HasMaxLength(1000);

                    b.Property<string>("GlobalId");

                    b.Property<string>("Name")
                        .HasMaxLength(250);

                    b.Property<string>("Slug")
                        .HasMaxLength(250);

                    b.Property<string>("Tags");

                    b.HasKey("Id");

                    b.HasIndex("BucketId");

                    b.HasIndex("GlobalId")
                        .IsUnique();

                    b.ToTable("Files");
                });

            modelBuilder.Entity("Foundry.Buckets.Data.Entities.FileTag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("FileId");

                    b.Property<string>("TagName");

                    b.HasKey("Id");

                    b.HasIndex("FileId");

                    b.HasIndex("TagName");

                    b.ToTable("FileTags");
                });

            modelBuilder.Entity("Foundry.Buckets.Data.Entities.FileVersion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ContentType");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedById");

                    b.Property<string>("Extension");

                    b.Property<int>("FileId");

                    b.Property<string>("GlobalId");

                    b.Property<int?>("Height");

                    b.Property<bool>("IsCurrent");

                    b.Property<long>("Length");

                    b.Property<string>("Name");

                    b.Property<string>("Path");

                    b.Property<int>("Status");

                    b.Property<int>("VersionNumber");

                    b.Property<int?>("Width");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("FileId");

                    b.HasIndex("GlobalId")
                        .IsUnique();

                    b.ToTable("FileVersions");
                });

            modelBuilder.Entity("Foundry.Buckets.Data.Entities.Tag", b =>
                {
                    b.Property<string>("Name")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Slug");

                    b.HasKey("Name");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Foundry.Buckets.Data.Entities.Bucket", b =>
                {
                    b.HasOne("Foundry.Buckets.Data.Entities.Account", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById");
                });

            modelBuilder.Entity("Foundry.Buckets.Data.Entities.BucketAccessRequest", b =>
                {
                    b.HasOne("Foundry.Buckets.Data.Entities.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId");

                    b.HasOne("Foundry.Buckets.Data.Entities.Bucket", "Bucket")
                        .WithMany()
                        .HasForeignKey("BucketId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Buckets.Data.Entities.BucketAccount", b =>
                {
                    b.HasOne("Foundry.Buckets.Data.Entities.Account", "Account")
                        .WithMany("BucketAccounts")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Foundry.Buckets.Data.Entities.Bucket", "Bucket")
                        .WithMany("BucketAccounts")
                        .HasForeignKey("BucketId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Buckets.Data.Entities.File", b =>
                {
                    b.HasOne("Foundry.Buckets.Data.Entities.Bucket", "Bucket")
                        .WithMany("Files")
                        .HasForeignKey("BucketId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Buckets.Data.Entities.FileTag", b =>
                {
                    b.HasOne("Foundry.Buckets.Data.Entities.File", "File")
                        .WithMany("FileTags")
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Foundry.Buckets.Data.Entities.Tag", "Tag")
                        .WithMany("FileTags")
                        .HasForeignKey("TagName");
                });

            modelBuilder.Entity("Foundry.Buckets.Data.Entities.FileVersion", b =>
                {
                    b.HasOne("Foundry.Buckets.Data.Entities.Account", "CreatedBy")
                        .WithMany("FileVersions")
                        .HasForeignKey("CreatedById");

                    b.HasOne("Foundry.Buckets.Data.Entities.File", "File")
                        .WithMany("FileVersions")
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}

