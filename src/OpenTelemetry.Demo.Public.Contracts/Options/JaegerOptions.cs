namespace OpenTelemetry.Demo.Public.Contracts.Options
{
    public class JaegerOptions
    {
        public const string JaegerOptionsKey = "JaegerExporter";
        public string AgentHost { get; set; }
        public int AgentPort { get; set; }
    }
}