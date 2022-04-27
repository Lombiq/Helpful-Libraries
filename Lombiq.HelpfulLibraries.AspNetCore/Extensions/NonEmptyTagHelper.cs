using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Lombiq.HelpfulLibraries.Libraries.AspNetCore;

/// <summary>
/// An attribute tag helper that conditionally hides its element if the provided <see cref="ICollection"/> is <see
/// langword="null"/> or empty.
/// </summary>
/// <example>
/// <para>
/// Instead of this:
/// </para>
/// <code>
/// @if(Model.Items?.Count > 0)
/// {
///     &lt;article&gt;
///         &lt;p&gt;Some other stuff.&lt;/p&gt;
///         &lt;ul&gt;
///             @foreach (var item in Model.Items)
///             {
///                 &lt;li&gt;@item&lt;/li&gt;
///             }
///         &lt;/ul&gt;
///     &lt;/article&gt;
/// }
/// </code>
/// <para>
/// You can write this:
/// </para>
/// <code>
/// &lt;article if-not-empty="@Model.Items"&gt;
///     &lt;p&gt;Some other stuff.&lt;/p&gt;
///     &lt;ul&gt;
///         @foreach (var item in Model.Items)
///         {
///             &lt;li&gt;@item&lt;/li&gt;
///         }
///     &lt;/ul&gt;
/// &lt;/article&gt;
/// </code>
/// </example>
/// <remarks>
/// <para>
/// Make sure to include <c>@addTagHelper *, Lombiq.HelpfulLibraries</c> in your _ViewImports.cshtml file.
/// </para>
/// </remarks>
[HtmlTargetElement("*", Attributes = "if-not-empty")]
public class NonEmptyTagHelper : TagHelper
{
    [HtmlAttributeName("if-not-empty")]
    [SuppressMessage(
        "Usage",
        "CA2227:Collection properties should be read only",
        Justification = "TagHelper needs the direct access.")]
    public ICollection IfNotEmpty { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (IfNotEmpty?.Count < 1) output.SuppressOutput();
    }
}
