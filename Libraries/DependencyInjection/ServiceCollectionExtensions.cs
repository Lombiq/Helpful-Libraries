using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Admin;
using OrchardCore.BackgroundTasks;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.Data.Migration;
using OrchardCore.DisplayManagement.Theming;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using OrchardCore.ResourceManagement;
using OrchardCore.Security.Permissions;

namespace Lombiq.HelpfulLibraries.Libraries.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        private static readonly Type[] _singletonTypes = new[]
        {
            typeof(ContentPart), typeof(IBackgroundTask)
        };

        private static readonly Type[] _scopedTypes = new[]
        {
            typeof(IDataMigration), typeof(IContentPartDisplayDriver), typeof(IResourceManifestProvider),
            typeof(IPermissionProvider), typeof(IThemeSelector), typeof(IAdminThemeService), typeof(INavigationProvider)
        };

        private static readonly Type[] _transientTypes = new[]
        {
            typeof(IModularTenantEvents)
        };


        public static void AddCoreOrchardServiceImplementations(this IServiceCollection serviceCollection, Assembly assembly)
        {
            var publicClassTypes = assembly
                .GetExportedTypes()
                .Where(type => type.IsClass && !type.IsAbstract && !type.IsGenericType && !type.IsNested);

            foreach (var classType in publicClassTypes)
            {
                // If classType implements multiple services then register it for each of them.

                foreach (var singletonServiceType in _singletonTypes.Where(singletonType => singletonType.IsAssignableFrom(classType)))
                {
                    serviceCollection.AddSingleton(singletonServiceType, classType);
                }

                foreach (var scopedServiceType in _scopedTypes.Where(scopedType => scopedType.IsAssignableFrom(classType)))
                {
                    serviceCollection.AddScoped(scopedServiceType, classType);
                }

                foreach (var transientServiceType in _transientTypes.Where(transientType => transientType.IsAssignableFrom(classType)))
                {
                    serviceCollection.AddScoped(transientServiceType, classType);
                }
            }
        }
    }
}
