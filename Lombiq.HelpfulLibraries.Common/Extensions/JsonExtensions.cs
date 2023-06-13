namespace Newtonsoft.Json.Linq;

public static class JsonExtensions
{
    public static bool TryParse<T>(this JObject jObject, out T result)
    {
        try
        {
            result = jObject.ToObject<T>();
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }
}
