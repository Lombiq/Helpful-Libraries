using CliWrap;
using CliWrap.Buffered;
using Lombiq.HelpfulLibraries.Cli.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.Cli;

public class CliProgram
{
    public static CliProgram DotNet { get; } = new("dotnet");

    private readonly string _command;

    public CliProgram(string command) => _command = command + OperatingSystemHelper.GetExecutableExtension();

    public Command GetCommand(IEnumerable<object> arguments) =>
        CliWrap.Cli
            .Wrap(_command)
            .WithArguments(arguments.Select(argument => argument is IConvertible convertible
                ? convertible.ToString(CultureInfo.InvariantCulture)
                : argument.ToString()));

    /// <summary>
    /// Calls the command specified in the constructor with the provided arguments. If the process doesn't succeed or
    /// outputs to the standard error stream then an exception is thrown.
    /// </summary>
    /// <param name="arguments">
    /// The arguments passed to the command. If an item is not a <see langword="string"/>, then it's converted using the
    /// <see cref="CultureInfo.InvariantCulture"/> formatter.
    /// </param>
    /// <param name="additionalExceptionText">
    /// Text in the second line of the exception message after the standard "... command failed with the output below."
    /// text. If it's <see langword="null"/>, then the line is omitted.
    /// </param>
    /// <param name="token">Passed into the CliWrap <see cref="Command"/>.</param>
    /// <exception cref="InvalidOperationException">
    /// <para>Thrown if the command fails or outputs to the error stream. The format is like this:</para>
    /// <code>
    /// The {command} {arguments} command failed with the output below.
    /// {additional exception text}
    /// {standard error output}
    /// </code>
    /// </exception>
    public async Task ExecuteAsync(
        ICollection<object> arguments,
        string additionalExceptionText,
        CancellationToken token)
    {
        var result = await GetCommand(arguments)
            .WithValidation(CommandResultValidation.None)
            .ExecuteBufferedAsync(token);

        if (result.ExitCode != 0 || !string.IsNullOrEmpty(result.StandardError))
        {
            var argumentsString = arguments
                .Select(argument => argument is string argumentString && argumentString.Contains(' ')
                    ? $"\"{argument}\""
                    : argument.ToString())
                .Join();

            var lines = new[]
            {
                $"The {_command} {argumentsString} command failed with the output below.",
                additionalExceptionText,
                result.StandardOutput,
                result.StandardError,
            };

            throw new InvalidOperationException(lines.JoinNotNullOrEmpty(Environment.NewLine));
        }
    }

    /// <summary>
    /// Shortcut for <see cref="ExecuteAsync(ICollection{object},string,CancellationToken)"/> if there is no additional
    /// exception message.
    /// </summary>
    /// <param name="token">Passed into the CliWrap <see cref="Command"/>.</param>
    /// <param name="arguments">
    /// The arguments passed to the command. If an item is not a <see langword="string"/>, then it's converted using the
    /// <see cref="CultureInfo.InvariantCulture"/> formatter.
    /// </param>
    public Task ExecuteAsync(CancellationToken token, params object[] arguments) =>
        ExecuteAsync(arguments, additionalExceptionText: null, token);
}
