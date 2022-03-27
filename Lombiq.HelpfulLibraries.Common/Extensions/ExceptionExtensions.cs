using System.Runtime.InteropServices;
using System.Security;

namespace System;

public static class ExceptionExtensions
{
    /// <summary>
    /// Checks if the exception is one that the application can't recover from.
    /// </summary>
    public static bool IsFatal(this Exception ex) => ex is OutOfMemoryException or SecurityException or SEHException;
}
