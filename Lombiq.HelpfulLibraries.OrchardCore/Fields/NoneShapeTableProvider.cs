using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.DisplayManagement.Descriptors;
using OrchardCore.DisplayManagement.Implementation;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Fields;

/// <summary>
/// This provider creates a "None" editor and display type for every field which can be used to hide the field from
/// being editable or displayable respectively.
/// </summary>
public class NoneShapeTableProvider : IShapeTableProvider
{
    public const string None = nameof(None);

    private readonly IContentDefinitionManager _contentDefinitionManager;

    public NoneShapeTableProvider(IContentDefinitionManager contentDefinitionManager) =>
        _contentDefinitionManager = contentDefinitionManager;

    public void Discover(ShapeTableBuilder builder)
    {
        var allFieldNames = _contentDefinitionManager
            .ListPartDefinitions()
            .SelectMany(part => part.Fields)
            .Select(field => field.FieldDefinition.Name)
            .Distinct();

        foreach (var fieldName in allFieldNames)
        {
            Describe(builder, fieldName, isEditor: true);
            Describe(builder, fieldName, isEditor: false);
        }
    }

    private static void Describe(ShapeTableBuilder builder, string fieldName, bool isEditor)
    {
        // Creates a new editor or display mode entry in the field editor's dropdown menu.
        var property = isEditor ? "Editor" : "DisplayMode";
        BindShape(
            builder,
            $"{fieldName}_{(isEditor ? "Option" : "DisplayOption")}__{None}",
            context =>
            {
                var text = context
                    .ServiceProvider
                    .GetRequiredService<IStringLocalizer<NoneShapeTableProvider>>()["None"]
                    .Value;

                var selected = context.Value.Properties[property]?.ToString() == None
                    ? "selected"
                    : string.Empty;

                return Task.FromResult<IHtmlContent>(new HtmlString($"<option value=\"{None}\" {selected}>{text}</option>"));
            });

        // Creates a blank template for the actual edit or display shape.
        BindShape(
            builder,
            $"{fieldName}_{(isEditor ? "Display" : "Edit")}__{None}",
            _ => Task.FromResult<IHtmlContent>(new HtmlContentBuilder()));
    }

    private static void BindShape(
        ShapeTableBuilder builder,
        string shapeName,
        Func<DisplayContext, Task<IHtmlContent>> bindingAsync) =>
        builder
            .Describe(shapeName)
            .Configure(descriptor => descriptor.Bindings[shapeName] = new ShapeBinding { BindingAsync = bindingAsync, });
}
