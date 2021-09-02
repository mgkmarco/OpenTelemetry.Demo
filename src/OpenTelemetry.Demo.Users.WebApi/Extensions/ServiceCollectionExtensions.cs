using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Demo.Public.Contracts.Clients;
using OpenTelemetry.Demo.Public.Contracts.Options;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace OpenTelemetry.Demo.Users.WebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDbMigrations(this IServiceCollection services, IConfiguration configuration)
        {
            var datasourceOptions = new DatasourceOptions();
            configuration.GetSection(DatasourceOptions.DatasourceOptionsKey).Bind(datasourceOptions);
            services.Configure<DatasourceOptions>(configuration.GetSection(
                DatasourceOptions.DatasourceOptionsKey));

            var connection = new SqlConnection(datasourceOptions.ConnectionString);
            System.Diagnostics.Debug.WriteLine(connection);
            var evolve = new Evolve.Evolve(connection)
            {
                EmbeddedResourceAssemblies = new[] { typeof(Startup).Assembly },
                Locations = new []{ "Migrations" }
            };

            evolve.Migrate();
        }
        
        public static void AddHttpClients(this IServiceCollection services,  IConfiguration configuration)
        {
            var legislationsClientOptions = new LegislationsClientOptions();
            configuration.GetSection(LegislationsClientOptions.LegislationsOptionsKey).Bind(legislationsClientOptions);
            
            services.AddHttpClient(nameof(ILegislationsClient), httpClient =>
                {
                    httpClient.BaseAddress = new Uri(legislationsClientOptions.Endpoint);
                })
                .AddTypedClient(Refit.RestService.For<ILegislationsClient>);
        }
        
        public static void AddOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
        {
            var jaegerOptions = new JaegerOptions();
            configuration.GetSection(JaegerOptions.JaegerOptionsKey).Bind(jaegerOptions);
            
            services.AddOpenTelemetryTracing(builder => 
            {
                builder.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSqlClientInstrumentation(options => options.SetDbStatementForText = true)
                    .AddSource("UsersController")
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("OpenTelemetry.Demo.Users.WebApi"))
                    .AddJaegerExporter(opts =>
                    {
                        opts.AgentHost = jaegerOptions.AgentHost;
                        opts.AgentPort = jaegerOptions.AgentPort;
                    });
            });
        }
    }
}