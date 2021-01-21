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
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Foundry.Buckets.Cache;
using Foundry.Buckets.Data;
using Foundry.Buckets.Data.Repositories;
using Foundry.Buckets.Identity;
using Foundry.Buckets.Monitors;
using Foundry.Buckets.Options;
using Foundry.Buckets.Repositories;
using Foundry.Buckets.Security;
using Foundry.Buckets.Services;
using Foundry.Buckets.Storage;
using Stack.Data;
using Stack.Data.Options;
using Stack.Http;
using Stack.Http.Formatters;
using Stack.Http.Identity;
using Stack.Http.Options;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Foundry.Buckets
{
    /// <summary>
    /// start up configuration
    /// </summary>
    public class Startup
    {
        const string xmlFileName = "Foundry.Buckets.xml";
        internal IConfiguration Configuration { get; }
        internal string ApplicationName { get; }
        internal AuthorizationOptions AuthorizationOptions { get; }
        internal StorageOptions StorageOptions { get; }

        /// <summary>
        /// constructor for Startup
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AuthorizationOptions = Configuration.GetSection("Authorization").Get<AuthorizationOptions>();
            StorageOptions = Configuration.GetSection("Options:Storage").Get<StorageOptions>();
            ApplicationName = Configuration["Branding:ApplicationName"];
        }

        /// <summary>
        /// configure application services
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbProvider(Configuration);
            services.AddDbContextPool<BucketsDbContext>(builder => builder.UseConfiguredDatabase("Foundry.Buckets.Data", Configuration));

            services.AddMemoryCache((a) => { });

            services.AddOptions()
                .Configure<BrandingOptions>(Configuration.GetSection("Branding"))
                .AddScoped(config => config.GetService<IOptionsMonitor<BrandingOptions>>().CurrentValue)

                .Configure<DatabaseOptions>(Configuration.GetSection("Database"))
                .AddScoped(config => config.GetService<IOptionsMonitor<DatabaseOptions>>().CurrentValue)

                .Configure<ErrorHandlingOptions>(Configuration.GetSection("ErrorHandling"))
                .AddScoped(config => config.GetService<IOptionsMonitor<ErrorHandlingOptions>>().CurrentValue)

                .Configure<UrlsOptions>(Configuration.GetSection("Options:Urls"))
                .AddScoped(config => config.GetService<IOptionsMonitor<UrlsOptions>>().CurrentValue)

                .Configure<StorageOptions>(Configuration.GetSection("Options:Storage"))
                .AddScoped(config => config.GetService<IOptionsMonitor<StorageOptions>>().CurrentValue)

                .Configure<StorageLocalOptions>(Configuration.GetSection("Options:StorageLocal"))
                .AddScoped(config => config.GetService<IOptionsMonitor<StorageLocalOptions>>().CurrentValue)

                .Configure<StorageAzureOptions>(Configuration.GetSection("Options:StorageAzure"))
                .AddScoped(config => config.GetService<IOptionsMonitor<StorageAzureOptions>>().CurrentValue)

                .Configure<AuthorizationOptions>(Configuration.GetSection("Authorization"))
                .AddSingleton(config => config.GetService<IOptionsMonitor<AuthorizationOptions>>().CurrentValue);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddAutoMapper();

            services.AddSingleton<IFileUploadMonitor, FileUploadMonitor>();

            services.AddScoped<IStackIdentityResolver, SketchIdentityResolver>();

            //services.AddScoped<IClientIdentityHandler, ClientIdentityHandler>();
            //services.AddScoped<IProfileIdentityHandler, ProfileIdentityHandler>();
            //services.AddScoped<IClientAuthorization, ClientAuthorization>();
            //services.AddScoped<IProfileAuthorization, ProfileAuthorization>();
            //services.AddScoped<IIdentityResolver, ClaimsIdentityResolver>();

            switch (StorageOptions.StorageType)
            {
                case "Local":
                    services.AddScoped<IStorageProvider, StorageLocalProvider>();
                    break;
                default:
                    throw new NotImplementedException("'" + StorageOptions.StorageType + "' storage is not implemented");
            }

            services.AddScoped<BucketPermissionMediator>();

            services.AddScoped<BucketAccessRequestService>();
            services.AddScoped<BucketService>();
            services.AddScoped<BucketAccountService>();
            services.AddScoped<FileService>();
            services.AddScoped<AccountService>();
            services.AddScoped<TagService>();

            services.AddScoped<IBucketAccessRequestRepository, BucketAccessRequestRepository>();
            services.AddScoped<IBucketRepository, BucketRepository>();
            services.AddScoped<IBucketAccountRepository, BucketAccountRepository>();
            services.AddScoped<IFileRepository, FileRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ITagRepository, TagRepository>();

            services.AddScoped<IAccountCache, AccountCache>();

            services.Configure<SecurityHeaderOptions>(Configuration.GetSection("SecurityHeaders"))
                .AddScoped(svc => svc.GetService<IOptionsSnapshot<SecurityHeaderOptions>>().Value);

            services.AddCors(options => options.UseConfiguredCors("default", Configuration.GetSection("SecurityHeaders:Cors")));

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = AuthorizationOptions.Authority;
                    options.RequireHttpsMetadata = AuthorizationOptions.RequireHttpsMetadata;
                    options.ApiName = AuthorizationOptions.AuthorizationScope;
                });

            services.AddMvc()
                .AddJsonOptions(
                    options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                );

            services.AddMvcCore(options => options.InputFormatters.Insert(0, new TextMediaTypeFormatter()))
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                })
                .AddApiExplorer()
                .AddJsonFormatters();

            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = long.MaxValue;
            });

            services.AddSwagger(ApplicationName, AuthorizationOptions);
        }

        /// <summary>
        /// configure application
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment() || Configuration.GetValue("ErrorHandling:ShowDeveloperExceptions", false))
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UsePathBase(Configuration["PathBase"]);

            app.UseCors("default");

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

            app.UseQueryStringBearer();

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
