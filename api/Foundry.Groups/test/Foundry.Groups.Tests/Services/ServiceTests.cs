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
using Foundry.Groups.Data;
using Foundry.Groups.Repositories;
using Foundry.Groups.Security;
using Foundry.Groups.Services;
using Foundry.Groups.ViewModels;
using Stack.Validation.Handlers;
using System;
using System.Threading.Tasks;

namespace Foundry.Groups.Tests
{
    public class Logger<T> : ILogger<T>
        where T : class
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
        }
    }

    public abstract class ServiceTests
    {
        public ServiceTests()
        {
            Initialize();
        }

        public Account GetAdministrator()
        {
            return new Account { Id = Guid.NewGuid().ToString(), Name = "Administrator", IsAdministrator = true };
        }

        public Account GetUser()
        {
            return new Account { Id = Guid.NewGuid().ToString(), Name = "User" };
        }

        protected ILoggerFactory _loggerFactory;
        public DbContextOptions<GroupsDbContext> _dbContextOptions;

        protected TestContext CreateTestContext(Account account)
        {
            var dbContext = new GroupsDbContext(_dbContextOptions);
            var identityResolver = new TestIdentityResolver(GetUser());

            return new TestContext(dbContext, _loggerFactory, new StrictValidationHandler(dbContext, identityResolver))
            {
                Account = account
            };
        }

        protected async Task<Account> CreateAccount(TestContext context, string value = null)
        {
            value = value ?? Guid.NewGuid().ToString();

            var account = new Account { Name = value };
            await context.DbContext.Accounts.AddAsync(account);
            await context.DbContext.SaveChangesAsync();

            return account;
        }

        protected GroupCreate GetGroupCreate(string value = null)
        {
            value = value ?? Guid.NewGuid().ToString();
            return new GroupCreate() { Name = value, Description = value, LogoUrl = "http://logo.url", Summary = value };
        }

        protected GroupUpdate GetGroupUpdate(string id, string value = null)
        {
            value = value ?? Guid.NewGuid().ToString();
            return new GroupUpdate() { Id = id, Name = value, Description = value, LogoUrl = "http://logo.url", Summary = value };
        }

        protected MemberRequestService GetMemberRequestService(TestContext context)
        {
            var idr = context.GetIdentityResolver();

            var memberRequestService = new MemberRequestService(
                context.GetDomainEventDispatcher(),
                idr,
                new MemberRequestRepository(context.DbContext, new MemberRequestPermissionMediator(idr)),
                context.GetMapper(),
                context.GetValidationHandler(), new Logger<MemberRequestService>());

            return memberRequestService;
        }

        protected GroupRequestService GetGroupRequestService(TestContext context)
        {
            var idr = context.GetIdentityResolver();

            var groupRequestService = new GroupRequestService(
                idr,
                new GroupRequestRepository(context.DbContext, new GroupRequestPermissionMediator(idr)),
                context.GetMapper(),
                context.GetValidationHandler());

            return groupRequestService;
        }

        protected GroupService GetGroupService(TestContext context)
        {
            var idr = context.GetIdentityResolver();

            var groupService = new GroupService(
                context.GetDomainEventDispatcher(),
                idr,
                new GroupRepository(context.DbContext, new GroupPermissionMediator(idr)),
                context.GetMapper(),
                context.GetValidationHandler(), new Logger<GroupService>());

            return groupService;
        }

        protected MemberService GetMemberService(TestContext context)
        {
            var idr = context.GetIdentityResolver();

            var memberService = new MemberService(

                context.GetDomainEventDispatcher(),
                idr,
                new MemberRepository(context.DbContext, new MemberPermissionMediator(idr)),
                context.GetMapper(),
                context.GetValidationHandler(), new Logger<MemberService>());

            return memberService;
        }

        public void Initialize()
        {
            _loggerFactory = new LoggerFactory();
            _loggerFactory.AddConsole(LogLevel.Debug);
            _loggerFactory.AddDebug();

            _dbContextOptions = new DbContextOptionsBuilder<GroupsDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            //Profile testProfile = null;

            using (var ctx = CreateTestContext(null))
            {
                ctx.DbContext.Database.EnsureDeleted();
                ctx.DbContext.Database.EnsureCreated();
                //testProfile = ctx.TestDataFactory.AddProfileAndSetContextProfile("tester@step.local");
            }
        }
    }
}

