/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Moq;
using Foundry.Buckets.Storage;
using Stack.Http;
using Stack.Http.Identity;
using System;

namespace Foundry.Buckets.Tests
{
    public class TestContext<TDbContext> : IDisposable
        where TDbContext : DbContext
    {
        public IStackIdentityResolver IdentityResolver { get; set; }

        public IStorageProvider StorageProvider { get; set; }

        IStackIdentity _identity;
        public IStackIdentity Identity
        {
            get { return _identity; }
            set
            {
                _identity = value;
                IdentityResolver = new TestIdentityResolver(_identity);
            }
        }

        public IMapper Mapper { get; }

        public TDbContext DbContext { get; set; }

        public IHostingEnvironment HostingEnvironment {get;set;}

        public DbContextOptions DbContextOptions { get; set; }

        public TestContext(
            Func<DbContextOptions, TDbContext> dbContextResolver,
            Action<TestContext<TDbContext>> testContextSeed = null,
            bool useAdmin = false)
        {
            var configuration = new MapperConfiguration(cfg => {
                (typeof(Profile)).ProcessTypeOf("Foundry.Buckets", (t) =>
                {
                    cfg.AddProfile(t);
                });
            });

            Mapper = new Mapper(configuration);

            var mockHostingEnvironment = new Mock<IHostingEnvironment>();

            mockHostingEnvironment
                .Setup(m => m.EnvironmentName)
                .Returns("Hosting:UnitTestEnvironment");

            HostingEnvironment = mockHostingEnvironment.Object;

            StorageProvider = new TestStorageProvider();

            DbContextOptions = new DbContextOptionsBuilder<TDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            DbContext = dbContextResolver(DbContextOptions);

            testContextSeed?.Invoke(this);
        }

        public void Dispose()
        {
            DbContext.Dispose();
        }

    }
}

