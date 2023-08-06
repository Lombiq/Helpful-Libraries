using Microsoft.Extensions.Localization;
using OrchardCore.Workflows.Abstractions.Models;
using OrchardCore.Workflows.Activities;
using OrchardCore.Workflows.Models;
using System.Collections.Generic;

namespace Lombiq.HelpfulLibraries.OrchardCore.Workflow;

/// <summary>
/// A base class for a simple workflow event that only has a <c>Done</c> result.
/// </summary>
public abstract class SimpleEventActivity : EventActivity
{
    protected readonly IStringLocalizer T;

    public override string Name => GetType().Name;

    public abstract override LocalizedString DisplayText { get; }
    public abstract override LocalizedString Category { get; }

    protected SimpleEventActivity(IStringLocalizer stringLocalizer) =>
        T = stringLocalizer;

    public override IEnumerable<Outcome> GetPossibleOutcomes(
        WorkflowExecutionContext workflowContext,
        ActivityContext activityContext) =>
        new[] { new Outcome(T["Done"]) };

    public override ActivityExecutionResult Resume(
        WorkflowExecutionContext workflowContext,
        ActivityContext activityContext) =>
        Outcomes("Done");
}
