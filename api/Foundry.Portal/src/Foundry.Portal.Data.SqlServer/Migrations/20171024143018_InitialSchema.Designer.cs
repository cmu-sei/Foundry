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
using Foundry.Portal;
using Foundry.Portal.Data;
using System;

namespace Foundry.Portal.DataSqlServer.Migrations
{
    [DbContext(typeof(SketchDbContext))]
    [Migration("20171024143018_InitialSchema")]
    partial class InitialSchema
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Step.Core.Entities.Achievement", b =>
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

                    b.Property<DateTime?>("Updated");

                    b.Property<string>("UpdatedBy");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.HasIndex("ContentId");

                    b.HasIndex("GroupId");

                    b.ToTable("Achievements");
                });

            modelBuilder.Entity("Step.Core.Entities.Channel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Access");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<string>("GlobalId")
                        .IsRequired();

                    b.Property<string>("LogoUrl");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<DateTime?>("Updated");

                    b.Property<string>("UpdatedBy");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Channels");
                });

            modelBuilder.Entity("Step.Core.Entities.Comment", b =>
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

            modelBuilder.Entity("Step.Core.Entities.CommentVote", b =>
                {
                    b.Property<int>("CommentId");

                    b.Property<int>("ProfileId");

                    b.Property<int>("Value");

                    b.HasKey("CommentId", "ProfileId");

                    b.ToTable("CommentVotes");
                });

            modelBuilder.Entity("Step.Core.Entities.Content", b =>
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

                    b.Property<string>("GlobalId");

                    b.Property<string>("HoverUrl");

                    b.Property<bool>("IsDisabled");

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

            modelBuilder.Entity("Step.Core.Entities.ContentTag", b =>
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

            modelBuilder.Entity("Step.Core.Entities.Discussion", b =>
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

                    b.Property<int>("Status");

                    b.Property<int>("Type");

                    b.Property<DateTime?>("Updated");

                    b.Property<string>("UpdatedBy");

                    b.HasKey("Id");

                    b.HasIndex("ContentId");

                    b.ToTable("Discussions");
                });

            modelBuilder.Entity("Step.Core.Entities.Group", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<string>("GlobalId");

                    b.Property<string>("LogoUrl");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("ThumbnailUrl");

                    b.Property<DateTime?>("Updated");

                    b.Property<string>("UpdatedBy");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("Step.Core.Entities.GroupFollower", b =>
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

            modelBuilder.Entity("Step.Core.Entities.Membership", b =>
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

            modelBuilder.Entity("Step.Core.Entities.Playlist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description")
                        .HasMaxLength(500);

                    b.Property<string>("GlobalId");

                    b.Property<bool>("IsDefault");

                    b.Property<bool>("IsPublic");

                    b.Property<string>("LogoUrl");

                    b.Property<string>("Name")
                        .HasMaxLength(128);

                    b.Property<int?>("ProfileId");

                    b.Property<double>("RatingAverage");

                    b.Property<int>("RatingMedian");

                    b.Property<int>("RatingTotal");

                    b.Property<DateTime?>("Updated");

                    b.Property<string>("UpdatedBy");

                    b.HasKey("Id");

                    b.HasIndex("ProfileId");

                    b.ToTable("Playlists");
                });

            modelBuilder.Entity("Step.Core.Entities.PlaylistContent", b =>
                {
                    b.Property<int>("PlaylistId");

                    b.Property<int>("ContentId");

                    b.Property<int>("Id");

                    b.Property<int>("Order");

                    b.HasKey("PlaylistId", "ContentId");

                    b.HasIndex("ContentId");

                    b.ToTable("PlaylistContents");
                });

            modelBuilder.Entity("Step.Core.Entities.Profile", b =>
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

                    b.Property<DateTime?>("Updated");

                    b.Property<string>("UpdatedBy");

                    b.HasKey("Id");

                    b.HasIndex("GlobalId")
                        .IsUnique();

                    b.ToTable("Profiles");
                });

            modelBuilder.Entity("Step.Core.Entities.ProfileAchievement", b =>
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

            modelBuilder.Entity("Step.Core.Entities.ProfileContent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ContentId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<int>("Difficulty");

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

            modelBuilder.Entity("Step.Core.Entities.ProfileContentAchievement", b =>
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

                    b.Property<DateTime?>("Updated");

                    b.Property<string>("UpdatedBy");

                    b.HasKey("Id");

                    b.HasIndex("AchievementId");

                    b.HasIndex("ContentInstanceId");

                    b.ToTable("ProfileContentAcheivements");
                });

            modelBuilder.Entity("Step.Core.Entities.ProfileFollower", b =>
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

            modelBuilder.Entity("Step.Core.Entities.Subscription", b =>
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

            modelBuilder.Entity("Step.Core.Entities.Tag", b =>
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

            modelBuilder.Entity("Step.Core.Entities.Achievement", b =>
                {
                    b.HasOne("Step.Core.Entities.Channel", "Channel")
                        .WithMany()
                        .HasForeignKey("ChannelId");

                    b.HasOne("Step.Core.Entities.Content", "Content")
                        .WithMany("Achievements")
                        .HasForeignKey("ContentId");

                    b.HasOne("Step.Core.Entities.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId");
                });

            modelBuilder.Entity("Step.Core.Entities.Comment", b =>
                {
                    b.HasOne("Step.Core.Entities.Discussion", "Discussion")
                        .WithMany("Comments")
                        .HasForeignKey("DiscussionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Step.Core.Entities.Comment", "Parent")
                        .WithMany()
                        .HasForeignKey("ParentId");

                    b.HasOne("Step.Core.Entities.Profile", "Profile")
                        .WithMany()
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Step.Core.Entities.CommentVote", b =>
                {
                    b.HasOne("Step.Core.Entities.Comment")
                        .WithMany("Votes")
                        .HasForeignKey("CommentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Step.Core.Entities.Content", b =>
                {
                    b.HasOne("Step.Core.Entities.Profile", "Author")
                        .WithMany("Contents")
                        .HasForeignKey("AuthorId");

                    b.HasOne("Step.Core.Entities.Channel", "Channel")
                        .WithMany("Contents")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Step.Core.Entities.Group", "Publisher")
                        .WithMany("Contents")
                        .HasForeignKey("PublisherId");
                });

            modelBuilder.Entity("Step.Core.Entities.ContentTag", b =>
                {
                    b.HasOne("Step.Core.Entities.Content", "Content")
                        .WithMany("ContentTags")
                        .HasForeignKey("ContentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Step.Core.Entities.Tag", "Tag")
                        .WithMany("ContentTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Step.Core.Entities.Discussion", b =>
                {
                    b.HasOne("Step.Core.Entities.Content", "Content")
                        .WithMany("Discussions")
                        .HasForeignKey("ContentId");
                });

            modelBuilder.Entity("Step.Core.Entities.GroupFollower", b =>
                {
                    b.HasOne("Step.Core.Entities.Group", "Group")
                        .WithMany("Following")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Step.Core.Entities.Playlist", "Playlist")
                        .WithMany("GroupFollowers")
                        .HasForeignKey("PlaylistId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Step.Core.Entities.Membership", b =>
                {
                    b.HasOne("Step.Core.Entities.Group", "Group")
                        .WithMany("Members")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Step.Core.Entities.Profile", "Profile")
                        .WithMany("Memberships")
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Step.Core.Entities.Playlist", b =>
                {
                    b.HasOne("Step.Core.Entities.Profile", "Profile")
                        .WithMany("Playlists")
                        .HasForeignKey("ProfileId");
                });

            modelBuilder.Entity("Step.Core.Entities.PlaylistContent", b =>
                {
                    b.HasOne("Step.Core.Entities.Content", "Content")
                        .WithMany("PlaylistContents")
                        .HasForeignKey("ContentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Step.Core.Entities.Playlist", "Playlist")
                        .WithMany("Items")
                        .HasForeignKey("PlaylistId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Step.Core.Entities.ProfileAchievement", b =>
                {
                    b.HasOne("Step.Core.Entities.Achievement", "Achievement")
                        .WithMany()
                        .HasForeignKey("AchievementId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Step.Core.Entities.Profile", "Profile")
                        .WithMany()
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Step.Core.Entities.ProfileContent", b =>
                {
                    b.HasOne("Step.Core.Entities.Content", "Content")
                        .WithMany("ProfileContents")
                        .HasForeignKey("ContentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Step.Core.Entities.ProfileContent")
                        .WithMany("Instances")
                        .HasForeignKey("ProfileContentId");

                    b.HasOne("Step.Core.Entities.Profile", "Profile")
                        .WithMany("ProfileContents")
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Step.Core.Entities.ProfileContentAchievement", b =>
                {
                    b.HasOne("Step.Core.Entities.Achievement", "Achievement")
                        .WithMany()
                        .HasForeignKey("AchievementId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Step.Core.Entities.ProfileContent", "ContentInstance")
                        .WithMany()
                        .HasForeignKey("ContentInstanceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Step.Core.Entities.ProfileFollower", b =>
                {
                    b.HasOne("Step.Core.Entities.Playlist", "Playlist")
                        .WithMany("ProfileFollowers")
                        .HasForeignKey("PlaylistId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Step.Core.Entities.Profile", "Profile")
                        .WithMany("Following")
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Step.Core.Entities.Subscription", b =>
                {
                    b.HasOne("Step.Core.Entities.Channel", "Channel")
                        .WithMany("Subscribers")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Step.Core.Entities.Profile", "Profile")
                        .WithMany("Subscriptions")
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}

