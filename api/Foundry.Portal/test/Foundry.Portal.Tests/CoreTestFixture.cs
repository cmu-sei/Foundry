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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Step.Core;
using Step.Core.Data;
using Step.Core.Entities;
using System;
using System.Linq;
using Xunit;

namespace Tests
{
    public class CoreTestFixture : IDisposable
    {
        public CoreTestFixture()
        {
            Initialize();
        }

        protected DbContextOptions<StepDbContext> _dbOptions;
        protected Profile _user = null;
        protected ILoggerFactory _mill = null;
        private bool _useFixture = false;
        private bool _useInMemory = false;
        private TestDataGenerator _generator;

        public bool ShouldGenerateData { get; set; }

        public DbContextOptions<StepDbContext> DBOptions
        {
            get
            {
                return _dbOptions;
            }
        }

        public Profile User
        {
            get
            {
                return _user;
            }
        }

        public ILoggerFactory Mill
        {
            get
            {
                return _mill;
            }
        }

        public bool UseFixture
        {
            get
            {
                return _useFixture;
            }
        }

        public bool UseInMemory
        {
            get
            {
                return _useInMemory;
            }
        }

        public TestDataGenerator Generator
        {
            get
            {
                return _generator;
            }
        }

        protected void Initialize()
        {
            var config = new ConfigurationBuilder()
            .AddJsonFile("testsettings.json")
            .Build();

            string test = config.GetSection("TestSettings:GenerateData").Value;
            ShouldGenerateData = Convert.ToBoolean(config.GetSection("TestSettings:GenerateData").Value);
            _useFixture = !Convert.ToBoolean(config.GetSection("TestSettings").GetSection("CreateIndividualDBInstances").Value);
            //var x = config.GetSection("TestSettings").GetSection("UseInMemoryDB").Value;
            _useInMemory = Convert.ToBoolean(config.GetSection("TestSettings").GetSection("UseInMemoryDB").Value);

            if (!_useFixture)
            {
                return;
            }

            if (_useInMemory)
            {
                _dbOptions = new DbContextOptionsBuilder<StepDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;
            }
            else
            {
                _dbOptions = new DbContextOptionsBuilder<StepDbContext>().UseSqlite(config.GetConnectionString("DefaultConnection")).Options;
            }


            _mill = new LoggerFactory(); //TestLoggerFactory();
            _mill.AddConsole(config.GetSection("Logging"));
            _mill.AddDebug();

            using (StepDbContext ctx = new StepDbContext(_dbOptions))
            {
                ctx.Database.EnsureDeleted();
                ctx.Database.EnsureCreated();
                if (ShouldGenerateData)
                {
                    _generator = new TestDataGenerator(_dbOptions, new CoreOptions(), _mill);
                    _generator.GenerateFromFile("testdata.txt");
                }
                else
                {
                    _user = AddTestUser("tester@step.local");
                }
                //_user = ctx.Users.Where(u => u.Profile.Name == "tester@step.local").Include(u=>u.Profile).SingleOrDefault();
            }


        }

        protected Profile AddTestUser(string name)
        {
            using (StepDbContext ctx = new StepDbContext(DBOptions))
            {
                Profile profile = ctx.Profiles.SingleOrDefault(p => p.Name == name);
                if (profile == null)
                {
                    profile = new Profile
                    {
                        GlobalId = Guid.NewGuid().ToString(),
                        Name = name
                    };
                    ctx.Profiles.Add(profile);
                    ctx.SaveChanges();
                }
                return profile;
            }
        }

        public void Dispose()
        {
            // using (StepDbContext ctx = new StepDbContext(_dbOptions))
            // {
            // //    ctx.Database.EnsureDeleted();
            // }
        }
    }

    [CollectionDefinition("CoreTestCollection")]
    public class CoreTestCollection : ICollectionFixture<CoreTestFixture>
    {

    }
}

