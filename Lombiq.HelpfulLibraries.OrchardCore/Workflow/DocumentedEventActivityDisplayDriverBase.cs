using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Workflows.Activities;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Workflow;

public class DocumentedEventActivityDisplayDriverBase<TActivity> : SimpleEventActivityDisplayDriverBase<TActivity>
    where TActivity : class, IActivity
{
    protected readonly INotifier _notifier;
    private readonly IStringLocalizer<DocumentedEventActivityDisplayDriver> T;

    public virtual IDictionary<string, string> AvailableInputs => ImmutableDictionary<string, string>.Empty;
    public virtual IDictionary<string, string> ExpectedOutputs => ImmutableDictionary<string, string>.Empty;

    public DocumentedEventActivityDisplayDriverBase(
        INotifier notifier,
        IStringLocalizer<DocumentedEventActivityDisplayDriver> baseLocalizer)
    {
        _notifier = notifier;
        T = baseLocalizer;
    }

    public override async Task<IDisplayResult> EditAsync(TActivity model, BuildEditorContext context)
    {
        if (AvailableInputs?.Any() == true)
        {
            await NotifyAsync(T["The available inputs are:"], AvailableInputs);
        }

        if (ExpectedOutputs?.Any() == true)
        {
            await NotifyAsync(T["The expected outputs are:"], ExpectedOutputs);
        }

        return null; // We don't display any shapes, just the notifications above.
    }

    private ValueTask NotifyAsync(LocalizedString title, IDictionary<string, string> content)
    {
        var layoutBuilder = new StringBuilder(
            "{0}<table class=\"table\"><thead><tr><th scope=\"col\">{1}</th><th scope=\"col\">{2}</th></tr></thead><tbody>");

        var arguments = new List<object>(capacity: 3 + (2 * content.Count))
        {
            title.Value,
            T["Name"].Value,
            T["Type"].Value,
        };

        foreach (var (name, schema) in content)
        {
            var index = arguments.Count;
            layoutBuilder.Append(
                CultureInfo.InvariantCulture,
                $"<tr><td>{{{index}}}</td><td><code>{{{index + 1}}}</code></td></tr>");
            arguments.Add(name);
            arguments.Add(schema);
        }

        var layout = layoutBuilder.Append("</table>").ToString();
        return _notifier.InformationAsync(new LocalizedHtmlString(
            layout,
            layout,
            isResourceNotFound: false,
            arguments));
    }
}

public class DocumentedEventActivityDisplayDriver
{
}
