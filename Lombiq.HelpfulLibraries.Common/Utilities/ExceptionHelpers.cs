using System;

namespace Lombiq.HelpfulLibraries.Common.Utilities;

public static class ExceptionHelpers
{
    public static void ThrowIfNull(object value, string paramName, string message = null)
    {
        if (value == default) throw new ArgumentNullException(paramName, message);
    }

    public static void ThrowIfIsNullOrEmpty(string value, string paramName, string message = null)
    {
        if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(paramName, message);
    }
}
