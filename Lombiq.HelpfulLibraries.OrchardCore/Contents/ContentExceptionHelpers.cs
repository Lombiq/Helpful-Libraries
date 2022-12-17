using OrchardCore.ContentManagement;
using System;

namespace Lombiq.HelpfulLibraries.OrchardCore.Contents;

public static class ContentExceptionHelpers
{
    /// <summary>
    /// Throws <see cref="ArgumentException"/> if the given <paramref name="content"/>'s <c>ContentType</c> does not
    /// match <paramref name="expectedType"/>.
    /// </summary>
    /// <param name="content">The content whose <c>ContentType</c> is to be checked.</param>
    /// <param name="expectedType">The expected content type.</param>
    /// <param name="paramName">The name of the parameter to be shown in the exception.</param>
    public static void ThrowIfNotValidContentType(IContent content, string expectedType, string paramName)
    {
        if (content.ContentItem.ContentType != expectedType)
        {
#pragma warning disable S2302 // "nameof" should be used
            throw new ArgumentException($"Parameter {paramName} is not of the {expectedType} content type.", paramName);
#pragma warning restore S2302 // "nameof" should be used
        }
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> if the given <paramref name="content"/> has no element of the specified
    /// <typeparamref name="TElement"/> type.
    /// </summary>
    /// <param name="content">The content whose elements are to be checked.</param>
    /// <param name="paramName">The name of the parameter to be shown in the exception.</param>
    public static void ThrowIfHasNoElement<TElement>(IContent content, string paramName)
        where TElement : ContentElement
    {
        if (!content.ContentItem.Has<TElement>())
        {
            throw new ArgumentException($"Parameter {paramName} has no {typeof(TElement).Name} attached to it.", paramName);
        }
    }
}
