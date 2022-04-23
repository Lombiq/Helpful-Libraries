namespace Microsoft.Extensions.Caching.Memory;

public static class MemoryCacheExtensions
{
    /// <summary>
    /// Returns the value referenced by <paramref name="key"/> if it exists and it's of <typeparamref name="T"/>.
    /// Otherwise returns a new instance without saving it into the cache. Never <see langword="null"/>.
    /// </summary>
    public static T GetOrNew<T>(this IMemoryCache memoryCache, string key)
        where T : new() =>
        memoryCache.TryGetValue(key, out var valueObject) &&
        valueObject is T valueOfType
            ? valueOfType
            : new T();
}
