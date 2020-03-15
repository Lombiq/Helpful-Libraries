﻿using OrchardCore.ContentManagement;
using System;

namespace Lombiq.HelpfulLibraries.Libraries.Utilities
{
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

        public static void ThrowIfNotValidContentType(IContent content, string expectedType, string paramName)
        {
            if (content.ContentItem.ContentType != expectedType)
            {
                throw new ArgumentException($"Parameter {paramName} is not a(n) {expectedType} content.", paramName);
            }
        }

        public static void ThrowIfHasNoElement<TElement>(IContent content, string paramName) where TElement : ContentElement
        {
            if (!content.ContentItem.Has<TElement>())
            {
                throw new ArgumentException($"Parameter {paramName} has no {typeof(TElement).Name} attached to it.", paramName);
            }
        }
    }
}
