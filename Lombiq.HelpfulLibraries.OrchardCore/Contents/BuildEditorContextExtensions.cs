#nullable enable

using OrchardCore.DisplayManagement.Entities;
using System;
using System.Threading.Tasks;

namespace OrchardCore.DisplayManagement.Handlers;

public static class BuildEditorContextExtensions
{
    /// <inheritdoc cref="CreateModelAsync{TViewModel}"/>
    /// <param name="groupId">
    /// If not <see langword="null"/>, it's needed to check the group (e.g. in <see
    /// cref="SectionDisplayDriver{TModel,TSection}"/>. The value is checked against the <see
    /// cref="BuildShapeContext.GroupId"/> and if they don't match <see langword="null"/> is returned.
    /// </param>
    /// <param name="authorizeAsync">
    /// If not <see langword="null"/> and the awaited result is <see langword="false"/>, then <see langword="null"/> is
    /// returned.
    /// </param>
    /// <returns>
    /// A new instance of <typeparamref name="TViewModel"/>, populated with data from <paramref name="context"/>. Unless
    /// at least one of <paramref name="groupId"/> and <paramref name="authorizeAsync"/> are provided and the checks
    /// failed, at which case <see langword="null"/> is returned.
    /// </returns>
    [Obsolete($"Inherit your driver from {nameof(SiteDisplayDriver<object>)} instead, which does the group ID check implicitly.")]
    public static Task<TViewModel?> CreateModelMaybeAsync<TViewModel>(
        this BuildEditorContext context,
        string? prefix,
        string groupId,
        Func<Task<bool>>? authorizeAsync = null)
        where TViewModel : class, new() =>
        !string.IsNullOrEmpty(groupId) && context.GroupId != groupId
            ? Task.FromResult<TViewModel?>(null)
            : context.CreateModelMaybeAsync<TViewModel>(prefix, authorizeAsync);

    /// <inheritdoc cref="CreateModelAsync{TViewModel}"/>
    /// <param name="authorizeAsync">If <see langword="false"/>, then <see langword="null"/> is returned.</param>
    /// <returns>
    /// A new instance of <typeparamref name="TViewModel"/> populated with data from <paramref name="context"/>, or <see
    /// langword="null"/> if <paramref name="authorizeAsync"/>'s check fails.
    /// </returns>
    /// <remarks><para>
    /// If authorization is not needed, use <see cref="CreateModelAsync{TViewModel}"/> instead.
    /// </para></remarks>
    public static async Task<TViewModel?> CreateModelMaybeAsync<TViewModel>(
        this BuildEditorContext context,
        string? prefix,
        Func<Task<bool>>? authorizeAsync)
        where TViewModel : class, new() =>
        authorizeAsync != null && !await authorizeAsync()
            ? null
            : await context.CreateModelAsync<TViewModel>(prefix);

    /// <summary>
    /// Creates a new instance of <typeparamref name="TViewModel"/> and tries to populate it from the <paramref
    /// name="context"/> with the <paramref name="prefix"/>.
    /// </summary>
    /// <param name="context">The editor context of the current update request.</param>
    /// <param name="prefix">
    /// The name prefix of the fields being edited, usually <see cref="DisplayDriverBase.Prefix"/>.
    /// </param>
    /// <typeparam name="TViewModel">The expected view-model type.</typeparam>
    /// <returns>
    /// A new instance of <typeparamref name="TViewModel"/>, populated with data from <paramref name="context"/>.
    /// </returns>
    public static async Task<TViewModel> CreateModelAsync<TViewModel>(this BuildEditorContext context, string? prefix)
        where TViewModel : class, new()
    {
        var viewModel = new TViewModel();
        await context.Updater.TryUpdateModelAsync(viewModel, prefix);
        return viewModel;
    }
}
