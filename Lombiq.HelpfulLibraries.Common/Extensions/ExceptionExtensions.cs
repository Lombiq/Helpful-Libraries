using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;

namespace System;

public static class ExceptionExtensions
{
    /// <summary>
    /// Checks if the application can't recover from this type of exception.
    /// </summary>
    public static bool IsFatal(this Exception ex) => ex is OutOfMemoryException or SecurityException or SEHException;

    /// <summary>
    /// Aggregates the non-empty exception messages for <paramref name="exception"/> and all of its inner exceptions.
    /// </summary>
    public static ICollection<string> GetAllMessages(this Exception exception)
    {
        var messages = new List<string>();

        static void Iterate(ICollection<string> messages, Exception current)
        {
            if (!string.IsNullOrWhiteSpace(current.Message)) messages.Add(current.Message);

            switch (current.InnerException)
            {
                case AggregateException { InnerExceptions: { } innerExceptions }:
                    foreach (var innerItem in innerExceptions) Iterate(messages, innerItem);
                    return;
                case { } inner:
                    Iterate(messages, inner);
                    return;
                default:
                    return;
            }
        }

        Iterate(messages, exception);
        return messages;
    }
}
