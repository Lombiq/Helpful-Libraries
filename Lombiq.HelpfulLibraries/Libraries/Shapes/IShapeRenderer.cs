using System.Threading.Tasks;
using OrchardCore.DisplayManagement;

namespace Lombiq.HelpfulLibraries.Libraries.Shapes
{
    /// <summary>
    /// Renders shapes to <c>string</c>.
    /// </summary>
    public interface IShapeRenderer
    {
        /// <summary>
        /// Renders the given shape to <c>string</c>.
        /// </summary>
        /// <param name="shape">Shape to render.</param>
        /// <returns>Rendered shape.</returns>
        Task<string> RenderAsync(IShape shape);
    }
}
