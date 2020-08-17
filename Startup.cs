using Lombiq.HelpfulLibraries.Libraries.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Lombiq.HelpfulLibraries
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services) =>
            services.AddCoreOrchardServiceImplementations(typeof(Startup).Assembly);
    }
}
