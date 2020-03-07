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
using Foundry.Portal.Data.Entities;
using System;
using Stack.Http;

namespace Foundry.Portal.Tests
{
    public class TestContext<TDbContext> : IDisposable
        where TDbContext : DbContext
    {
        public TestIdentityResolver IdentityResolver { get; set; }
        public Profile _profile;
        public AutoMapper.Mapper Mapper { get; }

        public TDbContext DbContext { get; set; }

        public DbContextOptions DbContextOptions { get; }

        public TestContext(
            Func<DbContextOptions, TDbContext> dbContextResolver,
            Action<TestContext<TDbContext>> testContextSeed,
            bool useAdmin = false)
        {
            var configuration = new AutoMapper.MapperConfiguration(cfg => {
                (typeof(AutoMapper.Profile)).ProcessTypeOf("Foundry.Portal", (t) =>
                {
                    cfg.AddProfile(t);
                });
            });

            Mapper = new AutoMapper.Mapper(configuration);

            DbContextOptions = new DbContextOptionsBuilder<TDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            DbContext = dbContextResolver(DbContextOptions);

            testContextSeed(this);

        }

        public Profile Profile
        {
            get { return _profile; }
            set
            {
                _profile = value;
                IdentityResolver = new TestIdentityResolver(_profile);
            }
        }

        public void Dispose()
        {
            DbContext.Dispose();
        }

    }
}

