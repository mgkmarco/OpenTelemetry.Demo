using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace OpenTelemetry.Demo.Consumer.BackgroundServices
{
    public class ConsumerService : BackgroundService
    {
        public ConsumerService()
        {
            
        }
        
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new System.NotImplementedException();
        }
    }
}