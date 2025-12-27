using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Lombiq.HelpfulLibraries.OrchardCore.Users;

public static class PasswordHelper
{
    /// <summary>
    /// Generates a <paramref name="minLength"/> long random password.
    /// </summary>
    public static string GenerateRandomPassword(int minLength)
    {
        const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+-=";

        using var rng = RandomNumberGenerator.Create();
        const string digits = "0123456789";
        const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
        const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string specialChars = "!@#$%^&*()_+-=";

        var passwordChars = new List<char>
            {
                digits[rng.Next(0, digits.Length)],
                lowerChars[rng.Next(0, lowerChars.Length)],
                upperChars[rng.Next(0, upperChars.Length)],
                specialChars[rng.Next(0, specialChars.Length)],
            };

        while (passwordChars.Count < minLength)
        {
            passwordChars.Add(validChars[rng.Next(0, validChars.Length)]);
        }

        passwordChars = [.. passwordChars.OrderBy(_ => rng.Next(0, int.MaxValue))];
        string password = new([.. passwordChars]);

        return password;
    }
}
