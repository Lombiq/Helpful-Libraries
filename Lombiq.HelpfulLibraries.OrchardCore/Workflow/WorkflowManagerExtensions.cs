using OrchardCore.ContentManagement;
using OrchardCore.Workflows.Activities;
using OrchardCore.Workflows.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Workflow;

public static class WorkflowManagerExtensions
{
    /// <summary>
    /// Triggers an event by passing <paramref name="content"/>'s <see cref="ContentItem"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the activity to trigger. This will only work when it's the same as the event's type name which is
    /// customary in most events and enforced in <see cref="SimpleEventActivity"/> events.
    /// </typeparam>
    public static Task TriggerContentItemEventAsync<T>(this IWorkflowManager workflowManager, IContent content)
        where T : IEvent
    {
        var contentItem = content.ContentItem;
        return workflowManager.TriggerEventAsync(
            typeof(T).Name,
            contentItem,
            $"{contentItem.ContentType}-{contentItem.ContentItemId}");
    }

    /// <inheritdoc cref="TriggerContentItemEventAsync{T}(IWorkflowManager, IContent)"/>
    /// <remarks><para>Executes on the first item of <paramref name="workflowManagers"/> if any.</para></remarks>
    public static Task TriggerContentItemEventAsync<T>(this IEnumerable<IWorkflowManager> workflowManagers, IContent content)
        where T : IEvent =>
        workflowManagers.InvokeFirstOrCompletedAsync(manager => manager.TriggerContentItemEventAsync<T>(content));

    /// <summary>
    /// Triggers the <see cref="IEvent"/> identified by <paramref name="name"/>.
    /// </summary>
    /// <remarks><para>Executes on the first item of <paramref name="workflowManagers"/> if any.</para></remarks>
    public static Task TriggerEventAsync(
        this IEnumerable<IWorkflowManager> workflowManagers,
        string name,
        object input = null,
        string correlationId = null) =>
        workflowManagers.InvokeFirstOrCompletedAsync(manager => manager.TriggerEventAsync(name, input, correlationId));

    /// <summary>
    /// Triggers the <see cref="IEvent"/> identified by <typeparamref name="T"/>.
    /// </summary>
    public static Task TriggerEventAsync<T>(
        this IWorkflowManager workflowManager,
        object input = null,
        string correlationId = null)
        where T : IEvent =>
        workflowManager.TriggerEventAsync(typeof(T).Name, input, correlationId);

    /// <summary>
    /// Triggers the <see cref="IEvent"/> identified by <typeparamref name="T"/>.
    /// </summary>
    /// <remarks><para>Executes on the first item of <paramref name="workflowManagers"/> if any.</para></remarks>
    public static Task TriggerEventAsync<T>(
        this IEnumerable<IWorkflowManager> workflowManagers,
        object input = null,
        string correlationId = null)
        where T : IEvent =>
        workflowManagers.TriggerEventAsync(typeof(T).Name, input, correlationId);
}
