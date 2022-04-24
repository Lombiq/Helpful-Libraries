using OrchardCore.DisplayManagement;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Shapes;

/// <summary>
/// Renders shapes to <c>string</c>.
/// </summary>
public interface IShapeRenderer
{
    /// <summary>
    /// Renders shapes to <see langword="string"/>.
    /// </summary>
    Task<string> RenderAsync(IShape shape);
}
