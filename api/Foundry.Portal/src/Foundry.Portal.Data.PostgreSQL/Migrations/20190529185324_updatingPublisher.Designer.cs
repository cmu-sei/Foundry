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
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Foundry.Portal.Data;

namespace Foundry.Portal.Data.PostgreSQL.Migrations
{
    [DbContext(typeof(SketchDbContext))]
    [Migration("20190529185324_updatingPublisher")]
    partial class updatingPublisher
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Foundry.Portal.Data.Entities.Achievement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ContentId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<string>("GlobalId");

                    b.Property<int?>("GroupId");

                    b.Property<string>("LogoUrl");

                    b.Property<string>("Name");

                    b.Property<string>("Slug");

                    b.Property<DateTime?>("Updated");

                    b.Property<string>("UpdatedBy");

                    b.HasKey("Id");

                    b.HasIndex("ContentId");

                    b.HasIndex("GroupId");

                    b.ToTable("Achievements");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.Application", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClientUri");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<string>("DisplayName");

                    b.Property<bool>("Enabled");

                    b.Property<string>("EventReferenceUri");

                    b.Property<bool>("IsHidden");

                    b.Property<bool>("IsPinned");

                    b.Property<string>("LogoUri");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Slug");

                    b.Property<DateTime?>("Updated");

                    b.Property<string>("UpdatedBy");

                    b.HasKey("Id");

                    b.ToTable("Applications");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<string>("GlobalId")
                        .IsRequired();

                    b.Property<string>("LogoUrl");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Organization");

                    b.Property<int>("Permissions");

                    b.Property<string>("Slug");

                    b.Property<DateTime?>("Updated");

                    b.Property<string>("UpdatedBy");

                    b.HasKey("Id");

                    b.HasIndex("GlobalId")
                        .IsUnique();

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.Comment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<int>("DiscussionId");

                    b.Property<string>("GlobalId");

                    b.Property<int?>("ParentId");

                    b.Property<int>("ProfileId");

                    b.Property<string>("Text")
                        .IsRequired();

                    b.Property<DateTime?>("Updated");

                    b.Property<string>("UpdatedBy");

                    b.HasKey("Id");

                    b.HasIndex("DiscussionId");

                    b.HasIndex("ParentId");

                    b.HasIndex("ProfileId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.CommentVote", b =>
                {
                    b.Property<int>("CommentId");

                    b.Property<int>("ProfileId");

                    b.Property<int>("Value");

                    b.HasKey("CommentId", "ProfileId");

                    b.ToTable("CommentVotes");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.Content", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AuthorId");

                    b.Property<string>("Copyright");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<double>("DifficultyAverage");

                    b.Property<int>("DifficultyMedian");

                    b.Property<int>("DifficultyTotal");

                    b.Property<DateTime?>("End");

                    b.Property<int>("FeaturedOrder");

                    b.Property<int>("FlagCount");

                    b.Property<string>("GlobalId");

                    b.Property<int?>("GroupId");

                    b.Property<string>("HoverUrl");

                    b.Property<DateTime?>("Imported");

                    b.Property<string>("ImportedBy");

                    b.Property<bool>("IsDisabled");

                    b.Property<bool>("IsFeatured");

                    b.Property<bool>("IsRecommended");

                    b.Property<string>("LogoUrl");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("Order");

                    b.Property<string>("PublisherId");

                    b.Property<string>("PublisherName");

                    b.Property<string>("PublisherSlug");

                    b.Property<double>("RatingAverage");

                    b.Property<int>("RatingMedian");

                    b.Property<int>("RatingTotal");

                    b.Property<string>("Settings");

                    b.Property<string>("Slug");

                    b.Property<DateTime?>("Start");

                    b.Property<string>("Summary");

                    b.Property<string>("Tags");

                    b.Property<string>("ThumbnailUrl");

                    b.Property<string>("TrailerUrl");

                    b.Property<int>("Type");

                    b.Property<DateTime?>("Updated");

                    b.Property<string>("UpdatedBy");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("GlobalId")
                        .IsUnique();

                    b.HasIndex("GroupId");

                    b.ToTable("Contents");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.ContentKeyValue", b =>
                {
                    b.Property<int>("ContentId");

                    b.Property<string>("Key");

                    b.Property<string>("Value");

                    b.HasKey("ContentId", "Key");

                    b.ToTable("ContentKeyValue");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.ContentTag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ContentId");

                    b.Property<int>("TagId");

                    b.HasKey("Id");

                    b.HasIndex("ContentId");

                    b.HasIndex("TagId");

                    b.ToTable("ContentTags");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.Discussion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ContentId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<string>("GlobalId");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Slug");

                    b.Property<int>("Status");

                    b.Property<int>("Type");

                    b.Property<DateTime?>("Updated");

                    b.Property<string>("UpdatedBy");

                    b.HasKey("Id");

                    b.HasIndex("ContentId");

                    b.ToTable("Discussions");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.Group", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<string>("GlobalId");

                    b.Property<DateTime?>("Imported");

                    b.Property<string>("ImportedBy");

                    b.Property<string>("LogoUrl");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<double>("RatingAverage");

                    b.Property<int>("RatingMedian");

                    b.Property<int>("RatingTotal");

                    b.Property<string>("Slug");

                    b.Property<string>("Summary");

                    b.Property<string>("ThumbnailUrl");

                    b.Property<DateTime?>("Updated");

                    b.Property<string>("UpdatedBy");

                    b.HasKey("Id");

                    b.HasIndex("GlobalId")
                        .IsUnique();

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.GroupFollower", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<int>("GroupId");

                    b.Property<int>("PlaylistId");

                    b.Property<DateTime?>("Updated");

                    b.Property<string>("UpdatedBy");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.HasIndex("PlaylistId");

                    b.ToTable("GroupFollowers");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.GroupKeyValue", b =>
                {
                    b.Property<int>("GroupId");

                    b.Property<string>("Key");

                    b.Property<string>("Value");

                    b.HasKey("GroupId", "Key");

                    b.ToTable("GroupKeyValue");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.Membership", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("GroupId");

                    b.Property<int>("Order");

                    b.Property<int>("Permissions");

                    b.Property<int>("ProfileId");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.HasIndex("ProfileId");

                    b.ToTable("Memberships");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.Playlist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Copyright");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<int>("FeaturedOrder");

                    b.Property<string>("GlobalId")
                        .IsRequired();

                    b.Property<DateTime?>("Imported");

                    b.Property<string>("ImportedBy");

                    b.Property<bool>("IsDefault");

                    b.Property<bool>("IsDisabled");

                    b.Property<bool>("IsFeatured");

                    b.Property<bool>("IsPublic");

                    b.Property<bool>("IsRecommended");

                    b.Property<string>("LogoUrl");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int?>("ProfileId");

                    b.Property<string>("PublisherId");

                    b.Property<string>("PublisherName");

                    b.Property<string>("PublisherSlug");

                    b.Property<double>("RatingAverage");

                    b.Property<int>("RatingMedian");

                    b.Property<int>("RatingTotal");

                    b.Property<string>("Slug");

                    b.Property<string>("Summary");

                    b.Property<string>("Tags");

                    b.Property<string>("TrailerUrl");

                    b.Property<DateTime?>("Updated");

                    b.Property<string>("UpdatedBy");

                    b.HasKey("Id");

                    b.HasIndex("GlobalId")
                        .IsUnique();

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("ProfileId");

                    b.ToTable("Playlists");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.PlaylistGroup", b =>
                {
                    b.Property<string>("GroupId");

                    b.Property<int>("PlaylistId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime?>("Updated");

                    b.Property<string>("UpdatedBy");

                    b.HasKey("GroupId", "PlaylistId");

                    b.HasIndex("PlaylistId");

                    b.ToTable("PlaylistGroups");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.PlaylistKeyValue", b =>
                {
                    b.Property<int>("PlaylistId");

                    b.Property<string>("Key");

                    b.Property<string>("Value");

                    b.HasKey("PlaylistId", "Key");

                    b.ToTable("PlaylistKeyValue");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.PlaylistTag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("PlaylistId");

                    b.Property<int>("TagId");

                    b.HasKey("Id");

                    b.HasIndex("PlaylistId");

                    b.HasIndex("TagId");

                    b.ToTable("PlaylistTags");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("GlobalId")
                        .IsRequired();

                    b.Property<int?>("ParentId");

                    b.Property<int>("ProfileId");

                    b.Property<string>("Text")
                        .IsRequired();

                    b.Property<DateTime?>("Updated");

                    b.Property<string>("UpdatedBy");

                    b.Property<int>("Value");

                    b.HasKey("Id");

                    b.HasIndex("GlobalId")
                        .IsUnique();

                    b.HasIndex("ParentId");

                    b.HasIndex("ProfileId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.PostAttachment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("PostId");

                    b.Property<string>("Url")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.ToTable("PostAttachments");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.PostVote", b =>
                {
                    b.Property<int>("PostId");

                    b.Property<int>("ProfileId");

                    b.Property<int>("Value");

                    b.HasKey("PostId", "ProfileId");

                    b.ToTable("PostVotes");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.Profile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<string>("GlobalId")
                        .IsRequired();

                    b.Property<bool>("IsDisabled");

                    b.Property<string>("LogoUrl");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Organization");

                    b.Property<int>("Permissions");

                    b.Property<double>("RatingAverage");

                    b.Property<int>("RatingMedian");

                    b.Property<int>("RatingTotal");

                    b.Property<string>("Slug");

                    b.Property<DateTime?>("Updated");

                    b.Property<string>("UpdatedBy");

                    b.HasKey("Id");

                    b.HasIndex("GlobalId")
                        .IsUnique();

                    b.ToTable("Profiles");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.ProfileAchievement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AchievementId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<int>("ProfileId");

                    b.Property<DateTime?>("Updated");

                    b.Property<string>("UpdatedBy");

                    b.HasKey("Id");

                    b.HasIndex("AchievementId");

                    b.HasIndex("ProfileId");

                    b.ToTable("ProfileAchievements");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.ProfileApplication", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ApplicationId");

                    b.Property<int>("ProfileId");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationId");

                    b.HasIndex("ProfileId");

                    b.ToTable("ProfileApplications");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.ProfileContent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("Bookmarked");

                    b.Property<int>("ContentId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<int>("Difficulty");

                    b.Property<int>("FlagStatus");

                    b.Property<DateTime?>("Flagged");

                    b.Property<string>("FlaggedComment");

                    b.Property<int?>("ProfileContentId");

                    b.Property<int>("ProfileId");

                    b.Property<int>("Rating");

                    b.Property<DateTime?>("Updated");

                    b.Property<string>("UpdatedBy");

                    b.HasKey("Id");

                    b.HasIndex("ContentId");

                    b.HasIndex("ProfileContentId");

                    b.HasIndex("ProfileId");

                    b.ToTable("ProfileContents");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.ProfileContentAchievement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AchievementId");

                    b.Property<int>("ContentInstanceId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<string>("GlobalId");

                    b.Property<string>("Name");

                    b.Property<string>("Slug");

                    b.Property<DateTime?>("Updated");

                    b.Property<string>("UpdatedBy");

                    b.HasKey("Id");

                    b.HasIndex("AchievementId");

                    b.HasIndex("ContentInstanceId");

                    b.ToTable("ProfileContentAcheivements");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.ProfileFollower", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<int>("PlaylistId");

                    b.Property<int>("ProfileId");

                    b.Property<int>("Rating");

                    b.Property<DateTime?>("Updated");

                    b.Property<string>("UpdatedBy");

                    b.HasKey("Id");

                    b.HasIndex("PlaylistId");

                    b.HasIndex("ProfileId");

                    b.ToTable("ProfileFollowers");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.ProfileKeyValue", b =>
                {
                    b.Property<int>("ProfileId");

                    b.Property<string>("Key");

                    b.Property<string>("Value");

                    b.HasKey("ProfileId", "Key");

                    b.ToTable("ProfileKeyValue");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.Section", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int>("Order");

                    b.Property<int>("PlaylistId");

                    b.Property<string>("Slug");

                    b.HasKey("Id");

                    b.HasIndex("PlaylistId");

                    b.ToTable("Sections");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.SectionContent", b =>
                {
                    b.Property<int>("SectionId");

                    b.Property<int>("ContentId");

                    b.Property<int>("Id");

                    b.Property<int>("Order");

                    b.HasKey("SectionId", "ContentId");

                    b.HasIndex("ContentId");

                    b.ToTable("SectionContents");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.Setting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Key");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Slug");

                    b.Property<DateTime?>("Updated");

                    b.Property<string>("UpdatedBy");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.Achievement", b =>
                {
                    b.HasOne("Foundry.Portal.Data.Entities.Content", "Content")
                        .WithMany("Achievements")
                        .HasForeignKey("ContentId");

                    b.HasOne("Foundry.Portal.Data.Entities.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.Comment", b =>
                {
                    b.HasOne("Foundry.Portal.Data.Entities.Discussion", "Discussion")
                        .WithMany("Comments")
                        .HasForeignKey("DiscussionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Foundry.Portal.Data.Entities.Comment", "Parent")
                        .WithMany()
                        .HasForeignKey("ParentId");

                    b.HasOne("Foundry.Portal.Data.Entities.Profile", "Profile")
                        .WithMany()
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.CommentVote", b =>
                {
                    b.HasOne("Foundry.Portal.Data.Entities.Comment")
                        .WithMany("Votes")
                        .HasForeignKey("CommentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.Content", b =>
                {
                    b.HasOne("Foundry.Portal.Data.Entities.Profile", "Author")
                        .WithMany("Contents")
                        .HasForeignKey("AuthorId");

                    b.HasOne("Foundry.Portal.Data.Entities.Group")
                        .WithMany("Contents")
                        .HasForeignKey("GroupId");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.ContentKeyValue", b =>
                {
                    b.HasOne("Foundry.Portal.Data.Entities.Content", "Content")
                        .WithMany("KeyValues")
                        .HasForeignKey("ContentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.ContentTag", b =>
                {
                    b.HasOne("Foundry.Portal.Data.Entities.Content", "Content")
                        .WithMany("ContentTags")
                        .HasForeignKey("ContentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Foundry.Portal.Data.Entities.Tag", "Tag")
                        .WithMany("ContentTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.Discussion", b =>
                {
                    b.HasOne("Foundry.Portal.Data.Entities.Content", "Content")
                        .WithMany("Discussions")
                        .HasForeignKey("ContentId");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.GroupFollower", b =>
                {
                    b.HasOne("Foundry.Portal.Data.Entities.Group", "Group")
                        .WithMany("Following")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Foundry.Portal.Data.Entities.Playlist", "Playlist")
                        .WithMany("GroupFollowers")
                        .HasForeignKey("PlaylistId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.GroupKeyValue", b =>
                {
                    b.HasOne("Foundry.Portal.Data.Entities.Group", "Group")
                        .WithMany("KeyValues")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.Membership", b =>
                {
                    b.HasOne("Foundry.Portal.Data.Entities.Group", "Group")
                        .WithMany("Members")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Foundry.Portal.Data.Entities.Profile", "Profile")
                        .WithMany("Memberships")
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.Playlist", b =>
                {
                    b.HasOne("Foundry.Portal.Data.Entities.Profile", "Profile")
                        .WithMany("Playlists")
                        .HasForeignKey("ProfileId");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.PlaylistGroup", b =>
                {
                    b.HasOne("Foundry.Portal.Data.Entities.Playlist", "Playlist")
                        .WithMany()
                        .HasForeignKey("PlaylistId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.PlaylistKeyValue", b =>
                {
                    b.HasOne("Foundry.Portal.Data.Entities.Playlist", "Playlist")
                        .WithMany("KeyValues")
                        .HasForeignKey("PlaylistId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.PlaylistTag", b =>
                {
                    b.HasOne("Foundry.Portal.Data.Entities.Playlist", "Playlist")
                        .WithMany("PlaylistTags")
                        .HasForeignKey("PlaylistId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Foundry.Portal.Data.Entities.Tag", "Tag")
                        .WithMany("PlaylistTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.Post", b =>
                {
                    b.HasOne("Foundry.Portal.Data.Entities.Post", "Parent")
                        .WithMany()
                        .HasForeignKey("ParentId");

                    b.HasOne("Foundry.Portal.Data.Entities.Profile", "Profile")
                        .WithMany()
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.PostAttachment", b =>
                {
                    b.HasOne("Foundry.Portal.Data.Entities.Post")
                        .WithMany("Attachments")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.PostVote", b =>
                {
                    b.HasOne("Foundry.Portal.Data.Entities.Post")
                        .WithMany("Votes")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.ProfileAchievement", b =>
                {
                    b.HasOne("Foundry.Portal.Data.Entities.Achievement", "Achievement")
                        .WithMany()
                        .HasForeignKey("AchievementId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Foundry.Portal.Data.Entities.Profile", "Profile")
                        .WithMany()
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.ProfileApplication", b =>
                {
                    b.HasOne("Foundry.Portal.Data.Entities.Application", "Application")
                        .WithMany("ProfileApplications")
                        .HasForeignKey("ApplicationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Foundry.Portal.Data.Entities.Profile", "Profile")
                        .WithMany()
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.ProfileContent", b =>
                {
                    b.HasOne("Foundry.Portal.Data.Entities.Content", "Content")
                        .WithMany("ProfileContents")
                        .HasForeignKey("ContentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Foundry.Portal.Data.Entities.ProfileContent")
                        .WithMany("ProfileContents")
                        .HasForeignKey("ProfileContentId");

                    b.HasOne("Foundry.Portal.Data.Entities.Profile", "Profile")
                        .WithMany("ProfileContents")
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.ProfileContentAchievement", b =>
                {
                    b.HasOne("Foundry.Portal.Data.Entities.Achievement", "Achievement")
                        .WithMany()
                        .HasForeignKey("AchievementId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Foundry.Portal.Data.Entities.ProfileContent", "ContentInstance")
                        .WithMany()
                        .HasForeignKey("ContentInstanceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.ProfileFollower", b =>
                {
                    b.HasOne("Foundry.Portal.Data.Entities.Playlist", "Playlist")
                        .WithMany("ProfileFollowers")
                        .HasForeignKey("PlaylistId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Foundry.Portal.Data.Entities.Profile", "Profile")
                        .WithMany("Following")
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.ProfileKeyValue", b =>
                {
                    b.HasOne("Foundry.Portal.Data.Entities.Profile", "Profile")
                        .WithMany("KeyValues")
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.Section", b =>
                {
                    b.HasOne("Foundry.Portal.Data.Entities.Playlist", "Playlist")
                        .WithMany("Sections")
                        .HasForeignKey("PlaylistId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.SectionContent", b =>
                {
                    b.HasOne("Foundry.Portal.Data.Entities.Content", "Content")
                        .WithMany("SectionContents")
                        .HasForeignKey("ContentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Foundry.Portal.Data.Entities.Section", "Section")
                        .WithMany("SectionContents")
                        .HasForeignKey("SectionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}

