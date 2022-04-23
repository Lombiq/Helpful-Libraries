using System;
using System.Diagnostics.CodeAnalysis;

namespace Lombiq.HelpfulLibraries.Common.Utilities;

/// <summary>
/// A wrapper around <see cref="Random"/> which emphasizes that it is not for any security critical purpose. By using
/// this class the user acknowledges that, which means manually disabling the <see
/// href="https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca5394#when-to-suppress-warnings">
/// CA5394</see> warning is no longer necessary.
/// </summary>
[SuppressMessage(
    "Microsoft.Security",
    "CA5394",
    Justification = "The name makes it explicit that it's not a security concern.")]
[SuppressMessage(
    "Security",
    "SCS0005:Weak random number generator.",
    Justification = "The name makes it explicit that it's not a security concern.")]
public class NonSecurityRandomizer
{
    private readonly Random _random;

    public NonSecurityRandomizer(int seed) => _random = new Random(seed);

    public NonSecurityRandomizer() => _random = new Random();

    /// <summary>
    /// Returns a random <see cref="int"/> that is at least 0 and lower than <paramref name="below"/>.
    /// </summary>
    public int GetFromRange(int below) => _random.Next(below);

    /// <summary>
    /// Returns a random <see cref="int"/> between 0 and <see cref="int.MaxValue"/> inclusive.
    /// </summary>
    public int Get() => _random.Next();

    /// <summary>
    /// Returns a random <see cref="double"/> between 0 and <see cref="double.MaxValue"/> inclusive.
    /// </summary>
    public double GetDouble() => _random.NextDouble();
}
