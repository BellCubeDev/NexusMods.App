using NexusMods.Paths;
using NexusMods.Paths.Utilities;

namespace NexusMods.Games.RedEngine.ModInstallers;

public static class Helpers
{
    public static Extension[] IgnoreExtensions = {
        KnownExtensions.Txt,
        KnownExtensions.Md,
        KnownExtensions.Pdf,
        KnownExtensions.Png
    };

}
