/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Stack.Http.Options;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Foundry.Analytics
{
    /// <summary>
    /// service collection extensions
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// add monitored scoped option
        /// </summary>
        /// <typeparam name="TOptions"></typeparam>
        /// <param name="serviceCollection"></param>
        /// <param name="configuration"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IServiceCollection AddMonitoredOptions<TOptions>(this IServiceCollection serviceCollection, IConfiguration configuration, string key)
             where TOptions : class
        {
            var x = serviceCollection.Configure<TOptions>(configuration.GetSection(key))
                .AddScoped(config => config.GetService<IOptionsMonitor<TOptions>>().CurrentValue);

            return x;
        }

        /// <summary>
        /// add swagger
        /// </summary>
        /// <param name="services"></param>
        /// <param name="applicationName"></param>
        /// <param name="xmlFileName"></param>
        /// <param name="authorizationOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddSwagger(this IServiceCollection services, string applicationName, string xmlFileName, AuthorizationOptions authorizationOptions)
        {
            services.AddSwaggerGen(options =>
            {
                if (File.Exists(xmlFileName))
                {
                    options.IncludeXmlComments(xmlFileName);
                }

                options.SwaggerDoc("v1", new Info
                {
                    Title = applicationName,
                    Version = "v1",
                    Description = "API documentation and interaction for " + applicationName
                });

                options.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Type = "oauth2",
                    Flow = "implicit",
                    AuthorizationUrl = authorizationOptions.AuthorizationUrl,
                    Scopes = new Dictionary<string, string>
                    {
                        { authorizationOptions.AuthorizationScope, "public api access" }
                    }
                });
                options.DescribeAllEnumsAsStrings();
                options.CustomSchemaIds(x =>
                {

                    string ns = (!x.Namespace.StartsWith("Stack"))
                        ? x.Namespace + "."
                        : "";

                    string n = (x.IsGenericType)
                        ? x.Name.Substring(0, x.Name.Length - 2) + x.GenericTypeArguments.Last().Name.Split('.').Last()
                        : x.Name;

                    return ns + n;
                });
            });

            return services;
        }
    }
}

