using Microsoft.Extensions.Logging;

namespace Conperio.AzureTableLogging;

public class AzureTableLoggerConfiguration
{
    public AzureTableLoggerConfiguration() { }
    public AzureTableLoggerConfiguration(string connectionString, string table, LogLevel logLevel)
    {
        ConnectionString = connectionString;
        Table = table;
        LogLevel = logLevel;
    }

    public string ConnectionString { get; set; } = string.Empty;
    public string Table { get; set; } = string.Empty;
    public LogLevel LogLevel { get; set; }
}
