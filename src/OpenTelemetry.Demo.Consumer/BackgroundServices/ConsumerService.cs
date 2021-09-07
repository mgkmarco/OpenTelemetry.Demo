using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Context.Propagation;
using Prometheus;

namespace OpenTelemetry.Demo.Consumer.BackgroundServices
{
    public class ConsumerService : BackgroundService
    {
        private static readonly ActivitySource Activity = new(nameof(ConsumerService));
        private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;
        private readonly MetricServer _metricServer;
        private readonly IAdvancedBus _bus;

        public ConsumerService([NotNull] MetricServer metricServer, [NotNull] IAdvancedBus bus)
        {
            _metricServer = metricServer;
            _metricServer.Start();
            _bus = bus;
        }
        
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                
            }
        }
    }
}