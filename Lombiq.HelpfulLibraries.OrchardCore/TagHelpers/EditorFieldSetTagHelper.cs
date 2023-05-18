using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.TagHelpers;

[HtmlTargetElement("fieldset", Attributes = "asp-for")]
public class EditorFieldSetTagHelper : TagHelper
{
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

    public EditorFieldSetTagHelper(IHtmlGenerator htmlGenerator) =>
        _htmlGenerator = htmlGenerator;

    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (output.Attributes.TryGetAttribute("class", out var classAttribute))
        {
            var newValue = $"{classAttribute.Value} form-group";
            output.Attributes.Remove(classAttribute);
            output.Attributes.Add("class", newValue);
        }
        else
        {
            output.Attributes.Add("class", "form-group mb-3 col-xl-6");
        }

        var isCheckbox = InputType.EqualsOrdinalIgnoreCase("checkbox");

        if (isCheckbox) output.Content.AppendHtml("<div class=\"custom-control custom-checkbox\">");

        var label = _htmlGenerator.GenerateLabel(ViewContext, For.ModelExplorer, For.Name, Label.Html(), htmlAttributes: null);

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

            label.Attributes["class"] = "custom-control-label";

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
}
