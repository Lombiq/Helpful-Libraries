using CliWrap;
using CliWrap.Buffered;
using System.Globalization;

namespace Lombiq.HelpfulLibraries.Cli.Helpers;

public static class DotnetCli
{
    private static readonly string _dotnetExecutable = "dotnet" + OperatingSystemHelper.GetExecutableExtension();

    /// <summary>
    /// Calls the <c>dotnet</c> command with the provided arguments. If the process doesn't succeed or outputs to the
    /// standard error stream then an exception is thrown.
    /// </summary>
    /// <param name="arguments">
    /// The arguments passed to <c>dotnet</c>. If an item is not <see langword="string"/>, then it's converted using
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
    /// The dotnet {arguments} command failed with the output below.
    /// {additional exception text}
    /// {standard error output}
    /// </code>
    /// </exception>
    public static async Task CommandAsync(
        ICollection<object> arguments,
        string additionalExceptionText,
        CancellationToken token)
    {
        var result = await CliWrap.Cli
            .Wrap(_dotnetExecutable)
            .WithArguments(arguments.Select(argument => argument is IConvertible convertible
                ? convertible.ToString(CultureInfo.InvariantCulture)
                : argument.ToString()))
            .WithValidation(CommandResultValidation.None)
            .ExecuteBufferedAsync(token);

        if (result.ExitCode != 0 || !string.IsNullOrEmpty(result.StandardError))
        {
            var argumentsString = string.Join(
                separator: " ",
                arguments.Select(argument => argument is string argumentString && argumentString.Contains(' ')
                    ? $"\"{argument}\""
                    : argument));

            var lines = new[]
            {
                $"The dotnet {argumentsString} command failed with the output below.",
                additionalExceptionText,
                result.StandardError,
            };

            throw new InvalidOperationException(string.Join(Environment.NewLine, lines.Where(line => line != null)));
        }
    }

    /// <summary>
    /// Shortcut for <see cref="CommandAsync(System.Collections.Generic.ICollection{object},string,System.Threading.CancellationToken)"/>
    /// if there is no additional exception message.
    /// </summary>
    /// <param name="token">Passed into the CliWrap <see cref="Command"/>.</param>
    /// <param name="arguments">
    /// The arguments passed to <c>dotnet</c>. If an item is not <see langword="string"/>, then it's converted using
    /// <see cref="CultureInfo.InvariantCulture"/> formatter.
    /// </param>
    public static Task CommandAsync(CancellationToken token, params object[] arguments) =>
        CommandAsync(arguments, additionalExceptionText: null, token);
}
