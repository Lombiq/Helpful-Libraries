using System.Runtime.InteropServices;
using System.Security;

namespace System;

public static class ExceptionExtensions
{
    /// <summary>
    /// Checks if the application can't recover from this type of exception.
    /// </summary>
    public static bool IsFatal(this Exception ex) => ex is OutOfMemoryException or SecurityException or SEHException;
}
