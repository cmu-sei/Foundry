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
using Stack.Http.Options;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Foundry.Orders
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services, string applicationName, AuthorizationOptions authorizationOptions)
        {
            services.AddSwaggerGen(c =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }

                c.SwaggerDoc("v1", new Info
                {
                    Title = applicationName,
                    Version = "v1",
                    Description = "API documentation and interaction for " + applicationName
                });

                c.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Type = "oauth2",
                    Flow = "implicit",
                    AuthorizationUrl = authorizationOptions.AuthorizationUrl,
                    Scopes = new Dictionary<string, string>
                    {
                        { authorizationOptions.AuthorizationScope, "public api access" }
                    }
                });

                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "oauth2", new[] { authorizationOptions.AuthorizationScope } }
                });

                c.DescribeAllEnumsAsStrings();
                c.CustomSchemaIds(x =>
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

        public static IServiceCollection AddDbProvider(this IServiceCollection services, IConfiguration config)
        {
            string dbProvider = config.GetValue<string>("Database:Provider", "Sqlite").Trim();
            switch (dbProvider)
            {
                case "Sqlite":
                    services.AddEntityFrameworkSqlite();
                    break;
                case "SqlServer":
                    services.AddEntityFrameworkSqlServer();
                    break;
                case "PostgreSQL":
                    services.AddEntityFrameworkNpgsql();
                    break;
            }
            return services;
        }
    }
}
