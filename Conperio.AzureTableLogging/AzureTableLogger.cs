﻿using Azure.Data.Tables;
using Microsoft.Extensions.Logging;

namespace Conperio.AzureTableLogging;

public class AzureTableLogger : ILogger
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

        var log = new TableEntity
        {
            PartitionKey = DateTime.Now.ToString("yyyyMMdd"),
            RowKey = Guid.NewGuid().ToString()
        };
        log.Add("EventId", eventId.Id);
        log.Add("LogLevel", (int)logLevel);
        log.Add("Message", formatter(state, exception));

        _tableClient.AddEntity(log);
    }
}
