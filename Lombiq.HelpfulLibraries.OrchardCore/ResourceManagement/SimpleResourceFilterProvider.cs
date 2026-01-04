using OrchardCore.Modules;
using System;
using System.Collections.Generic;

namespace Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement;

internal sealed class SimpleResourceFilterProvider : IResourceFilterProvider
{
    private readonly Action<ResourceFilterBuilder> _filter;

    public IEnumerable<string> RequiredThemes { get; private set; }

    public SimpleResourceFilterProvider(Action<ResourceFilterBuilder> filter, ICollection<string> requiredThemes, IClock clock)
    {
        _filter = filter;
        RequiredThemes = requiredThemes;
    }

    public void AddResourceFilter(ResourceFilterBuilder builder) => _filter(builder);
}
