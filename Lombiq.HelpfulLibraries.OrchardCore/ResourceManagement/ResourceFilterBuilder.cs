using Lombiq.HelpfulLibraries.OrchardCore.Contents;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ResourceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YesSql;
using ISession = YesSql.ISession;

namespace Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement;

public class ResourceFilterBuilder
{
    public IList<ResourceFilter> ResourceFilters { get; private set; } = new List<ResourceFilter>();

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
    /// Adds a filter that matches the given <paramref name="path"/> to the list of <see cref="ResourceFilters"/>.
    /// </summary>
    public ResourceFilter WhenPath(string path) =>
        When(context => context.Request.Path.Value?.Trim('/').EqualsOrdinalIgnoreCase(path.Trim('/')) == true);

    /// <summary>
    /// Adds a filter that matches any of the provided <paramref name="paths"/> to the list of
    /// <see cref="ResourceFilters"/>.
    /// </summary>
    public ResourceFilter WhenPaths(params string[] paths)
    {
        var trimmedPaths = paths.Select(path => path.Trim('/'));
        return When(context =>
            trimmedPaths.Contains(context.Request.Path.Value?.Trim('/'), StringComparer.OrdinalIgnoreCase));
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
    /// Adds an always matching filter to the list of <see cref="ResourceFilters"/>.
    /// </summary>
    public ResourceFilter Always(Action<IResourceManager> execution = null)
    {
        var filter = When(_ => true);
        filter.Execution = execution;
        return filter;
    }

    private ResourceFilter WhenContentTypeInner(string displayType, params string[] contentTypes)
    {
        if (!contentTypes.Any())
        {
            throw new ArgumentOutOfRangeException(
                nameof(contentTypes),
                $"{nameof(contentTypes)} must have at least 1 item.");
        }

        return When(async context =>
        {
            var routeValues = context
                .Request
                .RouteValues
                .ToDictionary(pair => pair.Key, pair => pair.Value?.ToString(), StringComparer.OrdinalIgnoreCase);

            if (routeValues.GetMaybe("action") != displayType ||
                routeValues.GetMaybe("contentItemId") is not { } contentItemId)
            {
                return false;
            }

            var session = context.RequestServices.GetRequiredService<ISession>();
            var contentItemIndex = await session.QueryContentItemIndex(PublicationStatus.Published)
                .Where(index => index.ContentItemId == contentItemId)
                .FirstOrDefaultAsync();
            return contentItemIndex?.ContentType is { } contentType &&
                contentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase);
        });
    }
}
