using Microsoft.AspNetCore.Mvc;
using OrchardCore.Admin;
using System;

namespace Lombiq.HelpfulLibraries.Tests.Controllers;

public class RouteTestController : Controller
{
    public IActionResult Foo() => Content(string.Empty);

    public IActionResult Bar() => Content(string.Empty);

    [Admin]
    public IActionResult Baz() => Content(string.Empty);

    [Route("I/Am/Routed")]
    public IActionResult Route() => Content(string.Empty);

    [Admin]
    [Route("I/Am/Routed/Admin")]
    public IActionResult AdminRoute() => Content(string.Empty);

    [Route("content/{id}")]
    public IActionResult RouteSubstitution(int id) => Content(string.Empty);

    [Route("content/{id}/{selector?}")]
    public IActionResult RouteSubstitutionOptional(int id, string selector, string anotherValue) => Content(string.Empty);

    public IActionResult Arguments(int number, double fraction, DateTime dateTime, string text) =>
        Content(string.Empty);
}
