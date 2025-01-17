using FluentAssertions;
using NexusMods.DataModel;
using NexusMods.Paths;
using NexusMods.Paths.Extensions;
using NexusMods.Paths.Utilities;

namespace NexusMods.Games.BethesdaGameStudios.Tests;

[Trait("RequiresGameInstalls", "True")] // Technically this doesn't require the game, but the DI system does for the other tests
public class FileAnalysisTests
{

    private readonly FileContentsCache _cache;
    private readonly AbsolutePath _plugin1;

    // TODO: Write tests using _plugin2.
    // ReSharper disable once NotAccessedField.Local
    private readonly AbsolutePath _plugin2;

    public FileAnalysisTests(FileContentsCache cache)
    {
        _cache = cache;
        _plugin1 = KnownFolders.EntryFolder.CombineUnchecked("Resources").CombineUnchecked("testfile1.esp");
        _plugin2 = KnownFolders.EntryFolder.CombineUnchecked("Resources").CombineUnchecked("testfile2.esl");
    }

    [Fact]
    public async Task LoadsMetadataForPlugins()
    {
        var result = await _cache.AnalyzeFileAsync(_plugin1);

        result.AnalysisData.Should().ContainEquivalentOf(new PluginAnalysisData
        {
            IsLightMaster = true,
            Masters = new[] { "Skyrim.esm".ToRelativePath() },
        });
    }
}
