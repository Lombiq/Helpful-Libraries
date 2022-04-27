using OrchardCore.DisplayManagement;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Shapes;

public class ShapeRenderer : IShapeRenderer
{
    private readonly IDisplayHelper _displayHelper;
    private readonly HtmlEncoder _htmlEncoder;

    public ShapeRenderer(IDisplayHelper displayHelper, HtmlEncoder htmlEncoder)
    {
        _displayHelper = displayHelper;
        _htmlEncoder = htmlEncoder;
    }

    public async Task<string> RenderAsync(IShape shape)
    {
        await using var stringWriter = new StringWriter();
        var htmlContent = await _displayHelper.ShapeExecuteAsync(shape);
        htmlContent.WriteTo(stringWriter, _htmlEncoder);
        return stringWriter.ToString();
    }
}
