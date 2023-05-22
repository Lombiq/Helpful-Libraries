using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.TagHelpers;

[HtmlTargetElement("fieldset", Attributes = "asp-for")]
public class EditorFieldSetTagHelper : TagHelper
{
    private const string Class = "class";

    private readonly IHtmlGenerator _htmlGenerator;

    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; }

    [HtmlAttributeName("asp-for")]
    public ModelExpression For { get; set; }

    [HtmlAttributeName("label")]
    public LocalizedHtmlString Label { get; set; }

    [HtmlAttributeName("hint")]
    public LocalizedHtmlString Hint { get; set; }

    [HtmlAttributeName("type")]
    public string InputType { get; set; } = "text";

    [HtmlAttributeName("required")]
    public bool IsRequired { get; set; }

    public EditorFieldSetTagHelper(IHtmlGenerator htmlGenerator) =>
        _htmlGenerator = htmlGenerator;

    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (output.Attributes.TryGetAttribute(Class, out var classAttribute))
        {
            var newValue = $"{classAttribute.Value} form-group";
            output.Attributes.Remove(classAttribute);
            output.Attributes.Add(Class, newValue);
        }
        else
        {
            output.Attributes.Add(Class, "form-group mb-3 col-xl-6");
        }

        var isRequired = IsRequired || HasRequiredAttribute(For);
        var isCheckbox = InputType.EqualsOrdinalIgnoreCase("checkbox");

        if (isCheckbox) output.Content.AppendHtml("<div class=\"custom-control custom-checkbox\">");

        var label = _htmlGenerator.GenerateLabel(
            ViewContext,
            For.ModelExplorer,
            For.Name,
            Label.Html().Trim() + (isRequired ? " *" : string.Empty),
            htmlAttributes: null);

        if (isCheckbox)
        {
            var input = _htmlGenerator.GenerateCheckBox(
                ViewContext,
                For.ModelExplorer,
                For.Name,
                For.Model switch
                {
                    null => null,
                    bool value => value,
                    _ => bool.TryParse(For.Model.ToString(), out var parsedValue) ? parsedValue : null,
                },
                new { @class = "custom-control-input" });

            if (isRequired)
            {
                input.Attributes.Add("required", "required");
            }

            label.Attributes[Class] = "custom-control-label";

            AppendContent(output, input);
            output.Content.AppendHtml("&nbsp;");
            AppendContent(output, label);
        }
        else
        {
            var input = _htmlGenerator.GenerateTextBox(
                ViewContext,
                For.ModelExplorer,
                For.Name,
                For.Model,
                For.ModelExplorer.Metadata.EditFormatString,
                new { @class = "form-control", type = InputType });

            if (isRequired)
            {
                input.Attributes.Add("required", "required");
            }

            AppendContent(output, label);
            AppendContent(output, input);
        }

        if (InputType.EqualsOrdinalIgnoreCase("checkbox")) output.Content.AppendHtml("</div>");

        var tagBuilder = _htmlGenerator.GenerateValidationMessage(
            ViewContext,
            For.ModelExplorer,
            For.Name,
            message: null,
            tag: "span",
            htmlAttributes: null);
        AppendContent(output, tagBuilder);

        AppendContent(output, Hint);

        return Task.CompletedTask;
    }

    private static void AppendContent(TagHelperOutput output, IHtmlContent content)
    {
        if (content != null) output.Content.AppendHtml(content.Html());
    }

    private static bool HasRequiredAttribute(ModelExpression modelExpression) =>
        modelExpression
            .Metadata
            .ContainerType?
            .GetProperty(modelExpression.Name)?
            .GetCustomAttributes(typeof(RequiredAttribute), inherit: false)
            .FirstOrDefault() is RequiredAttribute;
}
