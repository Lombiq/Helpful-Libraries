using Autofac;
using Autofac.Core;
using Orchard.Environment.Extensions;
using Piedone.HelpfulLibraries.ServiceValidation.ValidationDictionaries;

namespace Piedone.HelpfulLibraries.ServiceValidation
{
    [OrchardFeature("Piedone.HelpfulLibraries.ServiceValidation")]
    public class ServiceValidationModule : IModule
    {
        public void Configure(IComponentRegistry componentRegistry)
        {
            // This is necessary as generic dependencies are currently not resolved, see issue: https://github.com/OrchardCMS/Orchard/issues/1968
            var builder = new ContainerBuilder();
            builder.RegisterGeneric(typeof(ServiceValidationDictionary<>)).As(typeof(IServiceValidationDictionary<>));

            builder.Update(componentRegistry);
        }
    }
}
