using EasyNetQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Demo.Public.Contracts.Options;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;

namespace OpenTelemetry.Demo.Legislations.WebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMessageBus(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitOptions = new RabbitOptions();
            configuration.GetSection(RabbitOptions.RabbitOptionsKey).Bind(rabbitOptions);
            var connectionString = "host=";

            for (int i = 0; i < rabbitOptions.Hosts.Count; i++)
            {
                if (i == 0)
                {
                    connectionString += $"{rabbitOptions.Hosts[i]}";
                    break;
                }

                connectionString += $",{rabbitOptions.Hosts[i]}";
            }
            
            services.AddSingleton(RabbitHutch.CreateBus(connectionString).Advanced);
        }
        
        public static void AddOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
        {
            var jaegerOptions = new JaegerOptions();
            configuration.GetSection(JaegerOptions.JaegerOptionsKey).Bind(jaegerOptions);

            var redisOptions = new RedisOptions();
            configuration.GetSection(RedisOptions.RedisOptionsKey).Bind(redisOptions);

            services.AddOpenTelemetryTracing(builder =>
            {
                var connectionMux = ConnectionMultiplexer.Connect($"{redisOptions.Host}:{redisOptions.Port}");
                services.AddSingleton(connectionMux.GetDatabase());
                    
                builder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRedisInstrumentation(connectionMux)
                    .AddSource("LegislationsController")
                    .SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddService("OpenTelemetry.Demo.Legislations.WebApi"))
                    .AddJaegerExporter(opts =>
                    {
                        opts.AgentHost = jaegerOptions.AgentHost;
                        opts.AgentPort = jaegerOptions.AgentPort;
                    });
            });
        }
    }
}