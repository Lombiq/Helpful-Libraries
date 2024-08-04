using Fluid;
using Fluid.Ast;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Liquid;

/// <summary>
/// Describes a Liquid parser tag's parser and render methods.
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
