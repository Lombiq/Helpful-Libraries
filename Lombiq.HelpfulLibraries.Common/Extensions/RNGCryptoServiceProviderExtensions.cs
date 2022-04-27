namespace System.Security.Cryptography;

// The extension method should follow the naming of the original class.
#pragma warning disable S101 // Types should be named in PascalCase

public static class RNGCryptoServiceProviderExtensions
#pragma warning restore S101 // Types should be named in PascalCase
{
    /// <summary>
    /// Returns a non-negative cryptographically secure random integer that is within a specified range.
    /// </summary>
    /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
    /// <param name="maxValue">
    /// The exclusive upper bound of the random number returned. <paramref name="maxValue"/> must be greater than or
    /// equal to <paramref name="minValue"/>.
    /// </param>
    /// <returns>A cryptographically random number within the specified range.</returns>
    /// <remarks>
    /// <para>
    /// Taken from <see
    /// href="https://docs.microsoft.com/en-us/archive/msdn-magazine/2007/september/net-matters-tales-from-the-cryptorandom"/>.
    /// </para>
    /// </remarks>
    [Obsolete("Since RNGCryptoServiceProvider is obsolete, use the similar extension on RandomNumberGenerator instead.")]
    public static int Next(this RNGCryptoServiceProvider rng, int minValue, int maxValue)
    {
        if (minValue > maxValue) throw new ArgumentOutOfRangeException(nameof(minValue));
        if (minValue == maxValue) return minValue;

        var diff = (long)maxValue - minValue;
        var uint32Buffer = new byte[4];

        while (true)
        {
            rng.GetBytes(uint32Buffer);
            var rand = BitConverter.ToUInt32(uint32Buffer, 0);

            const long max = 1 + (long)uint.MaxValue;
            var remainder = max % diff;
            if (rand < max - remainder)
            {
                return (int)(minValue + (rand % diff));
            }
        }
    }
}
