using Microsoft.AspNetCore.Mvc;
using OrchardCore.Admin;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Lombiq.HelpfulLibraries.Tests.Controllers;

[SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Just a dummy controller to test URL generation.")]
public class RouteTestController : Controller
{
    public IActionResult Foo() => Content(string.Empty);

    public IActionResult Bar() => Content(string.Empty);

    [Admin]
    public IActionResult Baz() => Content(string.Empty);

    [Route("I/Am/Routed")]
    public IActionResult Route() => Content(string.Empty);

    [Route("content/{id}")]
    public IActionResult RouteSubstitution(int id) => Content(string.Empty);

    public IActionResult Arguments(int number, double fraction, DateTime dateTime, string text) =>
        Content(string.Empty);
}
