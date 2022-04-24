using Lombiq.HelpfulLibraries.Common.Utilities;
using System.Globalization;
using System.Threading.Tasks;

namespace System.IO;

public static class IoExtensions
{
    /// <summary>
    /// Concatenates the provided <paramref name="lineFragments"/> to the <paramref name="writer"/> using invariant
    /// culture.
    /// </summary>
    public static void WriteLineInvariant(this TextWriter writer, params FormattableString[] lineFragments)
    {
        foreach (var fragment in lineFragments)
        {
            writer.Write(fragment.ToString(CultureInfo.InvariantCulture));
        }

        writer.WriteLine();
    }

    /// <summary>
    /// Concatenates the provided <paramref name="lineFragments"/> to the <paramref name="writer"/> using invariant
    /// culture.
    /// </summary>
    public static Task WriteLineInvariantAsync(this TextWriter writer, params FormattableString[] lineFragments) =>
        writer.WriteLineAsync(StringHelper.Concatenate(lineFragments));
}
