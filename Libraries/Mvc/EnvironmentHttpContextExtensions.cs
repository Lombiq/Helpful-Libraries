using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Microsoft.AspNetCore.Http
{
    public static class EnvironmentHttpContextExtensions
    {
        public static bool IsDevelopment(this HttpContext httpContext) =>
            httpContext.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment();
    }
}
