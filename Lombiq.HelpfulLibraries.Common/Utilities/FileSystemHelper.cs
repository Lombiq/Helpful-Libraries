using System;
using System.IO;

namespace Lombiq.HelpfulLibraries.Common.Utilities;

/// <summary>
/// Some shortcuts for file system operations.
/// </summary>
public static class FileSystemHelper
{
    /// <summary>
    /// Returns a composed file system path from the <paramref name="pathComponents"/> and creates it as a directory if
    /// it doesn't exist.
    /// </summary>
    public static string EnsureDirectoryExists(params string[] pathComponents)
    {
        var path = Path.Combine(pathComponents);
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        return path;
    }

    /// <summary>
    /// Returns <see langword="true"/> if all items in <paramref name="paths"/> exist according to <see
    /// cref="File.Exists"/>.
    /// </summary>
    public static bool AllExist(params string[] paths) => paths.TrueForAll(File.Exists);
}
