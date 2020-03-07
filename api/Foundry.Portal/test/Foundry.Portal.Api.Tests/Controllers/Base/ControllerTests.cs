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
using Microsoft.Extensions.Logging;
using Foundry.Portal.Data;
using Foundry.Portal.TestBed;
using Stack.Validation.Handlers;
using System;

namespace Foundry.Portal.Api.Tests.Controllers
{
    public abstract class ControllerTests
    {
        protected const string _complexPassword = "!@#WeRth^65$098";
        protected const string _name = "tester@step.local";

        ILoggerFactory _loggerFactory;
        DbContextOptions<SketchDbContext> _dbContextOptions;

        public ControllerTests()
        {
            Initialize();
        }

        protected TestContext CreateTestContext()
        {
            var dbContext = new SketchDbContext(_dbContextOptions);
            return new TestContext(new CoreOptions(), dbContext, _loggerFactory, new StrictValidationHandler(dbContext));
        }

        public void Initialize()
        {
            _loggerFactory = new LoggerFactory();
            _loggerFactory.AddConsole(LogLevel.Debug);
            _loggerFactory.AddDebug();

            _dbContextOptions = new DbContextOptionsBuilder<SketchDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var ctx = CreateTestContext())
            {
                ctx.DbContext.Database.EnsureDeleted();
                ctx.DbContext.Database.EnsureCreated();
                ctx.TestDataFactory.AddProfileAndSetContextProfile(_name);
            }
        }
    }
}

