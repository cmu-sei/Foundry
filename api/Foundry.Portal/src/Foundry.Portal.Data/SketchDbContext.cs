/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Foundry.Portal.Data.Entities;
using Stack.Data.Transactions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Foundry.Portal.Data
{
    /// <summary>
    /// Sketch Market Database Context
    /// </summary>
    public class SketchDbContext : DbContext
    {
        /// <summary>
        /// Options
        /// </summary>
        public DbContextOptions DbContextOptions { get; }

        /// <summary>
        /// create an instance of SketchDbContext
        /// </summary>
        /// <param name="dbContextOptions"></param>
        public SketchDbContext(DbContextOptions dbContextOptions)
            : base(dbContextOptions)
        {
            DbContextOptions = dbContextOptions ?? throw new ArgumentNullException(nameof(dbContextOptions));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Profile>().HasIndex(p => p.GlobalId).IsUnique();
            builder.Entity<Client>().HasIndex(c => c.GlobalId).IsUnique();
            builder.Entity<Post>().HasIndex(p => p.GlobalId).IsUnique();

            var playlist = builder.Entity<Playlist>();
            playlist.HasIndex(p => p.Name).IsUnique();
            playlist.HasIndex(p => p.GlobalId).IsUnique();

            var content = builder.Entity<Content>();
            content.HasIndex(c => c.GlobalId).IsUnique();

            builder.Entity<CommentVote>().HasKey(v => new { v.CommentId, v.ProfileId });
            builder.Entity<PostVote>().HasKey(pv => new { pv.PostId, pv.ProfileId });
            builder.Entity<SectionContent>().HasKey(sc => new { sc.SectionId, sc.ContentId });

            builder.Entity<ProfileKeyValue>().HasKey(pkv => new { pkv.ProfileId, pkv.Key });
            builder.Entity<ContentKeyValue>().HasKey(ckv => new { ckv.ContentId, ckv.Key });
            builder.Entity<PlaylistKeyValue>().HasKey(pkv => new { pkv.PlaylistId, pkv.Key });

            builder.Entity<PlaylistGroup>().HasKey(pg => new { pg.GroupId, pg.PlaylistId });
        }

        /// <summary>
        /// in memory database is not compatible with transactions so we need this
        /// check to allow the unit of work to manage or not manage a database transaction
        /// </summary>
        public bool IsInMemory
        {
            get
            {
                if (DbContextOptions == null || DbContextOptions.Extensions == null)
                    return false;

                return DbContextOptions.Extensions.Any(e => e.GetType().Name == "InMemoryOptionsExtension");
            }
        }

        /// <summary>
        /// save changes
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            ProcessChanges();
            return base.SaveChanges();
        }

        /// <summary>
        /// save changes
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess"></param>
        /// <returns></returns>
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ProcessChanges();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        /// <summary>
        /// save changes
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            ProcessChanges();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        /// <summary>
        /// save changes
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            ProcessChanges();
            return base.SaveChangesAsync(cancellationToken);
        }

        void ProcessChanges()
        {
            var entries = ChangeTracker.Entries();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {
                    HandleEntityPrimary(entry);
                    HandleEntityGlobal(entry);
                    HandleEntityAudit(entry);
                }
            }
        }

        static void HandleEntityPrimary(EntityEntry entry)
        {
            if (entry.Entity is IEntityPrimary primary)
            {
                //maybe this length value can be pulled from the attribute?
                if (!string.IsNullOrWhiteSpace(primary.Name) && primary.Name.Length > Keys.MAXLENGTH_NAME)
                    primary.Name = primary.Name.Substring(0, Keys.MAXLENGTH_NAME);

                //maybe this length value can be pulled from the attribute?
                if (!string.IsNullOrWhiteSpace(primary.Description) && primary.Description.Length > Keys.MAXLENGTH_DESCRIPTION)
                    primary.Description = primary.Description.Substring(0, Keys.MAXLENGTH_DESCRIPTION);

                primary.Slug = primary.Name.ToUrlString();
            }
        }

        static void HandleEntityAudit(EntityEntry entry)
        {
            var audit = entry.Entity as IEntityAudit;

            if (audit != null && entry.State == EntityState.Added)
            {
                audit.Created = DateTime.UtcNow;
            }

            if (audit != null && (!audit.Updated.HasValue || audit.Updated == DateTime.MinValue) && entry.State == EntityState.Modified)
            {
                audit.Updated = DateTime.UtcNow;
            }
        }

        static void HandleEntityGlobal(EntityEntry entry)
        {

            if (entry.Entity is IEntityGlobal global)
            {
                Guid.TryParse(global.GlobalId, out Guid guid);
                if (guid.ToString() == Guid.Empty.ToString()) guid = Guid.NewGuid();
                global.GlobalId = guid.ToString().ToLower();
            }
        }

        public DbSet<Application> Applications { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<ContentTag> ContentTags { get; set; }
        public DbSet<Discussion> Discussions { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentVote> CommentVotes { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<ProfileApplication> ProfileApplications { get; set; }
        public DbSet<ProfileFollower> ProfileFollowers { get; set; }
        public DbSet<ProfileContent> ProfileContents { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostVote> PostVotes { get; set; }
        public DbSet<PostAttachment> PostAttachments { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistTag> PlaylistTags { get; set; }
        public DbSet<PlaylistGroup> PlaylistGroups { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<SectionContent> SectionContents { get; set; }
    }
}
