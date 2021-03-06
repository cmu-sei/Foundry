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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Foundry.Portal.Data;
using System;

namespace Foundry.Portal.Data.SqliteMigrations
{
    [DbContext(typeof(SketchDbContext))]
    [Migration("20180613185417_sections")]
    partial class sections
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011");

            modelBuilder.Entity("Foundry.Portal.Data.Entities.Achievement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ChannelId");

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

                    b.HasIndex("ChannelId");

                    b.HasIndex("ContentId");

                    b.HasIndex("GroupId");

                    b.ToTable("Achievements");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.Channel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Access");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<string>("GlobalId")
                        .IsRequired();

                    b.Property<DateTime?>("Imported");

                    b.Property<string>("ImportedBy");

                    b.Property<string>("LogoUrl");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<double>("RatingAverage");

                    b.Property<int>("RatingMedian");

                    b.Property<int>("RatingTotal");

                    b.Property<string>("Slug");

                    b.Property<DateTime?>("Updated");

                    b.Property<string>("UpdatedBy");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Channels");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.ChannelKeyValue", b =>
                {
                    b.Property<int>("ChannelId");

                    b.Property<string>("Key");

                    b.Property<string>("Value");

                    b.HasKey("ChannelId", "Key");

                    b.ToTable("ChannelKeyValue");
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

                    b.Property<int>("ChannelId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<double>("DifficultyAverage");

                    b.Property<int>("DifficultyMedian");

                    b.Property<int>("DifficultyTotal");

                    b.Property<DateTime?>("End");

                    b.Property<int>("FlagCount");

                    b.Property<string>("GlobalId");

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

                    b.Property<int?>("PublisherId");

                    b.Property<double>("RatingAverage");

                    b.Property<int>("RatingMedian");

                    b.Property<int>("RatingTotal");

                    b.Property<string>("Settings");

                    b.Property<string>("Slug");

                    b.Property<DateTime?>("Start");

                    b.Property<string>("Tags");

                    b.Property<string>("ThumbnailUrl");

                    b.Property<int>("Type");

                    b.Property<DateTime?>("Updated");

                    b.Property<string>("UpdatedBy");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("ChannelId");

                    b.HasIndex("PublisherId");

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

                    b.Property<string>("ThumbnailUrl");

                    b.Property<DateTime?>("Updated");

                    b.Property<string>("UpdatedBy");

                    b.HasKey("Id");

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

            modelBuilder.Entity("Foundry.Portal.Data.Entities.Notification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<string>("DomainEventGlobalId");

                    b.Property<int?>("DomainEventType");

                    b.Property<string>("GlobalId");

                    b.Property<string>("Name");

                    b.Property<int>("ProfileId");

                    b.Property<string>("Slug");

                    b.Property<int>("Type");

                    b.Property<DateTime?>("Updated");

                    b.Property<string>("UpdatedBy");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.HasIndex("ProfileId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.PageView", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastUrl");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.ToTable("PageViews");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.Playlist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description")
                        .HasMaxLength(500);

                    b.Property<string>("GlobalId");

                    b.Property<DateTime?>("Imported");

                    b.Property<string>("ImportedBy");

                    b.Property<bool>("IsDefault");

                    b.Property<bool>("IsPublic");

                    b.Property<string>("LogoUrl");

                    b.Property<string>("Name")
                        .HasMaxLength(128);

                    b.Property<int?>("ProfileId");

                    b.Property<double>("RatingAverage");

                    b.Property<int>("RatingMedian");

                    b.Property<int>("RatingTotal");

                    b.Property<string>("Slug");

                    b.Property<DateTime?>("Updated");

                    b.Property<string>("UpdatedBy");

                    b.HasKey("Id");

                    b.HasIndex("ProfileId");

                    b.ToTable("Playlists");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.PlaylistKeyValue", b =>
                {
                    b.Property<int>("PlaylistId");

                    b.Property<string>("Key");

                    b.Property<string>("Value");

                    b.HasKey("PlaylistId", "Key");

                    b.ToTable("PlaylistKeyValue");
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

            modelBuilder.Entity("Foundry.Portal.Data.Entities.ProfileContent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("Bookmarked");

                    b.Property<int>("ContentId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<int>("Difficulty");

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

            modelBuilder.Entity("Foundry.Portal.Data.Entities.Subscription", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ChannelId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<int>("Order");

                    b.Property<int>("Permissions");

                    b.Property<int>("ProfileId");

                    b.Property<DateTime?>("Updated");

                    b.Property<string>("UpdatedBy");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.HasIndex("ProfileId");

                    b.ToTable("Subscriptions");
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
                    b.HasOne("Foundry.Portal.Data.Entities.Channel", "Channel")
                        .WithMany()
                        .HasForeignKey("ChannelId");

                    b.HasOne("Foundry.Portal.Data.Entities.Content", "Content")
                        .WithMany("Achievements")
                        .HasForeignKey("ContentId");

                    b.HasOne("Foundry.Portal.Data.Entities.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.ChannelKeyValue", b =>
                {
                    b.HasOne("Foundry.Portal.Data.Entities.Channel", "Channel")
                        .WithMany("KeyValues")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade);
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

                    b.HasOne("Foundry.Portal.Data.Entities.Channel", "Channel")
                        .WithMany("Contents")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Foundry.Portal.Data.Entities.Group", "Publisher")
                        .WithMany("Contents")
                        .HasForeignKey("PublisherId");
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

            modelBuilder.Entity("Foundry.Portal.Data.Entities.Notification", b =>
                {
                    b.HasOne("Foundry.Portal.Data.Entities.Profile", "Profile")
                        .WithMany()
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.Playlist", b =>
                {
                    b.HasOne("Foundry.Portal.Data.Entities.Profile", "Profile")
                        .WithMany("Playlists")
                        .HasForeignKey("ProfileId");
                });

            modelBuilder.Entity("Foundry.Portal.Data.Entities.PlaylistKeyValue", b =>
                {
                    b.HasOne("Foundry.Portal.Data.Entities.Playlist", "Playlist")
                        .WithMany("KeyValues")
                        .HasForeignKey("PlaylistId")
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

            modelBuilder.Entity("Foundry.Portal.Data.Entities.Subscription", b =>
                {
                    b.HasOne("Foundry.Portal.Data.Entities.Channel", "Channel")
                        .WithMany("Subscribers")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Foundry.Portal.Data.Entities.Profile", "Profile")
                        .WithMany("Subscriptions")
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}

