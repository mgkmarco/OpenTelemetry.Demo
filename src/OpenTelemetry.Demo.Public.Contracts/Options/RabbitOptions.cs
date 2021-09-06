using System.Collections.Generic;

namespace OpenTelemetry.Demo.Public.Contracts.Options
{
    public class RabbitOptions
    {
        public const string RabbitOptionsKey = "RabbitMessageBus";
        public string Hosts { get; set; } = "localhost:5672";
        public string VirtualHost { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}