using NexusMods.Common;
using NexusMods.DataModel.Abstractions;
using NexusMods.DataModel.ArchiveContents;
using NexusMods.DataModel.Games;
using NexusMods.DataModel.Loadouts;
using NexusMods.DataModel.Loadouts.ModFiles;
using NexusMods.DataModel.ModInstallers;
using NexusMods.Hashing.xxHash64;
using NexusMods.Paths;

namespace NexusMods.StandardGameLocators.TestHelpers.StubbedGames;

public class StubbedGameInstaller : IModInstaller
{
    // ReSharper disable once NotAccessedField.Local
    private readonly IDataStore _store;

    public StubbedGameInstaller(IDataStore store)
    {
        _store = store;
    }

    public Priority Priority(GameInstallation installation, EntityDictionary<RelativePath, AnalyzedFile> files)
    {
        return installation.Game is StubbedGame ? Common.Priority.Normal : Common.Priority.None;
    }

    public Task<IEnumerable<AModFile>> Install(GameInstallation installation, Hash srcArchive, EntityDictionary<RelativePath, AnalyzedFile> files, CancellationToken cancel)
    {
        return Task.FromResult(InstallSync(installation, srcArchive, files));
    }
    private IEnumerable<AModFile> InstallSync(GameInstallation installation, Hash srcArchive, EntityDictionary<RelativePath, AnalyzedFile> files)    {
        foreach (var (key, value) in files)
        {
            yield return new FromArchive
            {
                Id = ModFileId.New(),
                From = new HashRelativePath(srcArchive, key),
                To = new GamePath(GameFolderType.Game, key),
                Hash = value.Hash,
                Size = value.Size
            };
        }
    }
}
