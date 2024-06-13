#nullable enable

using Microsoft.AspNetCore.Mvc.Localization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lombiq.HelpfulLibraries.AspNetCore.Exceptions;

/// <summary>
/// An exception whose message is safe to display to the end user of a web application.
/// </summary>
/// <remarks><para>Note that the message may contain unencoded HTML content.</para></remarks>
public class FrontendException : Exception
{
    /// <summary>
    /// The string placed between error messages in the <see cref="Exception.Message"/> property, if there are multiple
    /// entries in the <see cref="HtmlMessages"/>.
    /// </summary>
    public const string MessageSeparator = "<br>";

    /// <summary>
    /// Gets the list of error messages that can be displayed on the front end.
    /// </summary>
    public IReadOnlyList<LocalizedHtmlString> HtmlMessages { get; } = [];

    public FrontendException(LocalizedHtmlString message, Exception? innerException = null)
        : base(message.Value, innerException) =>
        HtmlMessages = [message];

    public FrontendException(ICollection<LocalizedHtmlString> messages, Exception? innerException = null)
        : base(string.Join("<br>", messages.Select(message => message.Value)), innerException) =>
        HtmlMessages = [.. messages];

    public FrontendException(string message)
        : base(message) =>
        HtmlMessages = [new LocalizedHtmlString(message, message)];

    public FrontendException()
    {
    }

    public FrontendException(string message, Exception? innerException)
        : base(message, innerException) =>
        HtmlMessages = [new LocalizedHtmlString(message, message)];

    /// <summary>
    /// If the provided collection of <paramref name="errors"/> is not empty, it throws an exception with the included
    /// texts.
    /// </summary>
    /// <param name="errors">The possible collection of error texts.</param>
    /// <exception cref="FrontendException">The non-empty error messages from <paramref name="errors"/>.</exception>
    public static void ThrowIfAny(ICollection<string>? errors) =>
        ThrowIfAny(errors?.WhereNot(string.IsNullOrWhiteSpace).Select(error => new LocalizedHtmlString(error, error)).ToList());

    /// <inheritdoc cref="ThrowIfAny(System.Collections.Generic.ICollection{string}?)"/>
    public static void ThrowIfAny(ICollection<LocalizedHtmlString>? errors)
    {
        if (errors == null || errors.Count == 0) return;

        if (errors.Count == 1) throw new FrontendException(errors.Single());

        throw new FrontendException(errors);
    }
}
