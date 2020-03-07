/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Foundry.Portal.Data;
using Foundry.Portal.Data.Entities;
using Foundry.Portal.Events;
using Foundry.Portal.Repositories;
using Foundry.Portal.Security;
using Foundry.Portal.Services;
using Stack.Http;
using Stack.Validation.Handlers;
using System;
using System.Collections.Generic;

namespace Foundry.Portal.TestBed
{
    public class TestContext : IDisposable
    {
        readonly CoreOptions _coreOptions;
        readonly ILoggerFactory _loggerFactory;
        readonly IValidationHandler _validationHandler;
        TestIdentityResolver _identityResolver;
        Profile _profile;
        readonly Dictionary<Profile, Dictionary<string, object>> _serviceStore = new Dictionary<Profile, Dictionary<string, object>>();

        public TestDataFactory TestDataFactory { get; set; }

        public SketchDbContext DbContext { get; set; }

        public TestContext(CoreOptions options, SketchDbContext ctx, ILoggerFactory mill, IValidationHandler validationHandler)
        {
            _coreOptions = options;
            DbContext = ctx;
            _loggerFactory = mill;
            _validationHandler = validationHandler;

            TestDataFactory = new TestDataFactory(this);
            TestDataFactory.AddProfileAndSetContextProfile("test@this.session");
        }

        public Profile Profile
        {
            get { return _profile; }
            set
            {
                _profile = value;
                _identityResolver = new TestIdentityResolver(_profile);
            }
        }

        public TService GetService<TService>()
            where TService : class
        {
            var testProfileCache = new TestProfileCache();
            var testWebHookHandler = new TestWebHookHandler();
            var testNotificationHandler = new TestNotificationHandler();
            var testContentRatingCalculator = new TestContentRatingCalculator();
            var testContentDifficultyCalculator = new TestContentDifficultyCalculator();
            var testPlaylistRatingCalculator = new TestPlaylistRatingCalculator();
            var mockCache = new Mock<IMemoryCache>();

            var domainEventDelegator = new DomainEventDelegator(new DomainEventDispatcherOptions(),
                testWebHookHandler, testNotificationHandler, testContentRatingCalculator, testPlaylistRatingCalculator, testContentDifficultyCalculator);

            var domainEventDispatcher = new DomainEventDispatcher(domainEventDelegator);

            var configuration = new AutoMapper.MapperConfiguration(cfg => {
                (typeof(AutoMapper.Profile)).ProcessTypeOf("Foundry.Portal", (t) =>
                {
                    cfg.AddProfile(t);
                });
            });
            var mapper = new AutoMapper.Mapper(configuration);

            var type = typeof(TService);

            if (type == typeof(ContentService))
                return new ContentService(new ContentRepository(DbContext, new ContentPermissionMediator(_identityResolver)), new TagRepository(DbContext), new DiscussionRepository(DbContext), new ProfileRepository(DbContext, new ProfilePermissionMediator(_identityResolver)), _validationHandler, domainEventDispatcher, _coreOptions, _identityResolver, _loggerFactory, mapper) as TService;

            if (type == typeof(DiscussionService))
                return new DiscussionService(new DiscussionRepository(DbContext), _coreOptions, _identityResolver, _loggerFactory, mapper) as TService;

            if (type == typeof(PlaylistService))
                return new PlaylistService(new PlaylistRepository(DbContext, new PlaylistPermissionMediator(_identityResolver)), new TagRepository(DbContext), _validationHandler, _coreOptions, _identityResolver, _loggerFactory, domainEventDispatcher, mapper) as TService;

            if (type == typeof(ProfileService))
                return new ProfileService(new ProfileRepository(DbContext, new ProfilePermissionMediator(_identityResolver)), domainEventDispatcher, _coreOptions, _identityResolver, _loggerFactory, mapper, testProfileCache) as TService;

            if (type == typeof(TagService))
                return new TagService(new TagRepository(DbContext), _coreOptions, _identityResolver, _loggerFactory, mapper) as TService;

            return null;
        }

        public void Dispose() { }
    }
}
