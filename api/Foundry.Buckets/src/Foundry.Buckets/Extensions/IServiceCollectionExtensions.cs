using Microsoft.Extensions.DependencyInjection;
using Stack.Http.Options;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Foundry.Buckets
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
    }
}
