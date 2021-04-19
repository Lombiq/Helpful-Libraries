using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Lombiq.HelpfulLibraries.Libraries.Utilities
{
    public static class Sha256Helper
    {
        public static string ComputeHash(string text)
        {
            using var sha256 = new SHA256Managed();
            var hashedIdBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));

            var stringBuilder = new StringBuilder();

            foreach (var hashedIdByte in hashedIdBytes)
            {
                stringBuilder.Append(hashedIdByte.ToString("x2", CultureInfo.InvariantCulture));
            }

            return stringBuilder.ToString();
        }
    }
}
