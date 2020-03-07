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
using Microsoft.Extensions.Logging;
using Moq;
using Stack.DomainEvents;
using Foundry.Groups.Data;
using Stack.Http;
using Stack.Http.Identity;
using Stack.Validation.Handlers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foundry.Groups.Tests
{
    public class TestContext : IDisposable
    {
        readonly ILoggerFactory _loggerFactory;
        readonly IValidationHandler _validationHandler;
        TestIdentityResolver _identityResolver;
        Account _account;
        readonly Dictionary<Account, Dictionary<string, object>> _serviceStore = new Dictionary<Account, Dictionary<string, object>>();

        public TestDataFactory TestDataFactory { get; set; }

        public GroupsDbContext DbContext { get; set; }

        public TestContext(GroupsDbContext ctx, ILoggerFactory mill, IValidationHandler validationHandler)
        {
            DbContext = ctx;
            _loggerFactory = mill;
            _validationHandler = validationHandler;

            TestDataFactory = new TestDataFactory(this);
        }

        public Account Account
        {
            get { return _account; }
            set
            {
                _account = value;
                _identityResolver = new TestIdentityResolver(_account);
            }
        }

        public void Dispose() { }

        public DomainEventDispatcher GetDomainEventDispatcher()
        {
            return new DomainEventDispatcher(new DomainEventDelegator(new DomainEventDispatcherOptions(), new List<IDomainEventHandler>()));
        }

        public StrictValidationHandler GetValidationHandler()
        {
            return new StrictValidationHandler(DbContext);
        }

        public IStackIdentityResolver GetIdentityResolver()
        {
            return _identityResolver;
        }

        public IMapper GetMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                (typeof(Profile)).ProcessTypeOf("Foundry.Groups", (t) =>
                {
                    cfg.AddProfile(t);
                });
            });
            return new Mapper(configuration);
        }
    }
}
