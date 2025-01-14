using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Reactive;
using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NexusMods.App.UI;
using NexusMods.CLI;
using NexusMods.Common;
using NexusMods.FileExtractor.Extractors;
using NexusMods.FOMOD;
using NexusMods.DataModel;
using NexusMods.FileExtractor;
using NexusMods.Games.BethesdaGameStudios;
using NexusMods.Games.DarkestDungeon;
using NexusMods.Games.Generic;
using NexusMods.Games.RedEngine;
using NexusMods.Games.Reshade;
using NexusMods.Games.TestHarness;
using NexusMods.Networking.HttpDownloader;
using NexusMods.Networking.NexusWebApi;
using NexusMods.StandardGameLocators;
using NLog.Extensions.Logging;
using NLog.Targets;
using ReactiveUI;

namespace NexusMods.App;

public class Program
{
    private static ILogger<Program> _logger = default!;

    [STAThread]
    public static async Task<int> Main(string[] args)
    {
        var host = BuildHost();

        _logger = host.Services.GetRequiredService<ILogger<Program>>();
        TaskScheduler.UnobservedTaskException += (_, e) =>
        {
            _logger.LogError(e.Exception, "Unobserved task exception");
            e.SetObserved();
        };

        RxApp.DefaultExceptionHandler = Observer.Create<Exception>(ex =>
        {
            _logger.LogError(ex, "Unhandled exception");
        });

        if (args.Length > 0)
        {
            var service = host.Services.GetRequiredService<CommandLineConfigurator>();
            var root = service.MakeRoot();

            var builder = new CommandLineBuilder(root)
                .UseDefaults()
                .Build();


            var extr = host.Services.GetRequiredService<IExtractor>();
            var extr2 = host.Services.GetService<IExtractor>();
            var extrList = host.Services.GetServices<IExtractor>();

            return await builder.InvokeAsync(args);
        }

        Startup.Main(host.Services, args);
        return 0;
    }

    private static IHost BuildHost()
    {
        var host = Host.CreateDefaultBuilder(Environment.GetCommandLineArgs())
            .ConfigureLogging(AddLogging)
            .ConfigureServices((_, services) =>
                services.AddApp()
                    .Validate())
            .Build();
        return host;
    }


    static void AddLogging(ILoggingBuilder loggingBuilder)
    {
        var config = new NLog.Config.LoggingConfiguration();

        var fileTarget = new FileTarget("file")
        {
            FileName = @"c:\tmp\logs\nexusmods.app.current.log",
            ArchiveFileName = @"c:\tmp\logs\nexusmods.app.{##}.log",
            ArchiveOldFileOnStartup = true,
            MaxArchiveFiles = 10,
            Layout = "${processtime} [${level:uppercase=true}] (${logger}) ${message:withexception=true}",
            Header = "############ Nexus Mods App log file - ${longdate} ############"
        };

        var consoleTarget = new ConsoleTarget("console")
        {
            Layout = "${processtime} [${level:uppercase=true}] ${message:withexception=true}",
        };

        config.AddRuleForAllLevels(fileTarget);
        config.AddRuleForAllLevels(consoleTarget);

        loggingBuilder.ClearProviders();
        loggingBuilder.SetMinimumLevel(LogLevel.Information);
        loggingBuilder.AddNLog(config);
    }

    /// <summary>
    /// Don't Delete this method. It's used by the Avalonia Designer.
    /// </summary>
    // ReSharper disable once UnusedMember.Local
    private static AppBuilder BuildAvaloniaApp()
    {
        var host = BuildHost();
        return Startup.BuildAvaloniaApp(host.Services);
    }
}
