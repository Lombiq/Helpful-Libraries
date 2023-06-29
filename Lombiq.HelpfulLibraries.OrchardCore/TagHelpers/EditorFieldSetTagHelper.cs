using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.TagHelpers;

[HtmlTargetElement("fieldset", Attributes = "asp-for,label")]
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

    [HtmlAttributeName("readonly")]
    public bool IsReadOnly { get; set; }

    [HtmlAttributeName("options")]
    public IEnumerable<SelectListItem> Options { get; set; }

    public EditorFieldSetTagHelper(IHtmlGenerator htmlGenerator) =>
        _htmlGenerator = htmlGenerator;

    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        const string fieldsetClasses = "form-group mb-3 col-xl-6";

        if (output.Attributes.TryGetAttribute(Class, out var classAttribute))
        {
            var newValue = $"{classAttribute.Value} {fieldsetClasses}";
            output.Attributes.Remove(classAttribute);
            output.Attributes.Add(Class, newValue);
        }
        else
        {
            output.Attributes.Add(Class, fieldsetClasses);
        }

        AppendInputAndLabel(output, IsRequired || HasRequiredAttribute(For));

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

    private void AppendInputAndLabel(TagHelperOutput output, bool isRequired)
    {
        var label = _htmlGenerator.GenerateLabel(
            ViewContext,
            For.ModelExplorer,
            For.Name,
            Label.Html().Trim() + (isRequired ? " *" : string.Empty),
            htmlAttributes: null);

        var attributes = new Dictionary<string, object>();
        if (IsReadOnly) attributes["readonly"] = "readonly";
        if (isRequired) attributes["required"] = "required";

        if (InputType.EqualsOrdinalIgnoreCase("checkbox"))
        {
            attributes["class"] = "custom-control-input";
            var checkbox = _htmlGenerator.GenerateCheckBox(
                ViewContext,
                For.ModelExplorer,
                For.Name,
                For.Model switch
                {
                    null => null,
                    bool value => value,
                    _ => bool.TryParse(For.Model.ToString(), out var parsedValue) ? parsedValue : null,
                },
                attributes);

            label.Attributes[Class] = "custom-control-label";

            output.Content.AppendHtml("<div class=\"custom-control custom-checkbox\">");
            AppendContent(output, checkbox);
            output.Content.AppendHtml("&nbsp;");
            AppendContent(output, label);
            output.Content.AppendHtml("</div>");

            return;
        }

        var inputType = InputType;
        if (Options != null) inputType = "select";

        attributes["class"] = "form-select";
        if (inputType != "select")
        {
            attributes["class"] = "form-control";
            attributes["type"] = InputType;
        }

        var input = inputType == "select"
            ? _htmlGenerator.GenerateSelect(
                ViewContext,
                For.ModelExplorer,
                string.Empty,
                For.Name,
                Options,
                allowMultiple: false,
                attributes)
            : _htmlGenerator.GenerateTextBox(
                ViewContext,
                For.ModelExplorer,
                For.Name,
                For.Model,
                For.ModelExplorer.Metadata.EditFormatString,
                attributes);

        AppendContent(output, label);
        AppendContent(output, input);
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
