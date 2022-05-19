using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Demo.Public.Contracts.Clients;
using OpenTelemetry.Demo.Public.Contracts.Options;
using Refit;
using SB.Community.Hive.Contracts;
using System;
using System.IO;

namespace OpenTelemetry.Demo.Hive.Users.WebApi
{
    public class CommunityServiceProvider : ICommunityServiceProvider
    {
        public IServiceProvider CreateServiceProvider(IServiceCollection services)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

            services.AddHttpClient(nameof(ILegislationsClient),  (provider, httpClient) =>
            {
                var legislationsClientOptions = new LegislationsClientOptions();
                configuration.GetSection(LegislationsClientOptions.LegislationsOptionsKey).Bind(legislationsClientOptions);

                httpClient.BaseAddress = new Uri(legislationsClientOptions.Endpoint);
            })
                .AddTypedClient(RestService.For<ILegislationsClient>);

            var datasourceOptions = new DatasourceOptions();
            configuration.GetSection(DatasourceOptions.DatasourceOptionsKey).Bind(datasourceOptions);
            services.Configure<DatasourceOptions>(configuration.GetSection(DatasourceOptions.DatasourceOptionsKey));

            var provider = services.BuildServiceProvider();

            return provider;
        }
    }
}
