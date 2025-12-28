using Microsoft.Extensions.Options;
using OrchardCore.ResourceManagement;
using System;
using System.Collections.Concurrent;

namespace Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement;

public abstract class ResourceManagementOptionsConfiguratorBase : IConfigureOptions<ResourceManagementOptions>
{
    private const string Vendors = "vendors";

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

    public record ResourceManagementContext(
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
        /// Define an ES module script resource inside the <c>~/{Area}/js/{filename}</c> location.
        /// </summary>
        public ResourceDefinition DefineScriptModule(string resourceName, string fileName, params string[] dependencies) =>
            SetUrlAndDependencies(Manifest.DefineScriptModule(resourceName), "js", fileName, dependencies);

        /// <summary>
        /// Define a style resource inside the <c>~/{Area}/vendors/{filename}</c> location.
        /// </summary>
        public ResourceDefinition DefineVendorStyle(string resourceName, string fileName, params string[] dependencies) =>
            SetUrlAndDependencies(Manifest.DefineStyle(resourceName), Vendors, fileName, dependencies);

        /// <summary>
        /// Define a style resource inside the <c>~/{Area}/vendors/{filenames}</c> location.
        /// </summary>
        public ResourceDefinition DefineVendorStyle(
            string resourceName,
            (string Production, string Debug) fileNames,
            params string[] dependencies) =>
            SetUrlAndDependencies(Manifest.DefineStyle(resourceName), Vendors, fileNames, dependencies);

        /// <summary>
        /// Define a script resource inside the <c>~/{Area}/vendors/{filename}</c> location.
        /// </summary>
        public ResourceDefinition DefineVendorScript(string resourceName, string fileName, params string[] dependencies) =>
            SetUrlAndDependencies(Manifest.DefineScript(resourceName), Vendors, fileName, dependencies);

        /// <summary>
        /// Define a script resource inside the <c>~/{Area}/vendors/{filenames}</c> location.
        /// </summary>
        public ResourceDefinition DefineVendorScript(
            string resourceName,
            (string Production, string Debug) fileNames,
            params string[] dependencies) =>
            SetUrlAndDependencies(Manifest.DefineScript(resourceName), Vendors, fileNames, dependencies);

        /// <summary>
        /// Define an ES module script resource inside the <c>~/{Area}/vendors/{filename}</c> location.
        /// </summary>
        public ResourceDefinition DefineVendorScriptModule(string resourceName, string fileName, params string[] dependencies) =>
            SetUrlAndDependencies(Manifest.DefineScriptModule(resourceName), Vendors, fileName, dependencies);

        /// <inheritdoc cref="DefineVendorScriptModule(string, string, string[])" />
        public ResourceDefinition DefineVendorScriptModule(
            string resourceName,
            (string Production, string Debug) fileNames,
            params string[] dependencies) =>
            SetUrlAndDependencies(Manifest.DefineScriptModule(resourceName), Vendors, fileNames, dependencies);

        private ResourceDefinition SetUrlAndDependencies(
            ResourceDefinition definition,
            string type,
            string fileName,
            string[] dependencies) =>
            SetUrlAndDependencies(definition, type, (Production: fileName, Debug: null), dependencies);

        private ResourceDefinition SetUrlAndDependencies(
            ResourceDefinition definition,
            string type,
            (string Production, string? Debug) fileNames,
            string[] dependencies) =>
            definition
                .SetUrl(
                    $"~/{Configurator.Area}/{type}/{fileNames.Production}",
                    string.IsNullOrEmpty(fileNames.Debug)
                        ? null
                        : $"~/{Configurator.Area}/{type}/{fileNames.Debug}")
                .SetDependencies(dependencies);
    }
}
