using Conperio.AzureTableLogging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var azureStorageConnectionString = Environment.GetEnvironmentVariable("AzureStorageConnectionString");

if(string.IsNullOrEmpty(azureStorageConnectionString))
{
    throw new Exception("AzureStorageConnectionString environment variable not set");
}

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(builder =>
        builder.ClearProviders()

            .AddAzureTableLogger(configuration =>
            {
                configuration.ConnectionString = azureStorageConnectionString;
                configuration.Table = "TestLogs";
                configuration.LogLevel = LogLevel.Information;
            }))
            .Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();


logger.LogDebug(1, "Does this line get hit?");    // Not logged
logger.LogInformation(7, "Some information. {Date}", DateTime.Now);  // Logged with parameters
logger.LogError(7, "Oops, there was an error.");  // Logged
try
{
    var t1 = 1 - 1;
    var t = 100 / t1;
}
catch(Exception e)
{
    logger.LogError(e, "An error occured"); // Logged with stacktrace and exception message
}



await host.RunAsync();