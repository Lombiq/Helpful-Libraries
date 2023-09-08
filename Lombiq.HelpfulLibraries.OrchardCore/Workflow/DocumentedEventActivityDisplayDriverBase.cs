using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Workflows.Activities;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
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

        return null;
    }

    private ValueTask NotifyAsync(LocalizedString title, IDictionary<string, string> content)
    {
        var layoutBuilder = new StringBuilder("<strong>{0}</strong><dl>");

        var arguments = new List<object>(capacity: 1 + (2 * content.Count)) { title.Value };

        foreach (var (name, schema) in content)
        {
            var index = arguments.Count;
            layoutBuilder.Append("<dt>{").Append(index).Append("}</dt><dd><code>{").Append(index + 1).Append("}</code></dd>");
            arguments.Add(name);
            arguments.Add(schema);
        }

        var layout = layoutBuilder.Append("</dl>").ToString();
        return _notifier.InformationAsync(new LocalizedHtmlString(
            layout,
            layout,
            isResourceNotFound: false,
            arguments.ToArray()));
    }
}

[SuppressMessage("Minor Code Smell", "S2094:Classes should not be empty", Justification = "Used for shared localization.")]
public class DocumentedEventActivityDisplayDriver
{
}
