using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using System;

namespace Lombiq.HelpfulLibraries.AspNetCore.Mvc;

/// <summary>
/// Enforces the Development environment as well as localhost. When put on a controller or an action it'll set a <see
/// cref="NotFoundResult"/> if the current <see cref="IHostEnvironment"/> is not Development or if the host of the
/// current URL is "localhost".
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class DevelopmentAndLocalhostOnlyAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.HttpContext.IsDevelopmentAndLocalhost())
        {
            context.Result = new NotFoundResult();
            return;
        }

        base.OnActionExecuting(context);
    }
}
