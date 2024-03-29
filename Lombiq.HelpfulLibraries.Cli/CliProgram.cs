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

    /// <summary>
    /// Creates a <see cref="Command"/> object based on the passed <paramref name="arguments"/>.
    /// </summary>
    public Command GetCommand(params object[] arguments) =>
        GetCommand(arguments.ToList());

    /// <summary>
    /// Creates a <see cref="Command"/> object based on the passed <paramref name="arguments"/>.
    /// </summary>
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
        var result = await GetResultAsync(arguments, token);
        ThrowOnError(result, arguments, additionalExceptionText);
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

    /// <summary>
    /// Calls the command specified in the constructor with the provided arguments, and returns the standard output as a
    /// string. If the process doesn't succeed or outputs to the standard error stream then an exception is thrown.
    /// </summary>
    /// <param name="token">Passed into the CliWrap <see cref="Command"/>.</param>
    /// <param name="arguments">
    /// The arguments passed to the command. If an item is not a <see langword="string"/>, then it's converted using the
    /// <see cref="CultureInfo.InvariantCulture"/> formatter.
    /// </param>
    /// <returns>The standard output as a string.</returns>
    /// <exception cref="InvalidOperationException">
    /// <para>Thrown if the command fails or outputs to the error stream. The format is like this:</para>
    /// <code>
    /// The {command} {arguments} command failed with the output below.
    /// {additional exception text}
    /// {standard error output}
    /// </code>
    /// </exception>
    public Task<string> ExecuteAndGetOutputAsync(CancellationToken token, params object[] arguments) =>
        ExecuteAndGetOutputAsync(arguments, additionalExceptionText: null, token);

    /// <summary>
    /// Calls the command specified in the constructor with the provided arguments, and returns the standard output as a
    /// string. If the process doesn't succeed or outputs to the standard error stream then an exception is thrown.
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
    /// <returns>The standard output as a string.</returns>
    /// <exception cref="InvalidOperationException">
    /// <para>Thrown if the command fails or outputs to the error stream. The format is like this:</para>
    /// <code>
    /// The {command} {arguments} command failed with the output below.
    /// {additional exception text}
    /// {standard error output}
    /// </code>
    /// </exception>
    public async Task<string> ExecuteAndGetOutputAsync(
        ICollection<object> arguments,
        string additionalExceptionText,
        CancellationToken token)
    {
        var result = await GetResultAsync(arguments, token);

        ThrowOnError(result, arguments, additionalExceptionText);

        return result.StandardOutput;
    }

    private async Task<BufferedCommandResult> GetResultAsync(ICollection<object> arguments, CancellationToken token) =>
        await GetCommand(arguments)
            .WithValidation(CommandResultValidation.None)
            .ExecuteBufferedAsync(token);

    private void ThrowOnError(
        BufferedCommandResult result,
        ICollection<object> arguments,
        string additionalExceptionText)
    {
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
}
