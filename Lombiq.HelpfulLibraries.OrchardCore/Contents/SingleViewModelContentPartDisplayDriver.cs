using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrchardCore.ContentManagement.Display.ContentDisplay;

/// <summary>
/// A version of <see cref="ContentPartDisplayDriver{TPart}"/> where there is only one type of viewmodel used so the
/// <see cref="IUpdateModel.TryUpdateModelAsync{TModel}(TModel)"/> logic can be extracted in update.
/// </summary>
/// <typeparam name="TPart">The <see cref="ContentPart"/> to be edited.</typeparam>
/// <typeparam name="TViewModel">The designated viewmodel.</typeparam>
public abstract class SingleViewModelContentPartDisplayDriver<TPart, TViewModel> : ContentPartDisplayDriver<TPart>
    where TPart : ContentPart, new()
    where TViewModel : class, new()
{
    /// <summary>
    /// Performs the update activity and returns any model state errors.
    /// </summary>
    /// <returns>A collection of custom validation errors, or <see langword="null"/>.</returns>
    protected abstract Task<IEnumerable<(string Key, string Error)>> UpdateAsync(
        TPart part,
        TViewModel viewModel,
        UpdatePartEditorContext context);

    public override async Task<IDisplayResult> UpdateAsync(
        TPart part,
        IUpdateModel updater,
        UpdatePartEditorContext context)
    {
        var viewModel = new TViewModel();
        if (await updater.TryUpdateModelAsync(viewModel, Prefix) &&
            await UpdateAsync(part, viewModel, context) is { } updateResults)
        {
            foreach (var (key, error) in updateResults)
            {
                updater.ModelState.AddModelError(key, error);
            }
        }

        return await EditAsync(part, context);
    }
}
