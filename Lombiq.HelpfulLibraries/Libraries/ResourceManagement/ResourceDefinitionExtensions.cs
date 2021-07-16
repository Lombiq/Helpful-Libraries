using System.Collections.Generic;
using System.Linq;

namespace OrchardCore.ResourceManagement
{
    public static class ResourceManifestExtensions
    {
        /// <summary>
        /// Adds <paramref name="newDependencies"/> and all of their dependencies recursively, and strips duplicates.
        /// </summary>
        public static ResourceDefinition SetDependenciesRecursively(
            this ResourceDefinition definition,
            ResourceManifest manifest,
            params ResourceDefinition[] newDependencies)
        {
            var resources = manifest.GetResources(definition.Type);

            definition.SetDependencies(); // This is to ensure definition.Dependencies is not null.

            void Add(IEnumerable<ResourceDefinition> additionalDependencies)
            {
                foreach (var dependency in additionalDependencies)
                {
                    definition.Dependencies.Add(dependency.Name);

                    var children = dependency
                        .Dependencies
                        .SelectWhere(subDependency => resources.GetMaybe(subDependency))
                        .SelectMany(subDependencies => subDependencies);
                    Add(children);
                }
            }

            Add(newDependencies);
            var unique = definition.Dependencies.ToHashSet();
            definition.Dependencies.Clear();
            definition.Dependencies.AddRange(unique);

            return definition;
        }
    }
}
