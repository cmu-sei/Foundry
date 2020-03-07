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
using Foundry.Buckets.Data.Entities;
using Stack.Data.Transactions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Foundry.Buckets.Data
{
    public class BucketsDbContext : DbContext
    {
        UnitOfWork _unitOfWork;

        public DbContextOptions Options { get; }

        public BucketsDbContext(DbContextOptions options)
            : base(options)
        {
            Options = options;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Bucket>().HasIndex(p => p.GlobalId).IsUnique();
            builder.Entity<File>().HasIndex(p => p.GlobalId).IsUnique();
            builder.Entity<FileVersion>().HasIndex(p => p.GlobalId).IsUnique();

            builder.Entity<BucketAccount>().HasKey(ba => new { ba.AccountId, ba.BucketId });
        }

        public UnitOfWork CreateUnitOfWork()
        {
            if (_unitOfWork == null)
            {
                _unitOfWork = new UnitOfWork(this, IsInMemory);

                return _unitOfWork;
            }

            throw new InvalidOperationException("DbContext can only have one UnitOfWork at a time");
        }

        /// <summary>
        /// in memory database is not compatible with transactions so we need this
        /// check to allow the unit of work to manage or not manage a database transaction
        /// </summary>
        public bool IsInMemory
        {
            get
            {
                if (Options == null || Options.Extensions == null)
                    return false;

                return Options.Extensions.Any(e => e.GetType().Name == "InMemoryOptionsExtension");
            }
        }

        public override int SaveChanges()
        {
            try
            {
                ProcessChanges();
                return base.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            try
            {
                ProcessChanges();
                return base.SaveChanges(acceptAllChangesOnSuccess);
            }
            catch
            {
                throw;
            }
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                ProcessChanges();
                return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            }
            catch
            {
                throw;
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                ProcessChanges();
                return base.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                throw;
            }
        }

        void ProcessChanges()
        {
            var entries = ChangeTracker.Entries();
            var created = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {
                    if (entry.Entity is IAudit audit && audit.Created == DateTime.MinValue)
                    {
                        audit.Created = created;
                    }

                    if (entry.Entity is ISlug slug)
                    {
                        slug.Slug = slug.Name.ToSlug();
                    }

                    if (entry.Entity is IGlobal global && string.IsNullOrWhiteSpace(global.GlobalId))
                    {
                        global.GlobalId = Guid.NewGuid().ToString().ToLower();
                    }
                }
            }
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Bucket> Buckets { get; set; }
        public DbSet<BucketAccessRequest> BucketAccessRequests { get; set; }
        public DbSet<BucketAccount> BucketAccounts { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<FileVersion> FileVersions { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<FileTag> FileTags { get; set; }
    }
}
