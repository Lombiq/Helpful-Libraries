using Microsoft.Extensions.Options;
using OrchardCore.ResourceManagement;
using System;
using System.Collections.Concurrent;

namespace Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement;

public abstract class ResourceManagementOptionsConfiguratorBase : IConfigureOptions<ResourceManagementOptions>
{
    private static readonly ConcurrentDictionary<Type, ResourceManifest> _resourceManifests = new();

    protected abstract string Area { get; }

    protected abstract void Configure(ResourceManagementContext context);

    public void Configure(ResourceManagementOptions options) =>
        options.ResourceManifests.Add(_resourceManifests.GetOrAdd(GetType(), _ =>
        {
            var manifest = new ResourceManifest();
            Configure(new ResourceManagementContext(manifest, this));
            return manifest;
        }));

    protected record ResourceManagementContext(
        ResourceManifest Manifest,
        ResourceManagementOptionsConfiguratorBase Configurator)
    {
        /// <summary>
        /// Define a style resource inside the <c>~/{Area}/css/{filename}</c> location.
        /// </summary>
        public ResourceDefinition DefineStyle(string resourceName, string fileName, params string[] dependencies) =>
            SetUrlAndDependencies(Manifest.DefineStyle(resourceName), "css", fileName, dependencies);

        /// <summary>
        /// Define a script resource inside the <c>~/{Area}/js/{filename}</c> location.
        /// </summary>
        public ResourceDefinition DefineScript(string resourceName, string fileName, params string[] dependencies) =>
            SetUrlAndDependencies(Manifest.DefineScript(resourceName), "js", fileName, dependencies);

        /// <summary>
        /// Define a style resource inside the <c>~/{Area}/vendors/{filename}</c> location.
        /// </summary>
        public ResourceDefinition DefineVendorStyle(string resourceName, string fileName, params string[] dependencies) =>
            SetUrlAndDependencies(Manifest.DefineStyle(resourceName), "vendors", fileName, dependencies);

        /// <summary>
        /// Define a script resource inside the <c>~/{Area}/vendors/{filename}</c> location.
        /// </summary>
        public ResourceDefinition DefineVendorScript(string resourceName, string fileName, params string[] dependencies) =>
            SetUrlAndDependencies(Manifest.DefineScript(resourceName), "vendors", fileName, dependencies);

        private ResourceDefinition SetUrlAndDependencies(
            ResourceDefinition definition,
            string type,
            string fileName,
            string[] dependencies) =>
            definition
                .SetUrl($"~/{Configurator.Area}/{type}/{fileName}")
                .SetDependencies(dependencies);
    }
}
