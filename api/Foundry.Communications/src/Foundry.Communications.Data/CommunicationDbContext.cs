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
using Foundry.Communications.Data.Entities;
using Stack.Data.Transactions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Foundry.Communications.Data
{
    public class CommunicationDbContext : DbContext
    {
        DbContextOptions _options;
        UnitOfWork _unitOfWork;

        public DbContextOptions Options => _options;

        public CommunicationDbContext(DbContextOptions options)
            : base(options)
        {
            _options = options;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

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
                if (_options == null || _options.Extensions == null)
                    return false;

                return _options.Extensions.Any(e => e.GetType().Name == "InMemoryOptionsExtension");
            }
        }

        public override int SaveChanges()
        {
            ProcessChanges();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ProcessChanges();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            ProcessChanges();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

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
                }
            }
        }

        public DbSet<Message> Messages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupTarget> GroupTargets { get; set; }
        public DbSet<Recipient> Recipients { get; set; }
        public DbSet<Source> Sources { get; set; }
        public DbSet<Target> Targets { get; set; }
    }
}
