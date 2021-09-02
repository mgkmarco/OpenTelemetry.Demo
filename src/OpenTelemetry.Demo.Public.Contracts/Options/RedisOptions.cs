namespace OpenTelemetry.Demo.Public.Contracts.Options
{
    public class RedisOptions
    {
        public static string RedisOptionsKey = "Redis";
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 6379;
    }
}