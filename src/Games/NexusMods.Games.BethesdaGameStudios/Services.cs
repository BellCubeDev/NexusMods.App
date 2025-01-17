using Microsoft.Extensions.DependencyInjection;
using NexusMods.Common;
using NexusMods.DataModel.Abstractions;
using NexusMods.DataModel.Games;
using NexusMods.DataModel.JsonConverters.ExpressionGenerator;
using NexusMods.DataModel.ModInstallers;
using NexusMods.Games.BethesdaGameStudios.Installers;

namespace NexusMods.Games.BethesdaGameStudios;

public static class Services
{
    public static IServiceCollection AddBethesdaGameStudios(this IServiceCollection services)
    {
        services.AddAllSingleton<IModInstaller, LooseFileInstaller>();
        services.AddAllSingleton<IGame, SkyrimSpecialEdition>();
        services.AddAllSingleton<IFileAnalyzer, PluginAnalyzer>();
        services.AddAllSingleton<ITypeFinder, TypeFinder>();
        services.AddAllSingleton<IFileMetadataSource, AnalysisMetaDataSource>();
        return services;
    }

}
