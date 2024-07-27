using Microsoft.AspNetCore.Html;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Html;
using OrchardCore.DisplayManagement.Shapes;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement;

// Based on OrchardCore.DisplayManagement.PositionWrapper.
public class HtmlShape : IHtmlContent, IPositioned, IShape
{
    private static readonly IDictionary<string, string> _dummyAttributes = new Dictionary<string, string>().ToFrozenDictionary();
    private static readonly IDictionary<string, object> _dummyProperties = new Dictionary<string, object>().ToFrozenDictionary();

    private readonly IHtmlContent _value;

    public string Position { get; set; }

    public ShapeMetadata Metadata { get; set; } = new();

    public string Id { get; set; }

    public string TagName { get; set; }

    public IList<string> Classes => [];

    public IDictionary<string, string> Attributes => _dummyAttributes;

    public IDictionary<string, object> Properties => _dummyProperties;

    public IReadOnlyList<IPositioned> Items => [];

    public HtmlShape(IHtmlContent value, string position)
    {
        _value = value;
        Position = position;
    }

    public HtmlShape(string value, string position)
        : this(new HtmlContentString(value), position)
    {
    }

    public void WriteTo(TextWriter writer, HtmlEncoder encoder) => _value.WriteTo(writer, encoder);

    public ValueTask<IShape> AddAsync(object item, string position) => throw new ReadOnlyException();
}
