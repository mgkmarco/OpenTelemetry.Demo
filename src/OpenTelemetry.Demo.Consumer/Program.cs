﻿using System;
using System.IO;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Demo.Consumer.BackgroundServices;
using OpenTelemetry.Demo.Public.Contracts.Options;
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

                            return new MetricServer(hostname: metricsOptions.Host, metricsOptions.Port);
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
                        });
                });
    }
}