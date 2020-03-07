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
using Foundry.Portal.Cache;
using Foundry.Portal.Data.Entities;
using Foundry.Portal.Data.Generator.Templates;
using Foundry.Portal.Events;
using Foundry.Portal.Identity;
using Foundry.Portal.Repositories;
using Foundry.Portal.Security;
using Foundry.Portal.Services;
using Foundry.Portal.ViewModels;
using Foundry.Portal.WebHooks;
using Stack.Http;
using Stack.Http.Identity;
using Stack.Validation.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundry.Portal.Data.Generator
{
    public class DataWebHookHandler : IDomainEventHandler
    {
        public async Task<IEnumerable<DomainEventResult>> Process(IDomainEvent e)
        {
            return await Task.FromResult(new List<DomainEventResult>());
        }
    }

    public class DataDomainEventDelegator : IDomainEventDelegator
    {
        public DataDomainEventDelegator(DataWebHookHandler handler)
        {
            Handlers = new List<IDomainEventHandler>()
            {
                handler
            };
        }
        public IEnumerable<IDomainEventHandler> Handlers { get; private set; }

        public async Task<IEnumerable<DomainEventResult>> Delegate(IDomainEvent e)
        {
            return await Task.FromResult(new List<DomainEventResult>());
        }
    }

    public class DataExtensionResolver : IExtensionResolver
    {
        public DataExtensionResolver(ExtensionOptions options)
        {
            _extensionOptions = options;
        }

        public ExtensionOptions _extensionOptions { get; set; }

        public async Task<IEnumerable<IdentityClientModel>> GetPublishedClients()
        {
            return await Task.FromResult(new List<IdentityClientModel>());
        }
        public async Task<IEnumerable<WebHookTarget>> GetWebHookTargets()
        {
            return await Task.FromResult(new List<WebHookTarget>());
        }
    }

    public class DataProfileCache : IProfileCache
    {
        public Profile Get(string globalId)
        {
            return null;
        }

        public void Remove(string globalId)
        {
            return;
        }

        public Profile Set(Profile profile)
        {
            return profile;
        }

        public Profile Set(string key, Profile entity)
        {
            return entity;
        }
    }

    public class DataFactoryIdentityResolver : IStackIdentityResolver
    {
        Profile _profile;

        public DataFactoryIdentityResolver(Profile profile)
        {
            _profile = profile;
        }

        public async Task<IStackIdentity> GetIdentityAsync()
        {
            var permissions = new List<string>();

            if (_profile.Permissions.HasFlag(SystemPermissions.Administrator))
                permissions.Add(SystemPermissions.Administrator.ToString().ToLower());

            if (_profile.Permissions.HasFlag(SystemPermissions.PowerUser))
                permissions.Add(SystemPermissions.PowerUser.ToString().ToLower());

            return await Task.FromResult(new ProfileIdentity() { Id = _profile.GlobalId, Permissions = permissions.ToArray(), Profile = _profile });
        }
    }

    public class DataFactory
    {
        CoreOptions _options;
        SketchDbContext _dbContext;
        readonly ILoggerFactory _loggerFactory;
        readonly IValidationHandler _validationHandler;
        Profile _profile = null;
        ProfileService _profileService;
        ContentService _contentService;

        public AutoMapper.Mapper Mapper { get; set; }

        bool _initialized;

        const int DefaultCreateProfileCount = 10;
        const int DefaultCreateChannelCount = 20;
        const int DefaultCreateGroupCount = 20;
        const int DefaultCreateContentCount = 20;
        const int Max = 20000;

        Profile Profile
        {
            get { return _profile; }
            set
            {
                _profile = value;
                var profileCache = new DataProfileCache();
                var identityResolver = new DataFactoryIdentityResolver(_profile);
                var validationHandler = new StrictValidationHandler(_dbContext);
                var extensionResolver = new DataExtensionResolver(_options.Extension);
                var webHookHandler = new DataWebHookHandler();
                var del = new DataDomainEventDelegator(webHookHandler);
                var dispatcher = new DomainEventDispatcher(del);

                var profileRepo = new ProfileRepository(_dbContext, new ProfilePermissionMediator(identityResolver));
                var contentRepo = new ContentRepository(_dbContext, new ContentPermissionMediator(identityResolver));
                var tagRepo = new TagRepository(_dbContext);
                var discussionRepo = new DiscussionRepository(_dbContext);

                _profileService = new ProfileService(profileRepo, dispatcher, _options, identityResolver, _loggerFactory, Mapper, profileCache);
                _contentService = new ContentService(contentRepo, tagRepo, discussionRepo, profileRepo, validationHandler, dispatcher, _options, identityResolver, _loggerFactory, Mapper);
            }
        }

        public bool IsConsole { get; internal set; }

        public DataFactory(CoreOptions options, SketchDbContext ctx, ILoggerFactory mill, IValidationHandler validationHandler)
        {
            _options = options;
            _dbContext = ctx;
            _loggerFactory = mill;
            _validationHandler = validationHandler;
        }

        void Initialize()
        {
            if (!_initialized)
            {
                var configuration = new AutoMapper.MapperConfiguration(cfg => {
                    (typeof(AutoMapper.Profile)).ProcessTypeOf("Foundry.Portal", (t) =>
                    {
                        cfg.AddProfile(t);
                    });
                });

                Mapper = new AutoMapper.Mapper(configuration);

                _dbContext.Database.EnsureCreated();

                var profile = _dbContext.Profiles.FirstOrDefault();

                if (profile == null)
                {
                    var testDataGenerator = new Profile
                    {
                        Name = "Test Data Generator",
                        GlobalId = Guid.Empty.ToString(),
                        Permissions = SystemPermissions.PowerUser
                    };

                    _dbContext.Profiles.Add(testDataGenerator);
                    _dbContext.SaveChangesAsync().Wait();

                    profile = testDataGenerator;
                }

                if (profile == null)
                {
                    Console.WriteLine("Could not create initial profile");
                }
                else
                {
                    Profile = profile;
                }

                _initialized = true;
            }
        }

        public async Task<CommandResult[]> Process(params Command[] commands)
        {
            var commandResults = new List<CommandResult>();

            if (commands != null)
            {
                Initialize();

                foreach (var command in commands)
                {
                    var commandResult = new CommandResult(command) { Success = false };

                    try
                    {
                        string templateValue = command.Value.ToLower();
                        int.TryParse(command.Value, out int value);

                        switch (command.Type)
                        {
                            case CommandType.Template:
                                switch (templateValue)
                                {
                                    case "external":
                                        Console.WriteLine("Running 'ExternalDataTemplate'");
                                        new ExternalDataTemplate(_options, _dbContext, _loggerFactory, _validationHandler).Run();
                                        commandResult.Success = true;
                                        break;
                                    case "cyber":
                                        Console.WriteLine("Running 'CyberWarfareSeedTemplate'");
                                        new CyberWarfareSeedTemplate(_options, _dbContext, _loggerFactory, _validationHandler, Mapper).Run();
                                        commandResult.Success = true;
                                        break;
                                    case "ender":
                                        Console.WriteLine("Running 'EndersGameSeedTemplate'");
                                        new EndersGameSeedTemplate(_options, _dbContext, _loggerFactory, _validationHandler, Mapper).Run();
                                        commandResult.Success = true;
                                        break;
                                    default:
                                        throw new ArgumentException("Template '" + templateValue + "' is not handled");
                                }
                                break;
                            case CommandType.CreateProfiles:
                                if (value < 0 || value > Max)
                                {
                                    Console.WriteLine(commandResult.AddMessage("Value {0} is invalid, setting to default {1}", value, DefaultCreateProfileCount));
                                    value = DefaultCreateProfileCount;
                                }
                                Console.WriteLine(commandResult.AddMessage("Creating {0} Profiles", value));
                                await CreateProfiles(value);
                                commandResult.Success = true;
                                break;
                            case CommandType.CreateContent:
                                if (value < 0 || value > Max)
                                {
                                    Console.WriteLine(commandResult.AddMessage("Value {0} is invalid, setting to default {1}", value, DefaultCreateContentCount));
                                    value = DefaultCreateContentCount;
                                }
                                Console.WriteLine(commandResult.AddMessage("Creating {0} Contents", value));
                                await CreateContents(value);
                                commandResult.Success = true;
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        commandResult.Exception = ex;
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                        Console.WriteLine();
                    }

                    commandResults.Add(commandResult);
                }
            }

            return commandResults.ToArray();
        }

        async Task CreateProfiles(int profilesToCreate)
        {
            int profilesCreated = 0;

            try
            {
                Initialize();

                while (profilesCreated < profilesToCreate)
                {
                    var guid = Guid.NewGuid().ToString();

                    var profile = new ProfileCreate
                    {
                        Name = string.Format("Profile [{0}]", guid),
                        GlobalId = guid
                    };

                    var result = await _profileService.Add(profile);

                    Console.WriteLine("Profile {0} [{1}] Created", result.Id, result.GlobalId);

                    profilesCreated++;
                }

                Console.WriteLine("Created {0} Profiles", profilesCreated);
                if (IsConsole)
                {
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Create profiles failed on try {0}.", profilesCreated + 1), ex);
            }
        }

        async Task CreateContents(int contentToCreate)
        {
            int contentCreated = 0;

            try
            {
                Initialize();

                var contentTypes = Enum.GetValues(typeof(ContentType));
                int contentTypeIndex = 1;

                while (contentCreated < contentToCreate)
                {
                    var guid = Guid.NewGuid().ToString();
                    if (contentTypeIndex >= contentTypes.Length) contentTypeIndex = 1;
                    var contentType = (ContentType)contentTypes.GetValue(contentTypeIndex);

                    var content = new ContentCreate
                    {
                        Name = string.Format("Content [{0}]", guid),
                        Type = contentType,
                        Tags = new string[] { contentType.ToString() },
                        Url = "https://cert.org",
                        Description = string.Format("Content Description [{0}]", guid),
                        LogoUrl = "https://www.cs.cmu.edu/sites/default/files/fall10p05_sm_0.jpg",
                        HoverUrl = "https://www.cs.cmu.edu/sites/default/files/fall10p05_sm_0.jpg",
                        ThumbnailUrl = "https://www.cs.cmu.edu/sites/default/files/fall10p05_sm_0.jpg"
                    };

                    var result = await _contentService.Add(content);

                    Console.WriteLine("Content {0} Created", result.Id, result);

                    contentCreated++;
                    contentTypeIndex++;
                }

                Console.WriteLine("Created {0} Contents", contentCreated);

                if (IsConsole)
                {
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Create contents failed on try {0}.", contentCreated + 1), ex);
            }
        }
    }
}
