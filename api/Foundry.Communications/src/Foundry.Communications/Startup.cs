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
using Foundry.Communications.Data;
using Foundry.Communications.Data.Repositories;
using Foundry.Communications.Hubs;
using Foundry.Communications.Identity;
using Foundry.Communications.Repositories;
using Foundry.Communications.Services;
using Stack.Data;
using Stack.Data.Options;
using Stack.Http;
using Stack.Http.Formatters;
using Stack.Http.Identity;
using Stack.Http.Options;

namespace Foundry.Communications
{
    /// <summary>
    /// Foundry.Communications startup
    /// </summary>
    public class Startup
    {
        const string xmlFileName = "Foundry.Communications.xml";
        public IConfiguration Configuration { get; }
        public string ApplicationName { get; }
        public AuthorizationOptions AuthorizationOptions { get; }

        /// <summary>
        /// create an instance of startup
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AuthorizationOptions = Configuration.GetSection("Authorization").Get<AuthorizationOptions>();
            ApplicationName = Configuration["Branding:ApplicationName"];
        }

        /// <summary>
        /// configure supported services for Foundry.Communications
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbProvider(Configuration);
            services.AddDbContextPool<CommunicationDbContext>(builder => builder.UseConfiguredDatabase("Foundry.Communications.Data", Configuration));

            services.AddOptions()
                .Configure<BrandingOptions>(Configuration.GetSection("Branding"))
                .AddScoped(config => config.GetService<IOptionsMonitor<BrandingOptions>>().CurrentValue)

                .Configure<DatabaseOptions>(Configuration.GetSection("Database"))
                .AddScoped(config => config.GetService<IOptionsMonitor<DatabaseOptions>>().CurrentValue)

                .Configure<ErrorHandlingOptions>(Configuration.GetSection("ErrorHandling"))
                .AddScoped(config => config.GetService<IOptionsMonitor<ErrorHandlingOptions>>().CurrentValue)

                .Configure<AuthorizationOptions>(Configuration.GetSection("Authorization"))
                .AddSingleton(config => config.GetService<IOptionsMonitor<AuthorizationOptions>>().CurrentValue);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddAutoMapper();

            services.AddScoped<IStackIdentityResolver, SketchIdentityResolver>();

            services.AddScoped<GroupService>();
            services.AddScoped<GroupTargetService>();
            services.AddScoped<MessageService>();
            services.AddScoped<NotificationService>();
            services.AddScoped<RecipientService>();
            services.AddScoped<SourceService>();
            services.AddScoped<TargetService>();

            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<IGroupTargetRepository, GroupTargetRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IRecipientRepository, RecipientRepository>();
            services.AddScoped<ISourceRepository, SourceRepository>();
            services.AddScoped<ITargetRepository, TargetRepository>();

            services.AddCors(options => options.UseConfiguredCors("default", Configuration.GetSection("CorsPolicy")));

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = AuthorizationOptions.Authority;
                    options.RequireHttpsMetadata = AuthorizationOptions.RequireHttpsMetadata;
                    options.ApiName = AuthorizationOptions.AuthorizationScope;
                });

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

            services.AddSignalR();

            services.AddSwagger(ApplicationName, "Foundry.Communications.xml", AuthorizationOptions);
        }

        /// <summary>
        /// configure
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors("default");

            app.UseSwagger(c =>
            {
                c.RouteTemplate = "api/{documentName}/openapi.json";
            });

            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "api";
                c.SwaggerEndpoint("/api/v1/openapi.json", ApplicationName + " (v1)");
                c.OAuthClientId(AuthorizationOptions.ClientId);
                c.OAuthClientSecret(AuthorizationOptions.ClientSecret);
                c.OAuthAppName(AuthorizationOptions.ClientName);
            });

            app.UseQueryStringBearer();
            app.UseAuthentication();

            app.UseSignalR(routes =>
            {
                routes.MapHub<NotificationHub>("/hubs/notifications");
            });

            app.UseMvc();
        }
    }
}
