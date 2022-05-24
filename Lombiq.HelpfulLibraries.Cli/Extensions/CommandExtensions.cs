using CliWrap.Builders;
using CliWrap.EventStream;
using System;
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
        Action<StandardErrorCommandEvent> stdErrHandler = default,
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
}
