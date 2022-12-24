using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

namespace Conperio.AzureTableLogging;

public static class AzureTableLoggerExtensions
{
    public static ILoggingBuilder AddAzureTableLogger(
        this ILoggingBuilder builder)
    {
        builder.AddConfiguration();

        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider, AzureTableLoggerProvider>());

        LoggerProviderOptions.RegisterProviderOptions
            <AzureTableLoggerConfiguration, AzureTableLoggerProvider>(builder.Services);

        return builder;
    }

    public static ILoggingBuilder AddAzureTableLogger(
        this ILoggingBuilder builder,
        Action<AzureTableLoggerConfiguration> configure)
    {
        builder.AddAzureTableLogger();
        builder.Services.Configure(configure);

        return builder;
    }
}
