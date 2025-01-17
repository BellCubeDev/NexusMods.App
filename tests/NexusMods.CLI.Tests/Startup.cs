using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NexusMods.Common;
using NexusMods.DataModel;
using NexusMods.FileExtractor;
using NexusMods.StandardGameLocators;
using NexusMods.StandardGameLocators.TestHelpers;

namespace NexusMods.CLI.Tests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddStandardGameLocators(false)
                .AddStubbedGameLocators()
                .AddDataModel()
                .AddFileExtractors()
                .AddCLI()
                .AddAllScoped<IRenderer, LoggingRenderer>()
                .AddLogging(builder => builder.SetMinimumLevel(LogLevel.Debug))
                .Validate();
    }
}

