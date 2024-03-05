using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using static Lombiq.HelpfulLibraries.AspNetCore.Security.ContentSecurityPolicyDirectives;

namespace Lombiq.HelpfulLibraries.Samples.Controllers;

public class ScriptModulesController : Controller
{
    [ContentSecurityPolicy("eval", ScriptSrc)]
    public IActionResult Index() => View();
}
