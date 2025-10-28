#nullable enable

using Lombiq.HelpfulLibraries.AspNetCore.Security;
using Microsoft.AspNetCore.Http;
using OrchardCore.ResourceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement.ResourceTypes;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Looks in the resource manager for a resource of type <see cref="ResourceType"/> called <see cref="ResourceName"/>.
/// If found, the directive <see cref="DirectiveName"/> is amended with the value or values in <see
/// cref="DirectiveValue"/>. The <see cref="DirectiveNameChain"/> refers to the resolution order where to look for the
/// existing directive values. Its first item is the <see cref="DirectiveName"/>.
/// </summary>
public abstract class ResourceManagerContentSecurityPolicyProvider : IContentSecurityPolicyProvider
{
    protected virtual IList<string> ResourceTypes { get; init; } = [];
    protected abstract string ResourceName { get; }
    protected abstract IReadOnlyCollection<string> DirectiveNameChain { get; }
    protected abstract string DirectiveValue { get; }

    protected string DirectiveName => DirectiveNameChain.First();

    [Obsolete($"Use {nameof(ResourceTypes)} instead.")]
    protected virtual string ResourceType => ResourceTypes.FirstOrDefault() ?? Script;

    public ValueTask UpdateAsync(IDictionary<string, string> securityPolicies, HttpContext context)
    {
        var resourceManager = context.RequestServices.GetRequiredService<IResourceManager>();

        var resourceExists = ResourceTypes.FirstOrDefault(resourceType => resourceManager
            .GetRequiredResources(resourceType)
            .Any(script => script.Resource.Name == ResourceName)) != null;

        if (resourceExists)
        {
            ContentSecurityPolicyProvider.MergeDirectiveValues(securityPolicies, DirectiveNameChain, DirectiveValue);
        }

        return ThenUpdateAsync(securityPolicies, context, resourceExists);
    }

    /// <summary>
    /// When overridden, this may be used for additional updates related to the resource in <see cref="ResourceName"/>.
    /// </summary>
    protected virtual ValueTask ThenUpdateAsync(
        IDictionary<string, string> securityPolicies,
        HttpContext context,
        bool resourceExists) =>
        ValueTask.CompletedTask;
}
