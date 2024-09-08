#nullable enable

using CliWrap.Builders;
using CliWrap.EventStream;
using Lombiq.HelpfulLibraries.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CliWrap;

public static class CommandExtensions
{
    /// <summary>
    /// Executes a <see cref="Command"/> as a <c>dotnet</c> command that starts a long-running application, and waits
    /// for the app to be started.
    /// </summary>
    public static async Task ExecuteDotNetApplicationAsync(
        this Command command,
        Action<StandardErrorCommandEvent>? stdErrHandler = default,
        CancellationToken cancellationToken = default)
    {
        await using var enumerator = command.ListenAsync(cancellationToken).GetAsyncEnumerator(cancellationToken);

        while (await enumerator.MoveNextAsync())
        {
            if (enumerator.Current is StandardOutputCommandEvent stdOut &&
                stdOut.Text.ContainsOrdinalIgnoreCase("Application started. Press Ctrl+C to shut down."))
            {
                return;
            }

            if (enumerator.Current is StandardErrorCommandEvent stdErr)
            {
                stdErrHandler?.Invoke(stdErr);
            }
        }
    }

    /// <summary>
    /// Same as <see cref="Command.WithArguments(Action{ArgumentsBuilder})"/>, but the configuration delegate is
    /// asynchronous.
    /// </summary>
    public static async Task<Command> WithArgumentsAsync(this Command command, Func<ArgumentsBuilder, Task> configureAsync)
    {
        var builder = new ArgumentsBuilder();
        await configureAsync(builder);

        return command.WithArguments(builder.Build());
    }

    /// <summary>
    /// Same as <see cref="Command.WithEnvironmentVariables(IReadOnlyDictionary{string,string?})"/>, but the values can
    /// be any type. If a value is <see cref="IConvertible"/>, like all C# primitive types then <see
    /// cref="NumberExtensions.ToTechnicalString(IConvertible)"/> is called, otherwise <see cref="object.ToString"/>.
    /// </summary>
    public static Command WithEnvironmentVariables(this Command command, IEnumerable<KeyValuePair<string?, object?>> variables) =>
        command.WithEnvironmentVariables(variables
            .Where(pair => pair is { Key: { } })
            .ToDictionary(
                pair => pair.Key!,
                pair => pair.Value switch
                {
                    string text => text,
                    IConvertible convertible => convertible.ToTechnicalString(),
                    _ => pair.Value?.ToString(),
                }));
}
