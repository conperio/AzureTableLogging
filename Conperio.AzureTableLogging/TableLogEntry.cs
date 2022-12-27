using Azure;
using Azure.Data.Tables;

namespace Conperio.AzureTableLogging;

public class TableLogEntry : ITableEntity
{
    public string PartitionKey { get; set; } = string.Empty;
    public string RowKey { get; set; } = string.Empty;
    public int EventId { get; set; }
    public int LogLevel { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTimeOffset? Timestamp { get; set; } = DateTimeOffset.Now;
    public ETag ETag { get; set; }

}
