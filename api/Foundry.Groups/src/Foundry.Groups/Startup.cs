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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Stack.Communication.Notifications;
using Stack.Data;
using Stack.Data.Options;
using Stack.DomainEvents;
using Foundry.Groups.Cache;
using Foundry.Groups.Data;
using Foundry.Groups.Data.Repositories;
using Foundry.Groups.Identity;
using Foundry.Groups.Options;
using Foundry.Groups.Repositories;
using Foundry.Groups.Security;
using Foundry.Groups.Services;
using Stack.Http;
using Stack.Http.Formatters;
using Stack.Http.Identity;
using Stack.Http.Options;
using Stack.Validation.Handlers;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Foundry.Groups
{
    public class Startup
    {
        string RootPath { get; set; } = "";
        string ApplicationName { get; set; } = "";
        public AuthorizationOptions AuthorizationOptions { get; set; } = new AuthorizationOptions();
        public IConfiguration Configuration { get; }

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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDbProvider(Configuration);
            services.AddDbContextPool<GroupsDbContext>(builder => builder.UseConfiguredDatabase("Foundry.Groups.Data", Configuration));

            services.AddAutoMapper();
            services.AddSwagger(ApplicationName, AuthorizationOptions);

            // notification handler creates notifications
            services.AddSingleton<IDomainEventHandler, NotificationHandler>();

            // handle domain events with the DomainEventDelegator directly
            services.AddSingleton<IDomainEventDelegator, DomainEventDelegator>();

            // domain event dispatcher handles api events
            services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();

            services.AddMemoryCache();
            services.AddLogging();

            services.AddOptions()
                .Configure<BrandingOptions>(Configuration.GetSection("Branding"))
                .AddScoped(config => config.GetService<IOptionsMonitor<BrandingOptions>>().CurrentValue)

                .Configure<DatabaseOptions>(Configuration.GetSection("Database"))
                .AddScoped(config => config.GetService<IOptionsMonitor<DatabaseOptions>>().CurrentValue)

                .Configure<ErrorHandlingOptions>(Configuration.GetSection("ErrorHandling"))
                .AddScoped(config => config.GetService<IOptionsMonitor<ErrorHandlingOptions>>().CurrentValue)

                .Configure<AuthorizationOptions>(Configuration.GetSection("Authorization"))
                .AddSingleton(config => config.GetService<IOptionsMonitor<AuthorizationOptions>>().CurrentValue)

                .Configure<MarketOptions>(Configuration.GetSection("Options:Market"))
                .AddSingleton(config => config.GetService<IOptionsMonitor<MarketOptions>>().CurrentValue)

                .Configure<DomainEventDispatcherOptions>(Configuration.GetSection("Options:DomainEventDispatcher"))
                .AddSingleton(config => config.GetService<IOptionsMonitor<DomainEventDispatcherOptions>>().CurrentValue)

                .Configure<CommunicationOptions>(Configuration.GetSection("Options:Communication"))
                .AddSingleton(config => config.GetService<IOptionsMonitor<CommunicationOptions>>().CurrentValue);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IStackIdentityResolver, SketchIdentityResolver>();
            services.AddScoped<IValidationHandler, ServiceProviderValidationHandler>();

            //add all services
            services.AddScoped<GroupService>();
            services.AddScoped<GroupRequestService>();
            services.AddScoped<MemberService>();
            services.AddScoped<MemberRequestService>();
            services.AddScoped<AccountService>();
            services.AddScoped<MigrationService>();

            //add all repositories
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<IGroupRequestRepository, GroupRequestRepository>();
            services.AddScoped<IMemberRepository, MemberRepository>();
            services.AddScoped<IMemberRequestRepository, MemberRequestRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();

            services.AddScoped<IStackIdentityResolver, SketchIdentityResolver>();
            services.AddScoped<IAccountCache, AccountCache>();

            services.AddScoped<AccountPermissionMediator>();
            services.AddScoped<GroupPermissionMediator>();
            services.AddScoped<GroupRequestPermissionMediator>();
            services.AddScoped<MemberPermissionMediator>();
            services.AddScoped<MemberRequestPermissionMediator>();

            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });

            services.AddResponseCompression();
            services.AddResponseCaching();

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
                    options.RequireHttpsMetadata = AuthorizationOptions.RequireHttpsMetadata;
                    options.ApiName = AuthorizationOptions.AuthorizationScope;
                });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            bool showDeveloperExceptions = env.IsDevelopment() || Configuration.GetValue("ErrorHandling:ShowDeveloperExceptions", false);

            if (showDeveloperExceptions)
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("error");
            }

            app.UsePathBase(Configuration["PathBase"]);
            app.UseCors("default");
            app.UseResponseCompression();
            app.UseResponseCaching();
            app.UseStaticFiles();
            app.UseAuthentication();

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

            app.UseMvcWithDefaultRoute();
        }
    }
}
