using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Demo.Consumer.BackgroundServices;
using OpenTelemetry.Demo.Public.Contracts.Options;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;

namespace OpenTelemetry.Demo.Consumer
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();

            return Task.CompletedTask;
        }
        
        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(builder =>
                {
                    builder.AddJsonFile("appsettings.json", true);
                    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                    if (environment == "Development")
                    {
                        builder
                            .AddJsonFile(
                                Path.Combine(AppContext.BaseDirectory,
                                    string.Format("..{0}..{0}..{0}", Path.DirectorySeparatorChar),
                                    $"appsettings.{environment}.json"),
                                optional: true
                            );
                    }
                })
                .ConfigureServices(services =>
                {
                    services.AddLogging()
                        .AddHostedService<ConsumerService>()
                        .AddSingleton(provider =>
                        {
                            var configuration = provider.GetRequiredService<IConfiguration>();
                            var metricsOptions = new MetricsOptions();
                            configuration.GetSection(MetricsOptions.MetricsOptionsKey).Bind(metricsOptions);
                            
                            var metricServer = new MetricServer(hostname: metricsOptions.Host, metricsOptions.Port);

                            return metricServer.Start();
                        })
                        .AddSingleton(provider =>
                        {
                            var configuration = provider.GetRequiredService<IConfiguration>();
                            var rabbitOptions = new RabbitOptions();
                            configuration.GetSection(RabbitOptions.RabbitOptionsKey).Bind(rabbitOptions);
                            var connectionString = $"host={rabbitOptions.Hosts}";

                            return RabbitHutch
                                .CreateBus(
                                    $"{connectionString};virtualHost={rabbitOptions.VirtualHost};username={rabbitOptions.Username};password={rabbitOptions.Password}")
                                .Advanced;
                        })
                        .AddSingleton(provider =>
                        {
                            var configuration = provider.GetRequiredService<IConfiguration>();
                            var jaegerOptions = new JaegerOptions();
                            configuration.GetSection(JaegerOptions.JaegerOptionsKey).Bind(jaegerOptions);

                            var traceProvider = Sdk.CreateTracerProviderBuilder()
                                .AddSource(nameof(ConsumerService))
                                .SetResourceBuilder(ResourceBuilder.CreateDefault()
                                    .AddService("OpenTelemetry.Demo.Consumer." + Guid.NewGuid().ToString()))
                                .AddJaegerExporter(opts =>
                                {
                                    opts.AgentHost = jaegerOptions.AgentHost;
                                    opts.AgentPort = jaegerOptions.AgentPort;
                                    opts.ExportProcessorType = ExportProcessorType.Simple;
                                }).Build();

                            return traceProvider;
                        });
                });
    }
}