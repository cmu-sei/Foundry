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
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Foundry.Groups.Data
{
    public class GroupsDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<Member> Members { get; set; }

        public DbSet<GroupRequest> GroupRequests { get; set; }

        public DbSet<MemberRequest> MemberRequests { get; set; }

        public DbContextOptions Options { get; }

        public GroupsDbContext(DbContextOptions options)
            : base(options)
        {
            Options = options;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Member>().HasKey(m => new { m.GroupId, m.AccountId });
            builder.Entity<MemberRequest>().HasKey(m => new { m.GroupId, m.AccountId });
            builder.Entity<GroupRequest>().HasKey(m => new { m.ParentGroupId, m.ChildGroupId });
        }

        /// <summary>
        /// save changes
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            PreProcesses();
            return base.SaveChanges();
        }

        /// <summary>
        /// save changes
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess"></param>
        /// <returns></returns>
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            PreProcesses();
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
            PreProcesses();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        /// <summary>
        /// save changes
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            PreProcesses();
            return base.SaveChangesAsync(cancellationToken);
        }

        void PreProcesses()
        {
            var entries = ChangeTracker.Entries();

            foreach (var entry in entries)
            {
                ProcessIAudit(entry);
                ProcessISlug(entry);
            }
        }

        private static void ProcessISlug(EntityEntry entry)
        {
            if (entry.Entity is ISlug slug)
            {
                var urlString = slug.Name.ToUrlString();

                if (string.IsNullOrWhiteSpace(slug.Slug) || slug.Slug != urlString)
                {
                    slug.Slug = urlString;
                }
            }
        }

        private static void ProcessIAudit(EntityEntry entry)
        {
            if (entry.Entity is IAudit audit)
            {
                if (entry.State == EntityState.Added)
                {
                    audit.Created = DateTime.UtcNow;
                }

                if (entry.State == EntityState.Modified)
                {
                    audit.Updated = DateTime.UtcNow;
                }
            }
        }
    }
}

