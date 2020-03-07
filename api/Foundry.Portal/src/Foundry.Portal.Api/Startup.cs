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
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Foundry.Portal.Cache;
using Foundry.Portal.Calculators;
using Foundry.Portal.Data;
using Foundry.Portal.Events;
using Foundry.Portal.Identity;
using Foundry.Portal.Mapping;
using Foundry.Portal.Messages;
using Foundry.Portal.Messages.RabbitMQ;
using Foundry.Portal.Notifications;
using Foundry.Portal.Reports;
using Foundry.Portal.Repositories;
using Foundry.Portal.Security;
using Foundry.Portal.Services;
using Foundry.Portal.WebHooks;
using Stack.Data.Options;
using Stack.Http.Formatters;
using Stack.Http.Identity;
using Stack.Http.Options;
using Stack.Validation.Handlers;

namespace Foundry.Portal.Api
{
    public class Startup
    {
        string ApplicationName { get; }
        AuthorizationOptions AuthorizationOptions = new AuthorizationOptions();
        IConfiguration Configuration { get; }

        /// <summary>
        /// constructor for Startup
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Configuration.GetSection("Authorization").Bind(AuthorizationOptions);
            ApplicationName = Configuration["Branding:ApplicationName"];
        }

        /// <summary>
        /// the supported services
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbProvider(Configuration);
            services.AddDbContextPool<SketchDbContext>(builder => builder.UseConfiguredDatabase(Configuration));

            services.AddMemoryCache();
            services.AddLogging();

            services.AddOptions()
                .Configure<BrandingOptions>(Configuration.GetSection("Branding"))
                .AddScoped(config => config.GetService<IOptionsMonitor<BrandingOptions>>().CurrentValue)

                .Configure<DatabaseOptions>(Configuration.GetSection("Database"))
                .AddScoped(config => config.GetService<IOptionsMonitor<DatabaseOptions>>().CurrentValue)

                .Configure<BucketsOptions>(Configuration.GetSection("Buckets"))
                .AddScoped(config => config.GetService<IOptionsMonitor<BucketsOptions>>().CurrentValue)

                .Configure<CoreOptions>(Configuration.GetSection("Options"))
                .AddScoped(config => config.GetService<IOptionsMonitor<CoreOptions>>().CurrentValue)

                .Configure<SeedOptions>(Configuration.GetSection("Options:Seed"))
                .AddSingleton(config => config.GetService<IOptionsMonitor<SeedOptions>>().CurrentValue)

                .Configure<CommunicationOptions>(Configuration.GetSection("Options:Communication"))
                .AddSingleton(config => config.GetService<IOptionsMonitor<CommunicationOptions>>().CurrentValue)

                .Configure<AnalyticsOptions>(Configuration.GetSection("Options:Analytics"))
                .AddScoped(config => config.GetService<IOptionsMonitor<AnalyticsOptions>>().CurrentValue)

                .Configure<ErrorHandlingOptions>(Configuration.GetSection("ErrorHandling"))
                .AddScoped(config => config.GetService<IOptionsMonitor<ErrorHandlingOptions>>().CurrentValue)

                .Configure<AuthorizationOptions>(Configuration.GetSection("Authorization"))
                .AddSingleton(config => config.GetService<IOptionsMonitor<AuthorizationOptions>>().CurrentValue)

                .Configure<ExtensionOptions>(Configuration.GetSection("Options:Extensions"))
                .AddSingleton(config => config.GetService<IOptionsMonitor<ExtensionOptions>>().CurrentValue)

                .Configure<DomainEventDispatcherOptions>(Configuration.GetSection("Options:DomainEventDispatcher"))
                .AddSingleton(config => config.GetService<IOptionsMonitor<DomainEventDispatcherOptions>>().CurrentValue)

                .Configure<RabbitMQOptions>(Configuration.GetSection("Options:RabbitMQ"))
                .AddSingleton(config => config.GetService<IOptionsMonitor<RabbitMQOptions>>().CurrentValue);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddAutoMapper(typeof(ContentProfile));

            Stack.Http.TypeExtensions.ProcessTypeOf(typeof(Service<>), "Foundry.Portal", (type) =>
            {
                if (!type.IsAbstract)
                {
                    services.AddScoped(type);
                }
            });

            services.AddScoped<ReportFactory>();
            services.AddScoped<ReportService>();

