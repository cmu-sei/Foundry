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
using Foundry.Orders.Data.Entities;

namespace Foundry.Orders.Data
{
    public class OrdersDbContext : DbContext
    {
        DbContextOptions _options;

        public DbContextOptions Options => _options;

        public OrdersDbContext() { }

        public OrdersDbContext(DbContextOptions options)
            : base(options)
        {
            _options = options ?? throw new System.ArgumentNullException("options");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Profile>().HasIndex(p => p.GlobalId).IsUnique();
        }

        public DbSet<Audience> Audiences { get; set; }

        public DbSet<Classification> Classifications { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<EventType> EventTypes { get; set; }

        public DbSet<Facility> Facilities { get; set; }

        public DbSet<File> Files { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Profile> Profiles { get; set; }

        public DbSet<Producer> Producers { get; set; }

        public DbSet<Branch> Branches { get; set; }

        public DbSet<Rank> Ranks { get; set; }

        public DbSet<ContentType> ContentTypes { get; set; }

        // join primary tables
        public DbSet<AudienceItem> AudienceItems { get; set; }

        public DbSet<AssessmentType> AssessmentTypes { get; set; }

        public DbSet<OperatingSystemType> OperatingSystemTypes { get; set; }

        public DbSet<Service> Services { get; set; }

        public DbSet<Terrain> Terrains { get; set; }

        public DbSet<SecurityTool> SecurityTools { get; set; }

        public DbSet<Simulator> Simulators { get; set; }

        public DbSet<Threat> Threats { get; set; }

        public DbSet<Support> Supports { get; set; }

        public DbSet<EmbeddedTeam> EmbeddedTeams { get; set; }

        // join relation tables

        public DbSet<OrderAssessmentType> OrderAssessmentTypes { get; set; }

        public DbSet<OrderAudienceItem> OrderAudienceItems { get; set; }

        public DbSet<OrderOperatingSystemType> OrderOperatingSystemTypes { get; set; }

        public DbSet<OrderService> OrderServices { get; set; }

        public DbSet<OrderTerrain> OrderTerrains { get; set; }

        public DbSet<OrderSimulator> OrderSimulators { get; set; }

        public DbSet<OrderSecurityTool> OrderSecurityTools { get; set; }

        public DbSet<OrderThreat> OrderThreats { get; set; }

        public DbSet<OrderSupport> OrderSupports { get; set; }

        public DbSet<OrderEmbeddedTeam> OrderEmbeddedTeams { get; set; }
    }
}
