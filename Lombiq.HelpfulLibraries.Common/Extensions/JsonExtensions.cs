using System.Text.Json.Nodes;

namespace System.Text.Json;

public static class JsonExtensions
{
    /// <summary>
    /// Attempts to parse the provided <paramref name="jsonNode"/>. If successful, returns <see langword="true"/> and
    /// <paramref name="result"/> contains the serialized object. Otherwise returns <see langword="false"/> and
    /// <paramref name="result"/> contains <see langword="default"/> of <typeparamref name="T"/>.
    /// </summary>
    public static bool TryParse<T>(this JsonNode jsonNode, out T result)
    {
        try
        {
            result = jsonNode.Deserialize<T>();
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }

    /// <summary>
    /// Converts the provided <paramref name="node"/> into the appropriate primitive types of <see langword="string"/>,
    /// <see langword="decimal"/>, <see langword="bool"/> or <see langword="null"/>. If the <paramref name="node"/> is
    /// array or object then it's serialized into JSON <see langword="string"/>.
    /// </summary>
    public static IComparable ToComparable(this JsonNode node) =>
        node.GetValueKind() switch
        {
            JsonValueKind.String => node.GetValue<string>(),
            JsonValueKind.Number => node.GetValue<decimal>(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Object => node.ToString(),
            JsonValueKind.Array => node.ToString(),
            _ => null,
        };
}
