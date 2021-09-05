using System.Collections.Generic;

namespace OpenTelemetry.Demo.Public.Contracts.Options
{
    public class RabbitOptions
    {
        public const string RabbitOptionsKey = "RabbitMessageBus";
        public List<string> Hosts { get; set; } = new List<string> {"localhost:5673"};
    }
}