using System;

namespace Lombiq.HelpfulLibraries.Common.Utilities;

public static class ExceptionHelpers
{
    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if the provided <paramref name="value"/> object is
    /// <see langword="null"/>.
    /// </summary>
    public static void ThrowIfNull(object value, string paramName, string message = null)
    {
        if (value == default) throw new ArgumentNullException(paramName, message);
    }

    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if the provided <paramref name="value"/> string is
    /// <see langword="null"/> or empty.
    /// </summary>
    public static void ThrowIfIsNullOrEmpty(string value, string paramName, string message = null)
    {
        if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(paramName, message);
    }
}
