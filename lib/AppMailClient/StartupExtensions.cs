using System;
using AppMailClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddAppMailClient(this IServiceCollection services, Action<AppMailClient.Options> config)
        {
            var options = new AppMailClient.Options();

            if (config != null)
            {
                config.Invoke(options);
            }

            services.AddSingleton<AppMailClient.Options>(options);

            Add(services, options);

            return services;
        }

        public static IServiceCollection AddAppMailClient(this IServiceCollection services, Func<AppMailClient.Options> config)
        {
            var options =  (config != null)
                ? config()
                : new AppMailClient.Options();

            services.AddSingleton<AppMailClient.Options>(options);

            Add(services, options);

            return services;
        }

        public static IServiceCollection AddAppMailClient(this IServiceCollection services, Func<IConfigurationSection> config)
        {
            var section = config();
            var options = section.Get<AppMailClient.Options>();

            services.AddOptions()
            .Configure<AppMailClient.Options>(opt => config())
            .AddScoped<AppMailClient.Options>(sp => sp.GetService<IOptionsMonitor<AppMailClient.Options>>().CurrentValue);

            Add(services, options);

            return services;
        }

        private static IServiceCollection Add(IServiceCollection services, AppMailClient.Options options)
        {
            if (String.IsNullOrEmpty(options?.Url))
            {
                services.AddScoped<IAppMailClient, MailerMock>();
            }
            else
            {
                services.AddScoped<IAppMailClient, Mailer>();

                services.AddHttpClient<IAppMailClient, Mailer>()
                    .ConfigureHttpClient(client => {
                        client.BaseAddress = new System.Uri(options.Url);
                        client.DefaultRequestHeaders.Add(options.KeyHeader, options.Key);
                    })
                    .AddPolicyHandler(
                        HttpPolicyExtensions.HandleTransientHttpError()
                        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,retryAttempt)))
                    );
            }
            return services;
        }
    }
}
