using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using System;
using System.Text;

namespace Conperio.AzureTableLogging;

public partial class AzureTableLogger : ILogger
{
    private readonly TableClient _tableClient;
    private readonly string _name;
    private readonly Func<AzureTableLoggerConfiguration> _getCurrentConfig;
    private bool _initialized = false;


    public AzureTableLogger(string name, Func<AzureTableLoggerConfiguration> config)
    {
        _name = name;
        _getCurrentConfig = config;
        
        _tableClient = new TableClient(_getCurrentConfig().ConnectionString, _getCurrentConfig().Table);
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;

    public bool IsEnabled(LogLevel logLevel)
    {
        return _getCurrentConfig().LogLevel <= logLevel;
    }

    private void Initialize()
    {
        _tableClient.CreateIfNotExists();
        _initialized = true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        if (!_initialized)
        {
            Initialize();
        }

        var log = new TableLogEntry
        {
            PartitionKey = DateTime.Now.ToString("yyyyMMdd"),
            RowKey = Guid.NewGuid().ToString(),
            EventId = eventId.Id,
            LogLevel = (int)logLevel,
            Name = _name,
            Message = FormatMessage(state, exception, formatter)
        };

        _tableClient.AddEntity(log);
    }

    private static string FormatMessage<TState>(TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        // The default formatter only formats the state.
        var sb = new StringBuilder();
        sb.Append(formatter(state, exception));
        if(exception != null)
        {
            sb.Append($"\nException: {exception.Message}\nStacktrace: {exception.StackTrace}");
        }
        return  sb.ToString();
    }
}
