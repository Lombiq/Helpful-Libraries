using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Lombiq.HelpfulLibraries.OrchardCore.Caching;

public static class WebApplicationExtensions
{
    public static WebApplication UseCachedWebRoot(this WebApplication app)
    {
        app.Environment.WebRootFileProvider = app.Services.GetRequiredService<CachedWebRootFileProvider>();
        return app;
    }
}
