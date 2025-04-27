using System;

namespace Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement;

internal class SimpleResourceFilterProvider : IResourceFilterProvider
{
    private readonly Action<ResourceFilterBuilder> _filter;

    public SimpleResourceFilterProvider(Action<ResourceFilterBuilder> filter) =>
        _filter = filter;

    public void AddResourceFilter(ResourceFilterBuilder builder) => _filter(builder);
}
