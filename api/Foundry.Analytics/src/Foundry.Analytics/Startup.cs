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
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Foundry.Analytics.Data;
using Foundry.Analytics.Data.Repositories;
using Foundry.Analytics.Identity;
using Foundry.Analytics.Options;
using Foundry.Analytics.Repositories;
using Foundry.Analytics.Services;
using Foundry.Analytics.xApi;
using Stack.Data;
using Stack.Data.Options;
using Stack.Http;
using Stack.Http.Formatters;
using Stack.Http.Identity;
using Stack.Http.Options;

namespace Foundry.Analytics
{
    /// <summary>
    /// analytics startup
    /// </summary>
    public class Startup
    {
        IConfiguration Configuration { get; }
        string ApplicationName { get; }
        AuthorizationOptions AuthorizationOptions { get; }

        /// <summary>
        /// constructor for Startup
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AuthorizationOptions = Configuration.GetSection("Authorization").Get<AuthorizationOptions>();
            ApplicationName = Configuration["Branding:ApplicationName"];
        }

        /// <summary>
        /// configure supporting services
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbProvider(Configuration);
            services.AddDbContextPool<AnalyticsDbContext>(builder => builder.UseConfiguredDatabase("Foundry.Analytics.Data", Configuration));

            services
                .AddMonitoredOptions<BrandingOptions>(Configuration, "Branding")
                .AddMonitoredOptions<DatabaseOptions>(Configuration, "Database")
                .AddMonitoredOptions<ErrorHandlingOptions>(Configuration, "ErrorHandling")
                .AddMonitoredOptions<LearningRecordStoreOptions>(Configuration, "Options:Lrs")
                .AddMonitoredOptions<IntegrationsOptions>(Configuration, "Options:Integrations")
                .AddMonitoredOptions<AuthorizationOptions>(Configuration, "Authorization");

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddAutoMapper();
            services.AddScoped<IStackIdentityResolver, SketchIdentityResolver>();

            services.AddScoped<IntegrationsService>();
            services.AddScoped<StatementService>();
            services.AddScoped<ClientEventService>();
            services.AddScoped<UserEventService>();
            services.AddScoped<ContentEventService>();

            services.AddScoped<IClientEventRepository, ClientEventRepository>();
            services.AddScoped<IUserEventRepository, UserEventRepository>();
            services.AddScoped<IContentEventRepository, ContentEventRepository>();

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

            services.AddSwagger(ApplicationName, "Foundry.Analytics.xml", AuthorizationOptions);
        }

        /// <summary>
        /// analytics configure
        /// </summary>
        /// <param name="app"></param>
        public void Configure(IApplicationBuilder app)
        {
            app.UseExceptionHandler(a => a.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature.Error;

                var result = JsonConvert.SerializeObject(new { error = exception.Message });
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(result);
            }));

            app.UsePathBase(Configuration["PathBase"]);

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
            app.UseMvc();
        }
    }
}
