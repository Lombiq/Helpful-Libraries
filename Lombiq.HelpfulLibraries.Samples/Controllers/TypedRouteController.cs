using Microsoft.AspNetCore.Mvc;
using System;

namespace Lombiq.HelpfulLibraries.Samples.Controllers;

public class TypedRouteController : Controller
{
    // Open this from under /Lombiq.HelpfulLibraries.Samples/TypedRoute/Index
    public ActionResult Index() => View();

    public ActionResult TypedRouteSample(string text, int number) => Content($"{text}: {number.ToTechnicalString()}");
}
