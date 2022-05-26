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

    public ResourceFilter When(Func<HttpContext, bool> filter)
    {
        var resourceFilter = new ResourceFilter
        {
            Filter = filter,
        };

        ResourceFilters.Add(resourceFilter);

        return resourceFilter;
    }

    public ResourceFilter When(Func<HttpContext, Task<bool>> filterAsync)
    {
        var resourceFilter = new ResourceFilter
        {
            FilterAsync = filterAsync,
        };

        ResourceFilters.Add(resourceFilter);

        return resourceFilter;
    }

    public ResourceFilter WhenPath(string path) =>
        When(context => context.Request.Path.Value?.EqualsOrdinalIgnoreCase(path) == true);

    public ResourceFilter WhenHomePage() => WhenPath("/");

    public ResourceFilter WhenPathStartsWith(string path) =>
        When(context => context.Request.Path.Value?.StartsWithOrdinalIgnoreCase(path) == true);

    public ResourceFilter WhenContentType(params string[] contentTypes)
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

            if (routeValues.GetMaybe("action") != "Display" ||
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

    public ResourceFilter Always(Action<IResourceManager> execution = null)
    {
        var filter = When(_ => true);
        filter.Execution = execution;
        return filter;
    }
}
