#nullable enable

using Microsoft.AspNetCore.Html;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Html;
using OrchardCore.DisplayManagement.Shapes;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.ResourceManagement;

public class HtmlShape : IHtmlContent, IPositioned, IShape
{
    private static readonly IDictionary<string, string> _dummyAttributes = new Dictionary<string, string>().ToFrozenDictionary();
    private static readonly IDictionary<string, object> _dummyProperties = new Dictionary<string, object>().ToFrozenDictionary();

    private readonly List<IHtmlContent> _beforeContent = [];
    private readonly Func<IHtmlContent?> _getHtml;
    private readonly List<IHtmlContent> _afterContent = [];

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

    public void WriteTo(TextWriter writer, HtmlEncoder encoder)
    {
        foreach (var before in _beforeContent)
        {
            before.WriteTo(writer, encoder);
        }

        _getHtml.Invoke()?.WriteTo(writer, encoder);

        foreach (var after in _afterContent)
        {
            after.WriteTo(writer, encoder);
        }
    }

    public ValueTask<IShape> AddAsync(object? item, string? position)
    {
        if (item is null) return ValueTask.FromResult<IShape>(this);

        var content = item as IHtmlContent ?? new HtmlContentString(item.ToString());
        var target = position?.ContainsOrdinalIgnoreCase("before") == true ? _beforeContent : _afterContent;

        target.Add(content);
        return ValueTask.FromResult<IShape>(this);
    }
}
