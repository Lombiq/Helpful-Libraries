using System.Threading.Tasks;
using OrchardCore.DisplayManagement;

namespace Lombiq.HelpfulLibraries.Libraries.Shapes;

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
