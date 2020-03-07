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
using System.ComponentModel.DataAnnotations;

namespace Stack.Validation.Tests
{
    public class TestDbContext : DbContext
    {
        /// <summary>
        /// Database Context Options
        /// </summary>
        public DbContextOptions Options { get; }

        /// <summary>
        /// create an instance of TestDbContext
        /// </summary>
        /// <param name="options"></param>
        public TestDbContext(DbContextOptions options)
            : base(options)
        {
            Options = options;
        }

        public DbSet<Hamster> Hamsters { get; set; }
        public DbSet<Umbrella> Umbrellas { get; set; }
    }

    public class Hamster
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ColorType Color { get; set; }

        public string CreatedById { get; set; }
    }

    public class Umbrella
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ColorType Color { get; set; }

        public string CreatedById { get; set; }
    }

    public enum ColorType
    {
        Pink,
        Blue,
        Gray,
        Beige
    }
}

