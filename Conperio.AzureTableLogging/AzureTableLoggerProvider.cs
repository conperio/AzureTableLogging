using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Runtime.Versioning;
using Microsoft.Extensions.Options;

namespace Conperio.AzureTableLogging;

[UnsupportedOSPlatform("browser")]
[ProviderAlias("AzureTable")]
public sealed class AzureTableLoggerProvider : ILoggerProvider
{
    private readonly IDisposable? _onChangeToken;
    private AzureTableLoggerConfiguration _currentConfig;
    private readonly ConcurrentDictionary<string, AzureTableLogger> _loggers =
        new(StringComparer.OrdinalIgnoreCase);

    public AzureTableLoggerProvider(
        IOptionsMonitor<AzureTableLoggerConfiguration> config)
    {
        _currentConfig = config.CurrentValue;
        _onChangeToken = config.OnChange(updatedConfig => _currentConfig = updatedConfig);
    }

    public ILogger CreateLogger(string categoryName) =>
        _loggers.GetOrAdd(categoryName, name => new AzureTableLogger(name, GetCurrentConfig));

    private AzureTableLoggerConfiguration GetCurrentConfig() => _currentConfig;

    public void Dispose()
    {
        _loggers.Clear();
        _onChangeToken?.Dispose();
    }
}