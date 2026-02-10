using Lombiq.HelpfulLibraries.OrchardCore.Contents;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement.Records;
using OrchardCore.ResourceManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YesSql;
using ISession = YesSql.ISession;

namespace Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement;

public class ResourceFilterBuilder
{
    public IList<ResourceFilter> ResourceFilters { get; private set; } = [];

    /// <summary>
    /// Adds the provided <paramref name="filter"/> to the list of <see cref="ResourceFilters"/>.
    /// </summary>
    public ResourceFilter When(Func<HttpContext, bool> filter)
    {
        var resourceFilter = new ResourceFilter
        {
            Filter = filter,
        };

        ResourceFilters.Add(resourceFilter);

        return resourceFilter;
    }

    /// <summary>
    /// Adds the provided asynchronous filter specified in <paramref name="filterAsync"/> to the list of
    /// <see cref="ResourceFilters"/>.
    /// </summary>
    public ResourceFilter When(Func<HttpContext, Task<bool>> filterAsync)
    {
        var resourceFilter = new ResourceFilter
        {
            FilterAsync = filterAsync,
        };

        ResourceFilters.Add(resourceFilter);

        return resourceFilter;
    }

    /// <summary>
    /// Adds a filter that matches any of the provided <paramref name="paths"/> to the list of
    /// <see cref="ResourceFilters"/>.
    /// </summary>
    public ResourceFilter WhenPath(params string[] paths)
    {
        var trimmedPaths = TrimPaths(paths);
        return When(context => IsPathContained(trimmedPaths, context));
    }

    /// <summary>
    /// Adds a filter that excludes all the provided <paramref name="paths"/> to the list of
    /// <see cref="ResourceFilters"/>.
    /// </summary>
    public ResourceFilter WhenNotPath(params string[] paths)
    {
        var trimmedPaths = TrimPaths(paths);
        return When(context => !IsPathContained(trimmedPaths, context));
    }

    /// <summary>
    /// Adds a filter that matches the path of the homepage (<c>"/"</c>) to the list of <see cref="ResourceFilters"/>.
    /// </summary>
    public ResourceFilter WhenHomePage() => WhenPath("/");

    /// <summary>
    /// Adds a filter that matches the beginning of the request's path with the given <paramref name="path"/> to the
    /// list of <see cref="ResourceFilters"/>.
    /// </summary>
    public ResourceFilter WhenPathStartsWith(string path) =>
        When(context => context.Request.Path.Value?.StartsWithOrdinalIgnoreCase(path) == true);

    /// <summary>
    /// Adds a filter that matches any of the provided <paramref name="contentTypes"/> to the list of
    /// <see cref="ResourceFilters"/> and it is currently Display display mode.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="contentTypes"/> has no provided items.
    /// </exception>
    public ResourceFilter WhenContentType(params string[] contentTypes) =>
        WhenContentTypeInner("Display", contentTypes);

    /// <summary>
    /// Adds a filter that matches any of the provided <paramref name="contentTypes"/> to the list of
    /// <see cref="ResourceFilters"/> and it is currently Edit display mode.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="contentTypes"/> has no provided items.
    /// </exception>
    public ResourceFilter WhenContentTypeEditor(params string[] contentTypes) =>
        WhenContentTypeInner("Edit", contentTypes);

    /// <summary>
    /// Adds a filter that matches any of the provided <paramref name="contentTypes"/> to the list of
    /// <see cref="ResourceFilters"/> when the content is being Previewed.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="contentTypes"/> has no provided items.
    /// </exception>
    public ResourceFilter WhenContentTypePreview(params string[] contentTypes) =>
        WhenContentTypeInner("Preview", contentTypes);

    /// <summary>
    /// Adds a filter that matches any of the provided <paramref name="contentTypes"/> to the list of
    /// <see cref="ResourceFilters"/> and it is currently Create display mode.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="contentTypes"/> has no provided items.
    /// </exception>
    public ResourceFilter WhenContentTypeCreate(params string[] contentTypes)
    {
        if (contentTypes.Length == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(contentTypes),
                $"{nameof(contentTypes)} must have at least 1 item.");
        }

        return When(context =>
        {
            var routeValues = context
                .Request
                .RouteValues
                .ToDictionary(pair => pair.Key, pair => pair.Value?.ToString(), StringComparer.OrdinalIgnoreCase);

            return routeValues.GetMaybe("action") == "Create" && contentTypes.Contains(routeValues.GetMaybe("id"));
        });
    }

    /// <summary>
    /// Adds an always matching filter to the list of <see cref="ResourceFilters"/>.
    /// </summary>
    public ResourceFilter Always(Action<IResourceManager>? execution = null)
    {
        var filter = When(_ => true);
        if (execution != null) filter.Executions.Add(execution);
        return filter;
    }

    private ResourceFilter WhenContentTypeInner(string displayType, params string[] contentTypes)
    {
        if (contentTypes.Length == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(contentTypes),
                $"{nameof(contentTypes)} must have at least 1 item.");
        }

        return When(async context =>
        {
            if (!GetContentItemId(displayType, context, out var contentItemId))
            {
                return false;
            }

            var session = context.RequestServices.GetRequiredService<ISession>();
            var query = displayType is "Edit" ?
                // We check for both published and draft content items.
                session.QueryIndex<ContentItemIndex>(index => index.Published || (index.Latest && !index.Published))
                : session.QueryContentItemIndex(PublicationStatus.Published);
            var contentItemIndex = await query
                .Where(index => index.ContentItemId == contentItemId)
                .FirstOrDefaultAsync(context.RequestAborted);
            return contentItemIndex?.ContentType is { } contentType &&
                contentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase);
        });
    }

    private static bool GetContentItemId(string displayType, HttpContext context, out string? contentItemId)
    {
        if (displayType == "Preview")
        {
            try
            {
                if (context.Request.GetFormValueMaybe("PreviewContentItemId") is { } previewContentItemId)
                {
                    contentItemId = previewContentItemId.FirstOrDefault();

                    return true;
                }
            }
            catch (InvalidDataException)
            {
                // The form contained invalid data, which can only really happen by deliberately crafting it like that,
                // what happens during security scans and cracking attempts. Nothing to do.
            }
        }
        else
        {
            var routeValues = context
                .Request
                .RouteValues
                .ToDictionary(pair => pair.Key, pair => pair.Value?.ToString(), StringComparer.OrdinalIgnoreCase);

            if (routeValues.GetMaybe("action") == displayType
                && routeValues.TryGetValue("contentItemId", out contentItemId))
            {
                return true;
            }
        }

        contentItemId = null;
        return false;
    }

    private static IEnumerable<string> TrimPaths(params string[] paths) =>
        paths.Select(path => path.Trim('/'));

    private static bool IsPathContained(IEnumerable<string> trimmedPaths, HttpContext context) =>
        trimmedPaths.Contains(context.Request.Path.Value?.Trim('/'), StringComparer.OrdinalIgnoreCase);
}
