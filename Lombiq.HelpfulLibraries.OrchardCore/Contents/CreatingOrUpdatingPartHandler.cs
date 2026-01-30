using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Contents;

/// <summary>
/// Abstraction over <see cref="ContentPartHandler{TPart}"/>, to assign a common event handler for
/// both <see cref="IContentPartHandler.CreatingAsync"/> and <see
/// cref="IContentPartHandler.UpdatingAsync"/> events. This replicates the behavior of handlers
/// before OC 3.0 that only used the latter.
/// </summary>
public abstract class CreatingOrUpdatingPartHandler<TPart> : ContentPartHandler<TPart>
    where TPart : ContentPart, new()
{
    protected abstract Task CreatingOrUpdatingAsync(TPart part);

    public override Task CreatingAsync(CreateContentContext context, TPart part) =>
        CreatingOrUpdatingAsync(part);

    public override Task UpdatingAsync(UpdateContentContext context, TPart part) =>
        CreatingOrUpdatingAsync(part);
}
