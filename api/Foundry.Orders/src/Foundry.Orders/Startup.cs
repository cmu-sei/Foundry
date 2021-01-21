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
using Foundry.Orders.Cache;
using Foundry.Orders.Data;
using Foundry.Orders.Data.Repositories;
using Foundry.Orders.Identity;
using Foundry.Orders.Options;
using Foundry.Orders.Repositories;
using Foundry.Orders.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Stack.Communication.Notifications;
using Stack.Data.Options;
using Stack.DomainEvents;
using Stack.Http;
using Stack.Http.Formatters;
using Stack.Http.Identity;
using Stack.Http.Options;
using Stack.Validation.Handlers;

namespace Foundry.Orders
{
    public class Startup
    {
        AuthorizationOptions AuthorizationOptions { get; }
        IConfiguration Configuration { get; }
        string ApplicationName { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AuthorizationOptions = Configuration.GetSection("Authorization").Get<AuthorizationOptions>();
            ApplicationName = Configuration["Branding:ApplicationName"];
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbProvider(Configuration);
            services.AddDbContext<OrdersDbContext>(builder => builder.UseConfiguredDatabase(Configuration));

            services.AddMemoryCache();
            services.AddLogging();
            services.AddResponseCompression();
            services.AddResponseCaching();
            services.AddAutoMapper();

            services.AddOptions()
                .Configure<BrandingOptions>(Configuration.GetSection("Branding"))
                .AddScoped(config => config.GetService<IOptionsMonitor<BrandingOptions>>().CurrentValue)

                .Configure<FileOptions>(Configuration.GetSection("FileUpload"))
                .AddScoped(config => config.GetService<IOptionsMonitor<FileOptions>>().CurrentValue)

                .Configure<DatabaseOptions>(Configuration.GetSection("Database"))
                .AddScoped(config => config.GetService<IOptionsMonitor<DatabaseOptions>>().CurrentValue)

                .Configure<AppMailClient.Options>(Configuration.GetSection("AppMail"))
                .AddScoped(config => config.GetService<IOptionsMonitor<AppMailClient.Options>>().CurrentValue)

                .Configure<ErrorHandlingOptions>(Configuration.GetSection("ErrorHandling"))
                .AddScoped(config => config.GetService<IOptionsMonitor<ErrorHandlingOptions>>().CurrentValue)

                .Configure<AuthorizationOptions>(Configuration.GetSection("Authorization"))
                .AddSingleton(config => config.GetService<IOptionsMonitor<AuthorizationOptions>>().CurrentValue)

                .Configure<DomainEventDispatcherOptions>(Configuration.GetSection("Options:DomainEventDispatcher"))
                .AddSingleton(config => config.GetService<IOptionsMonitor<DomainEventDispatcherOptions>>().CurrentValue)

                .Configure<MessageOptions>(Configuration.GetSection("Options:MessageOptions"))
                .AddSingleton(config => config.GetService<IOptionsMonitor<MessageOptions>>().CurrentValue)

                .Configure<CommunicationOptions>(Configuration.GetSection("Options:Communication"))
                .AddSingleton(config => config.GetService<IOptionsMonitor<CommunicationOptions>>().CurrentValue);

            services
                .Configure<ClientSettings>(Configuration.GetSection("ClientSettings"))
                .AddScoped(config => config.GetService<IOptionsMonitor<ClientSettings>>().CurrentValue);

            services.Configure<SecurityHeaderOptions>(Configuration.GetSection("SecurityHeaders"))
                .AddScoped(svc => svc.GetService<IOptionsSnapshot<SecurityHeaderOptions>>().Value);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<IStackIdentityResolver, SketchIdentityResolver>();

            services.AddScoped<IValidationHandler, ServiceProviderValidationHandler>();

            services.AddSingleton<IDomainEventHandler, NotificationHandler>();
            services.AddSingleton<IDomainEventDelegator, DomainEventDelegator>();
            services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();

            services.AddScoped<FileService>();
            services.AddScoped<OrderService>();
            services.AddScoped<CommentService>();

            //add all repositories

            services.AddScoped<IAssessmentTypeRepository, AssessmentTypeRepository>();
            services.AddScoped<IAudienceRepository, AudienceRepository>();
            services.AddScoped<IAudienceItemRepository, AudienceItemRepository>();
            services.AddScoped<IBranchRepository, BranchRepository>();
            services.AddScoped<IClassificationRepository, ClassificationRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IContentTypeRepository, ContentTypeRepository>();
            services.AddScoped<IEmbeddedTeamRepository, EmbeddedTeamRepository>();
            services.AddScoped<IEventTypeRepository, EventTypeRepository>();
            services.AddScoped<IFacilityRepository, FacilityRepository>();
            services.AddScoped<IFileRepository, FileRepository>();
            services.AddScoped<IOperatingSystemTypeRepository, OperatingSystemTypeRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IProducerRepository, ProducerRepository>();
            services.AddScoped<IProfileRepository, ProfileRepository>();
            services.AddScoped<IRankRepository, RankRepository>();
            services.AddScoped<ISecurityToolRepository, SecurityToolRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<ISimulatorRepository, SimulatorRepository>();
            services.AddScoped<ISupportRepository, SupportRepository>();
            services.AddScoped<ITerrainRepository, TerrainRepository>();
            services.AddScoped<IThreatRepository, ThreatRepository>();

            services.AddScoped<IProfileCache, ProfileCache>();

            services.AddAppMailClient(() => Configuration.GetSection("AppMail").Get<AppMailClient.Options>());

            services.AddCors(options => options.UseConfiguredCors("default", Configuration.GetSection("SecurityHeaders:Cors")));

            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                });

            services.AddMvcCore(options => options.InputFormatters.Insert(0, new TextMediaTypeFormatter()))
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                })
                .AddApiExplorer()
                .AddJsonFormatters();

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = AuthorizationOptions.Authority;
                    options.RequireHttpsMetadata = AuthorizationOptions.RequireHttpsMetadata;
                    options.ApiName = AuthorizationOptions.AuthorizationScope;
                });

            services.AddSwagger(ApplicationName, AuthorizationOptions);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            bool showDeveloperExceptions = env.IsDevelopment() || Configuration.GetValue("ErrorHandling:ShowDeveloperExceptions", false);

            if (showDeveloperExceptions) app.UseDeveloperExceptionPage();
            else app.UseExceptionHandler("error");

            if (env.IsDevelopment())
            {
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true,
                    HotModuleReplacementEndpoint = "/dist/__webpack_hmr"
                });
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

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
