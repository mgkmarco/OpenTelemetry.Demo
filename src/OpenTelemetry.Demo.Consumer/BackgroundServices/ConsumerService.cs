using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Demo.Consumer.Extensions;
using OpenTelemetry.Trace;
using Prometheus;

namespace OpenTelemetry.Demo.Consumer.BackgroundServices
{
    public class ConsumerService : IHostedService
    {
        private static readonly ActivitySource Activity = new(nameof(ConsumerService));
        private static readonly TextMapPropagator Propagator = new TraceContextPropagator();

        private readonly IMetricServer _metricServer;
        private readonly IAdvancedBus _bus;
        private readonly ILogger<ConsumerService> _logger;
        private readonly TracerProvider _tracerProvider;

        public ConsumerService([NotNull] IMetricServer metricServer, [NotNull] TracerProvider tracerProvider, [NotNull] IAdvancedBus bus,
            [NotNull] ILogger<ConsumerService> logger)
        {
            _metricServer = metricServer;
            _tracerProvider = tracerProvider;
            _bus = bus;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var queue = new Queue("test.trd.mgk");
            _bus.Consume(queue, (body, properties, info) =>
            {
                return Task.Factory.StartNew(() =>
                {
                    try
                    {
                        using (var activity = Activity.StartActivity("nameof(_bus.Consume)", ActivityKind.Consumer,
                            Propagator.ExtractActivityContextFromParentContext(properties, _logger),
                            new Dictionary<string, object>
                            {
                                {"messaging.system", "rabbitmq"},
                                {"messaging.rabbitmq.queue", queue.Name}
                            }))
                        {
                            var message = Encoding.UTF8.GetString(body);
                            _logger.LogInformation(
                                $"TraceParentID: {activity?.TraceId}, Message Consumed: \n{message}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message, ex);
                    }
                }, cancellationToken);
            });

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _metricServer.StopAsync();
            _tracerProvider.Shutdown();
        }
    }
}