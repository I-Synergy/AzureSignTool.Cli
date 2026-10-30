using AzureSignTool.Core.Enumerations;

namespace AzureSignTool.Core.Factories;

internal static class SipExtensionFactory
{
    private static readonly HashSet<string> AppxExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".appx",
        ".eappx",
        ".appxbundle",
        ".eappxbundle",
        ".msix",
        ".emsix",
        ".msixbundle",
        ".emsixbundle"
    };

    private static readonly HashSet<string> NuGetExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".nupkg",
        ".snupkg"
    };

    public static SipKind GetSipKind(ReadOnlySpan<char> filePath)
    {
        var extension = Path.GetExtension(filePath).ToString();

        if (AppxExtensions.Contains(extension))
        {
            return SipKind.Appx;
        }

        if (NuGetExtensions.Contains(extension))
        {
            return SipKind.NuGet;
        }

        return SipKind.None;
    }
}
