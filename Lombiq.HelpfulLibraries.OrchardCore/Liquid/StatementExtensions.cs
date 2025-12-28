using Cysharp.Text;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Fluid.Ast;

public static class StatementExtensions
{
    public static async Task<string?> RenderAsync(
        this IReadOnlyList<Statement> statements,
        TextEncoder encoder,
        TemplateContext context)
    {
        if (statements is not { Count: > 0 }) return null;

        await using var writer = new ZStringWriter();
        var completion = await statements.RenderStatementsAsync(writer, encoder, context);

        return completion == Completion.Normal ? writer.ToString().Trim() : null;
    }
}
