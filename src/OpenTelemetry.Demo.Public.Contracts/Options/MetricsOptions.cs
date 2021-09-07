namespace OpenTelemetry.Demo.Public.Contracts.Options
{
    public class MetricsOptions
    {
        public const string MetricsOptionsKey = "Metrics";
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 9090;
    }
}