using System.ComponentModel;

namespace Lombiq.HelpfulLibraries.OrchardCore.Contents;

/// <summary>
/// Collection of different ContentField editors.
/// Feel free to extend this class by adding more Enum classes with ContentField editors.
/// </summary>
public static class ContentFieldEditorEnums
{
    [Description("Editor options for HtmlFields.")]
    public enum HtmlFieldEditors
    {
        Monaco,
        Multiline,
        Trumbowyg,
        Wysiwyg,
    }

    [Description("Editor options for MarkdownFields. Wysiwyg editor can be used for MarkdownBodyParts aswell.")]
    public enum MarkdownFieldEditors
    {
        TextArea,
        Wysiwyg
    }

    [Description("Editor options for TaxonomyFields.")]
    public enum TaxonomyFieldEditors
    {
        Tags,
    }

    [Description("Editor options for TextFields.")]
    public enum TextFieldEditors
    {
        CodeMirror,
        Color,
        Email,
        IconPicker,
        Monaco,
        PredefinedList,
        Tel,
        TextArea,
        Url,
    }
}
