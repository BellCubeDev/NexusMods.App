using FluentAssertions;
using NexusMods.DataModel.Extensions;
using NexusMods.DataModel.Tests.Harness;
using NexusMods.Hashing.xxHash64;
using NexusMods.Paths.Utilities;

namespace NexusMods.DataModel.Tests;

public class FileHashCacheTests : ADataModelTest<FileHashCacheTests>
{
    public FileHashCacheTests(IServiceProvider provider) : base(provider)
    {
    }

    [Fact]
    public async Task CanGetHashOfSingleFile()
    {
        var file = KnownFolders.CurrentDirectory.CombineUnchecked(Guid.NewGuid().ToString()).AppendExtension(KnownExtensions.Tmp);
        await file.WriteAllTextAsync("Test data here");

        var hash = await FileHashCache.IndexFileAsync(file);
        hash.Hash.Should().Be((Hash)0xB08C91D1CDF11402);
        FileHashCache.TryGetCached(file, out var found).Should().BeTrue();
        found.Hash.Should().Be(hash.Hash);
        file.Delete();
    }

    [Fact]
    public async Task CanHashFolder()
    {
        var folder = KnownFolders.CurrentDirectory.CombineUnchecked("tempData");
        var file = folder.CombineUnchecked(Guid.NewGuid().ToString()).AppendExtension(KnownExtensions.Tmp);
        file.Parent.CreateDirectory();
        await file.WriteAllTextAsync("Test data here");

        var results = await FileHashCache.IndexFolderAsync(folder, CancellationToken.None).ToListAsync();
        results.Should().ContainSingle(x => x.Path == file);
    }


}
