using System;
using System.Threading.Tasks;

namespace OrchardCore.DisplayManagement.Handlers;

public static class BuildEditorContextExtensions
{
    public static async Task<T> CreateModelMaybeAsync<T>(
        this BuildEditorContext context,
        string prefix,
        string groupId = null,
        Func<Task<bool>> authorizeAsync = null)
        where T : class, new()
    {
        if (!string.IsNullOrEmpty(groupId) && context.GroupId != groupId) return null;

        if (authorizeAsync != null && !await authorizeAsync())
        {
            return null;
        }

        var viewModel = new T();
        return await context.Updater.TryUpdateModelAsync(viewModel, prefix)
            ? viewModel
            : null;
    }
}
