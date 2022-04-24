using OrchardCore.ContentManagement;
using System;

namespace Lombiq.HelpfulLibraries.OrchardCore.Contents;

public static class ContentExceptionHelpers
{
    public static void ThrowIfNotValidContentType(IContent content, string expectedType, string paramName)
    {
        if (content.ContentItem.ContentType != expectedType)
        {
#pragma warning disable S2302 // "nameof" should be used
            throw new ArgumentException($"Parameter {paramName} is not of the {expectedType} content type.", paramName);
#pragma warning restore S2302 // "nameof" should be used
        }
    }

    public static void ThrowIfHasNoElement<TElement>(IContent content, string paramName)
        where TElement : ContentElement
    {
        if (!content.ContentItem.Has<TElement>())
        {
            throw new ArgumentException($"Parameter {paramName} has no {typeof(TElement).Name} attached to it.", paramName);
        }
    }
}
