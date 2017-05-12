using Orchard;
using Orchard.Environment;
using Orchard.Environment.Extensions;
using Orchard.Mvc;
using Piedone.HelpfulLibraries.Libraries.Utilities;

namespace Autofac
{
    /* Custom ILifetimeScope builder to register the placeholder IHttpContextAccessor implementation. This is necessary 
     * because IHttpContextAccessor is an application-wide dependency, not even a shell singleton. This would cause all 
     * types of failures on other shells when calling IHttpContextAccessor.Set() (e.g. to work around work context being 
     * lost because of async or building a new WorkContextScope), since that would set the HttpContext everywhere.
     * This could be much better if this is fixed: https://github.com/OrchardCMS/Orchard/issues/4338 as now we need a 
     * stub HttpContext to carry the work context.
     * See also: http://stackoverflow.com/questions/8658946/autofac-lifetimes-and-the-default-provider-within-a-matching-lifetime-scope */
    [OrchardFeature("Piedone.HelpfulLibraries.Utilities")]
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder BuildBackgroundLifetimeScope(this ContainerBuilder builder)
        {
            builder.RegisterType<HttpContextPlaceholderAccessor>().As<IHttpContextAccessor>().InstancePerLifetimeScope();
            // Re-registering WorkContextAccessor is needed so it gets the stubbed IHttpContextAccessor.
            builder.RegisterType<WorkContextAccessor>().As<IWorkContextAccessor>().InstancePerLifetimeScope();

            return builder;
        }
    }
}
