using Fluid;
using Fluid.Ast;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Liquid;

/// <summary>
/// Describes a Liquid parser block's renderer method. This is used in <see
/// cref="LiquidServiceCollectionExtensions.AddLiquidParserBlock{T}"/> to add a custom <c>{% block_name %}</c>.
/// </summary>
public interface ILiquidParserBlock
{
    /// <summary>
    /// Renders the output of the parser block.
    /// </summary>
    ValueTask<Completion> WriteToAsync(
        IReadOnlyList<FilterArgument> argumentsList,
        IReadOnlyList<Statement> statements,
        TextWriter writer,
        TextEncoder encoder,
        TemplateContext context);
}
