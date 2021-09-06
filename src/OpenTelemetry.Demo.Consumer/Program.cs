using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Demo.Consumer.BackgroundServices;

namespace OpenTelemetry.Demo.Consumer
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            //setup our DI
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddHostedService<ConsumerService>()
                .BuildServiceProvider();

            return Task.CompletedTask;
        }
    }
}