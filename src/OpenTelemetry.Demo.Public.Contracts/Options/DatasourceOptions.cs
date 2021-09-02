namespace OpenTelemetry.Demo.Public.Contracts.Options
{
    public class DatasourceOptions
    {
        public const string DatasourceOptionsKey = "Datasource";
        public string ConnectionString { get; set; }
    }
}