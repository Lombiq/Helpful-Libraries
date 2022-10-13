using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.Samples.Controllers;

public class ErrorController : Controller
{
    // A front-end page to demonstrate error handling on JSON-returning REST API requests.
    // /Lombiq.HelpfulLibraries.Samples/Error/Json
    public IActionResult Json() => View();

    // Demonstrate what happens when the code throws an exception while fulfilling the API request. This method shows
    // that you should place everything that could throw an exception inside the SafeJsonAsync's callback.
    // /Lombiq.HelpfulLibraries.Samples/Error/JsonError
    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> JsonError() =>
        await this.SafeJsonAsync<object>(() =>
            throw new InvalidOperationException("Intentional failure."));

    // Same as above, but the exception is thrown in a separate thread inside the asynchronous state machine.
    // /Lombiq.HelpfulLibraries.Samples/Error/JsonErrorWithAwait
    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> JsonErrorWithAwait() =>
        await this.SafeJsonAsync<object>(async () =>
        {
            await Task.Delay(1, CancellationToken.None); // Simulate some asynchronous activity.
            throw new InvalidOperationException("Intentional failure.");
        });

    // This method actually returns an object and no error.
    // /Lombiq.HelpfulLibraries.Samples/Error/JsonSuccess
    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> JsonSuccess() =>
        await this.SafeJsonAsync(async () =>
        {
            await Task.Delay(1, CancellationToken.None); // Simulate some asynchronous activity.
            return new
            {
                Foo = "bar",
                This = true,
                That = 10,
            };
        });
}
