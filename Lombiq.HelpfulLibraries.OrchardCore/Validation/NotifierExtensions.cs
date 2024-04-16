using Lombiq.HelpfulLibraries.AspNetCore.Exceptions;
using OrchardCore.DisplayManagement.Notify;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Validation;

public static class NotifierExtensions
{
    /// <summary>
    /// Emits an error notification for each entry of <see cref="FrontendException.HtmlMessages"/> individually.
    /// </summary>
    public static async Task FrontEndErrorAsync(this INotifier notifier, FrontendException exception)
    {
        foreach (var message in exception.HtmlMessages)
        {
            await notifier.ErrorAsync(message);
        }
    }
}
