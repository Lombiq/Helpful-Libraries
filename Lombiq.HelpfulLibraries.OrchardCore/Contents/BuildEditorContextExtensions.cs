using OrchardCore.DisplayManagement.Entities;
using System;
using System.Threading.Tasks;

namespace OrchardCore.DisplayManagement.Handlers;

public static class BuildEditorContextExtensions
{
    /// <summary>
    /// Creates a new instance of <typeparamref name="TViewModel"/> and tries to populate it from the <paramref
    /// name="context"/> with the <paramref name="prefix"/>.
    /// </summary>
    /// <param name="context">The editor context of the current update request.</param>
    /// <param name="prefix">The name prefix of the fields being edited.</param>
    /// <param name="groupId">
    /// If not <see langword="null"/>, it's needed to check the group (e.g. in <see
    /// cref="SectionDisplayDriver{TModel,TSection}"/>. The value is checked against the <see
    /// cref="BuildShapeContext.GroupId"/> and if they don't match <see langword="null"/> is returned.
    /// </param>
    /// <param name="authorizeAsync">
    /// If not <see langword="null"/> and the awaited result is <see langword="false"/>, then <see langword="null"/> is
    /// returned.
    /// </param>
    /// <typeparam name="TViewModel">The expected view-model type.</typeparam>
    /// <returns>
    /// A new instance of <typeparamref name="TViewModel"/>, populated with data from <paramref name="context"/>. Unless
    /// at least one of <paramref name="groupId"/> and <paramref name="authorizeAsync"/> are provided and the checks
    /// failed, at which case <see langword="null"/> is returned.
    /// </returns>
    public static async Task<TViewModel> CreateModelMaybeAsync<TViewModel>(
        this BuildEditorContext context,
        string prefix,
        string groupId = null,
        Func<Task<bool>> authorizeAsync = null)
        where TViewModel : class, new()
    {
        if (!string.IsNullOrEmpty(groupId) && context.GroupId != groupId) return null;

        if (authorizeAsync != null && !await authorizeAsync())
        {
            return null;
        }

        var viewModel = new TViewModel();
        await context.Updater.TryUpdateModelAsync(viewModel, prefix);
        return viewModel;
    }
}
