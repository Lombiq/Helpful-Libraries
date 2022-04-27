using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Lombiq.HelpfulLibraries.Common.Utilities;

public static class Sha256Helper
{
    private static readonly Lazy<string> EmptyLazy = new(() => ComputeHash(string.Empty));

    /// <summary>
    /// Calculates the SHA-256 strong (cryptographic) hash from the specified <paramref name="text"/> string.
    /// </summary>
    /// <param name="text">The text that is decoded into a hashable byte array using <see cref="Encoding.UTF8"/>.</param>
    /// <returns>The hexadecimal string representation of the SHA-256 hash.</returns>
    public static string ComputeHash(string text)
    {
        using var sha256 = SHA256.Create();
        var hashedIdBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));

        var stringBuilder = new StringBuilder();

        foreach (var hashedIdByte in hashedIdBytes)
        {
            stringBuilder.Append(hashedIdByte.ToString("x2", CultureInfo.InvariantCulture));
        }

        return stringBuilder.ToString();
    }

    public static string Empty() => EmptyLazy.Value;
}
