/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.Extensions.Logging;
using Foundry.Portal.Data.Entities;
using Foundry.Portal.Data.Generator.Models;
using Foundry.Portal.Events;
using Foundry.Portal.Repositories;
using Foundry.Portal.Security;
using Foundry.Portal.Services;
using Stack.Validation.Handlers;
using System.Collections.Generic;
using System.Linq;

namespace Foundry.Portal.Data.Generator.Templates
{
    public abstract class SeedTemplate
    {
        CoreOptions _options;
        SketchDbContext _dbContext;
        readonly ILoggerFactory _loggerFactory;
        readonly IValidationHandler _validationHandler;
        readonly AutoMapper.IMapper _mapper;

        public SeedTemplate(CoreOptions options, SketchDbContext dbContext, ILoggerFactory loggerFactory, IValidationHandler validationHandler, AutoMapper.IMapper mapper)
        {
            _options = options;
            _dbContext = dbContext;
            _loggerFactory = loggerFactory;
            _validationHandler = validationHandler;
            _mapper = mapper;
        }

        internal void ProcessTemplate(List<ProfileSeedModel> seed)
        {
            foreach (var profile in seed)
            {
                var dataProfileCache = new DataProfileCache();
                var activeProfile = _dbContext.Profiles.SingleOrDefault(p => p.GlobalId.ToLower() == profile.GlobalId.ToLower());
                var identityResolver = new DataFactoryIdentityResolver(activeProfile);

                var profileRepo = new ProfileRepository(_dbContext, new ProfilePermissionMediator(identityResolver));

                if (activeProfile == null)
                {
                    activeProfile = profileRepo.Add(new Profile() { Name = profile.Name, GlobalId = profile.GlobalId }).Result;
                }

                var validationHandler = new StrictValidationHandler(_dbContext);
                var extensionResolver = new DataExtensionResolver(_options.Extension);
                var webHookHandler = new DataWebHookHandler();
                var domainEventDelegator = new DataDomainEventDelegator(webHookHandler);
                var domainEventDispatcher = new DomainEventDispatcher(domainEventDelegator);

                var contentRepo = new ContentRepository(_dbContext, new ContentPermissionMediator(identityResolver));
                var tagRepo = new TagRepository(_dbContext);
                var discussionRepo = new DiscussionRepository(_dbContext);

                var profileService = new ProfileService(profileRepo, domainEventDispatcher, _options, identityResolver, _loggerFactory, _mapper, dataProfileCache);
                var contentService = new ContentService(contentRepo, tagRepo, discussionRepo, profileRepo, validationHandler, domainEventDispatcher, _options, identityResolver, _loggerFactory, _mapper);
            }
        }
    }
}
