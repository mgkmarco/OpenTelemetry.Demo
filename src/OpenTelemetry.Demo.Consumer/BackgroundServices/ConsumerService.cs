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
using Prometheus;

namespace OpenTelemetry.Demo.Consumer.BackgroundServices
{
    public class ConsumerService : IHostedService
    {
        private static readonly ActivitySource Activity = new(nameof(ConsumerService));
        private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;

        private readonly IMetricServer _metricServer;
        private readonly IAdvancedBus _bus;
        private readonly ILogger<ConsumerService> _logger;

        public ConsumerService([NotNull] IMetricServer metricServer, [NotNull] IAdvancedBus bus,
            [NotNull] ILogger<ConsumerService> logger)
        {
            _metricServer = metricServer;
            _bus = bus;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var queue = new Queue("test.trd.mgk");
            _bus.Consume(queue, (body, properties, info) => Task.Factory.StartNew(() =>
            {
                // using(var activity = Activity.StartActivity(nameof(_bus.Consume), ActivityKind.Consumer))
                // {
                    var message = Encoding.UTF8.GetString(body);
                    
                //     var tags = new Dictionary<string, object>
                //     {
                //         { "messaging.system", "rabbitmq" },
                //         { "messaging.rabbitmq.queue", queue.Name }
                //     };
                //     
                //     activity.ExtractActivityFromHeader(Propagator, tags, messageProperties, _logger);
                //     
                // }
            }, cancellationToken));

            return Task.CompletedTask;

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _metricServer.StopAsync();
        }
    }
}