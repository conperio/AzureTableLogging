using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using System;

namespace Conperio.AzureTableLogging;

public partial class AzureTableLogger : ILogger
{
    private readonly TableClient _tableClient;
    private readonly string _name;
    private readonly Func<AzureTableLoggerConfiguration> _getCurrentConfig;


    public AzureTableLogger(string name, Func<AzureTableLoggerConfiguration> config)
    {
        _name = name;
        _getCurrentConfig = config;
        
        _tableClient = new TableClient(_getCurrentConfig().ConnectionString, _getCurrentConfig().Table);
        _tableClient.CreateIfNotExists();
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;

    public bool IsEnabled(LogLevel logLevel)
    {
        return _getCurrentConfig().LogLevel <= logLevel;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var log = new TableLogEntry
        {
            PartitionKey = DateTime.Now.ToString("yyyyMMdd"),
            RowKey = Guid.NewGuid().ToString(),
            EventId = eventId.Id,
            LogLevel = (int)logLevel,
            Message = MessageFormatter(state, exception)
        };

        _tableClient.AddEntity(log);
    }

    private static string MessageFormatter<TState>(TState state, Exception? exception)
    {
        return state?.ToString() + exception != null ? $"\nException: {exception?.Message}\nStacktrace: {exception?.StackTrace}" : string.Empty;
    }
}
