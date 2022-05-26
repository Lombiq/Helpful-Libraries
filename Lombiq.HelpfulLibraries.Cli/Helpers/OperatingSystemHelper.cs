using System;

namespace Lombiq.HelpfulLibraries.Cli.Helpers;

/// <summary>
/// Functions that have different implementations/results based on the current operating system.
/// </summary>
public static class OperatingSystemHelper
{
    /// <summary>
    /// Returns the file extension of an executable. Executable files end with <c>.exe</c> on Windows but with nothing
    /// on Linux and macOS.
    /// </summary>
    public static string GetExecutableExtension() => OperatingSystem.IsWindows() ? ".exe" : string.Empty;
}
