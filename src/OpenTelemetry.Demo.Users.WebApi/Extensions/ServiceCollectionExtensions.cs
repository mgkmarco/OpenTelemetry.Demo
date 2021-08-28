using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Demo.Public.Contracts.Clients;
using OpenTelemetry.Demo.Public.Contracts.Options;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace OpenTelemetry.Demo.Users.WebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
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
        
        public static void AddOpenTracing(this IServiceCollection services, IConfiguration configuration)
        {
            var jaegerOptions = new JaegerOptions();
            configuration.GetSection(JaegerOptions.JaegerOptionsKey).Bind(jaegerOptions);
            
            services.AddOpenTelemetryTracing(builder => 
            {
                builder.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
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