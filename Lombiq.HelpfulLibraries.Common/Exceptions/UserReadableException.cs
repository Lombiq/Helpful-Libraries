using System;
using System.Collections.Generic;
using System.Linq;

namespace Lombiq.HelpfulLibraries.Common.Exceptions;

/// <summary>
/// An exception whose message is safe to display to the end user of a desktop or command line application.
/// </summary>
/// <remarks><para>
/// In case of web application, use <c>Lombiq.HelpfulLibraries.AspNetCore</c>'s <c>FrontendException</c> instead.
/// </para></remarks>
public class UserReadableException : Exception
{
    /// <summary>
    /// Gets the list of error messages that can be displayed to the user.
    /// </summary>
    public IReadOnlyList<string> Messages { get; } = [];

    public UserReadableException(ICollection<string> messages, Exception? innerException = null)
        : base(string.Join(Environment.NewLine, messages), innerException) =>
        Messages = [.. messages];

    public UserReadableException(string message)
        : this([message])
    {
    }

    public UserReadableException()
    {
    }

    public UserReadableException(string message, Exception? innerException)
        : this([message], innerException)
    {
    }

    /// <summary>
    /// If the provided collection of <paramref name="errors"/> is not empty, it throws an exception with the included
    /// texts.
    /// </summary>
    /// <param name="errors">The possible collection of error texts.</param>
    /// <exception cref="UserReadableException">The non-empty error messages from <paramref name="errors"/>.</exception>
    public static void ThrowIfAny(ICollection<string>? errors)
    {
        errors = errors?.WhereNot(string.IsNullOrWhiteSpace).ToList();

        if (errors == null || errors.Count == 0) return;
        if (errors.Count == 1) throw new UserReadableException(errors.Single());

        throw new UserReadableException(errors);
    }
}
