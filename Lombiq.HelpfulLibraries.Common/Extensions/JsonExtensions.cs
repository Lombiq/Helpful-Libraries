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
}
