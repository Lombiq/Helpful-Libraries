using Fluid;
using Fluid.Ast;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Liquid;

/// <summary>
/// Describes a Liquid parser tag's renderer method. This is used in <see
/// cref="LiquidServiceCollectionExtensions.AddLiquidParserTag{T}"/> to add a custom <c>{% tag_name %}</c> tag.
/// </summary>
public interface ILiquidParserTag
{
    /// <summary>
    /// Renders the output of the parser tag.
    /// </summary>
    ValueTask<Completion> WriteToAsync(
        IReadOnlyList<FilterArgument> argumentsList,
        TextWriter writer,
        TextEncoder encoder,
        TemplateContext context);
}
