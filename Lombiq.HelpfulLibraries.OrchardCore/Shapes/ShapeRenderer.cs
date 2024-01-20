using OrchardCore.DisplayManagement;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Shapes;

public class ShapeRenderer(IDisplayHelper displayHelper, HtmlEncoder htmlEncoder) : IShapeRenderer
{
    public async Task<string> RenderAsync(IShape shape)
    {
        await using var stringWriter = new StringWriter();
        var htmlContent = await displayHelper.ShapeExecuteAsync(shape);
        htmlContent.WriteTo(stringWriter, htmlEncoder);
        return stringWriter.ToString();
    }
}