            // resolver to fetch Identity Server extensions
            services.AddSingleton<IExtensionResolver, ExtensionResolver>();

            // web hook handler processes extension web hook calls
            services.AddSingleton<IWebHookHandler, WebHookHandler>();

            // notification handler creates notifications
            services.AddSingleton<INotificationHandler, NotificationHandler>();

            // content rating calculator calculates overall rating after content is rated
            services.AddSingleton<IContentRatingCalculator, ContentRatingCalculator>();

            // playlist rating calculator calculates overall rating after content is rated
            services.AddSingleton<IPlaylistRatingCalculator, PlaylistRatingCalculator>();

            // content difficulty calculator calculates overall difficulty after content difficulty is set
            services.AddSingleton<IContentDifficultyCalculator, ContentDifficultyCalculator>();

            // domain event dispatcher handles api events
            services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();

            string provider = Configuration.GetValue("Options:DomainEventDispatcher:Handler", "WebHook");

            switch (provider)
            {
                case "RabbitMQ":
                    // if using RabbitMQ we want the publisher to be injected as the IDomainEventHandler
                    // which will drop Domain Events into the queue
                    services.AddSingleton<IDomainEventDelegator, RabbitMQMessagePublisher>();
                    // inject the RabbitMQ consumer so that Domain Events can be processed
                    services.AddSingleton<IMessageConsumer, RabbitMQMessageConsumer>();
                    break;
                default:
                    // handle domain events with the DomainEventDelegator directly
                    services.AddSingleton<IDomainEventDelegator, DomainEventDelegator>();
                    break;
            };

            services.AddScoped<IStackIdentityResolver, SketchIdentityResolver>();
            services.AddScoped<IValidationHandler, ServiceProviderValidationHandler>();

            //add all repositories
            services.AddScoped<IContentRepository, ContentRepository>();
            services.AddScoped<IDiscussionRepository, DiscussionRepository>();
            services.AddScoped<IPlaylistRepository, PlaylistRepository>();
            services.AddScoped<IProfileRepository, ProfileRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<ISettingRepository, SettingRepository>();
            services.AddScoped<IApplicationRepository, ApplicationRepository>();
            services.AddScoped<IPostRepository, PostRepository>();

            services.AddScoped<IProfileCache, ProfileCache>();
            services.AddScoped<IClientCache, ClientCache>();

            services.AddScoped<PlaylistPermissionMediator>();
            services.AddScoped<ContentPermissionMediator>();
            services.AddScoped<ProfilePermissionMediator>();

            services.AddRouting(options => { options.LowercaseUrls = true; });

            services.AddResponseCompression();
            services.AddResponseCaching();

            services.AddSwagger(ApplicationName, AuthorizationOptions);

            services.Configure<SecurityHeaderOptions>(Configuration.GetSection("SecurityHeaders"))
                .AddScoped(svc => svc.GetService<IOptionsSnapshot<SecurityHeaderOptions>>().Value);

            services.AddCors(options => options.UseConfiguredCors("default", Configuration.GetSection("CorsPolicy")));

            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                });

            services.AddMvcCore(options => options.InputFormatters.Insert(0, new TextMediaTypeFormatter()))
                .AddApiExplorer()
                .AddJsonFormatters();

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = AuthorizationOptions.Authority;
                    //options.AllowedScopes = { _authOptions.AuthorizationScope },
                    options.RequireHttpsMetadata = AuthorizationOptions.RequireHttpsMetadata;
                    options.ApiName = AuthorizationOptions.AuthorizationScope;
                });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IDomainEventDelegator delegator)
        {
            if (delegator is RabbitMQMessagePublisher)
            {
                app.UseMessageConsumer();
            }

            bool showDeveloperExceptions = env.IsDevelopment() || Configuration.GetValue("ErrorHandling:ShowDeveloperExceptions", false);

            if (showDeveloperExceptions)
            {
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseCors("default");
            app.UseResponseCompression();
            app.UseResponseCaching();
            app.UseStaticFiles();

            app.UseSwagger(c =>
            {
                c.RouteTemplate = "api/{documentName}/api.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "api";
                c.SwaggerEndpoint("/api/v1/api.json", ApplicationName + " (v1)");
                c.OAuthClientId(AuthorizationOptions.ClientId);
                c.OAuthClientSecret(AuthorizationOptions.ClientSecret);
                c.OAuthAppName(AuthorizationOptions.ClientName);
            });

            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
        }
    }
}

