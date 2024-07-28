#nullable enable

using Microsoft.AspNetCore.Html;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Html;
using OrchardCore.DisplayManagement.Shapes;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement;

public class HtmlShape : IHtmlContent, IPositioned, IShape
{
    private static readonly IDictionary<string, string> _dummyAttributes = new Dictionary<string, string>().ToFrozenDictionary();
    private static readonly IDictionary<string, object> _dummyProperties = new Dictionary<string, object>().ToFrozenDictionary();

    private readonly Func<IHtmlContent?> _getHtml;

    public string? Position { get; set; }

    public ShapeMetadata Metadata { get; set; } = new();

    public string? Id { get; set; }

    public string? TagName { get; set; }

    public IList<string> Classes => [];

    public IDictionary<string, string> Attributes => _dummyAttributes;

    public IDictionary<string, object> Properties => _dummyProperties;

    public IReadOnlyList<IPositioned> Items => [];

    public HtmlShape(Func<IHtmlContent?> getHtml, string? position = null)
    {
        _getHtml = getHtml;
        Position = position;
    }

    public HtmlShape(IHtmlContent? value, string? position = null)
        : this(() => value, position)
    {
    }

    public HtmlShape(string? value, string? position = null)
        : this(new HtmlContentString(value ?? string.Empty), position)
    {
    }

    public void WriteTo(TextWriter writer, HtmlEncoder encoder) => _getHtml.Invoke()?.WriteTo(writer, encoder);

    public ValueTask<IShape> AddAsync(object item, string position) => throw new ReadOnlyException();
}
