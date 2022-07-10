using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.Cli.Helpers;

public static class DotnetBuildHelper
{
    /// <summary>
    /// Executes <c>dotnet build $SolutionFileName$ --no-incremental --nologo --warnaserror
    /// --consoleLoggerParameters:NoSummary --verbosity:quiet -p:TreatWarningsAsErrors=true
    /// -p:RunAnalyzersDuringBuild=true</c> To dynamically run static code analysis.
    /// </summary>
    /// <param name="solutionPath">The path to the sln file.</param>
    /// <param name="additionalArguments">Further command line arguments.</param>
    /// <remarks>
    /// <para>
    /// See https://github.com/Lombiq/.NET-Analyzers/blob/dev/Docs/UsingAnalyzersDuringCommandLineBuilds.md#net-code-style-analysis
    /// for more information.
    /// </para>
    /// </remarks>
    public static Task ExecuteStaticCodeAnalysisAsync(string solutionPath, params string[] additionalArguments)
    {
        var relativeSolutionPath = Path.Combine("..", "..", "..", "..", "TestSolutions", solutionPath);

        var arguments = new List<object>
        {
            "build",
            relativeSolutionPath,
            "--no-incremental",
            "--nologo",
            "--warnaserror",
            "--consoleLoggerParameters:NoSummary",
            "--verbosity:quiet",
            "-p:TreatWarningsAsErrors=true",
            "-p:RunAnalyzersDuringBuild=true",
        };
        arguments.AddRange(additionalArguments);

        return CliProgram.DotNet.ExecuteAsync(CancellationToken.None, arguments.ToArray());
    }
}
