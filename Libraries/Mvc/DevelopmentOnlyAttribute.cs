using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Lombiq.HelpfulLibraries.Libraries.Mvc
{
    /// <summary>
    /// Enforces the Development environment. When put on a controller or an action it'll set a <see
    /// cref="NotFoundResult"/> if the current <see cref="IHostEnvironment"/> is not Development.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class DevelopmentOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var environment = context.HttpContext.RequestServices.GetRequiredService<IHostEnvironment>();
            if (!environment.IsDevelopment())
            {
                context.Result = new NotFoundResult();
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
