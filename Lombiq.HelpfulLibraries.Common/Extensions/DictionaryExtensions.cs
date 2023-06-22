using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace System.Collections.Generic;

public static class DictionaryExtensions
{
    /// <summary>
    /// Safely returns the value by key if it's in the dictionary. If the key is <see langword="default"/> or not found
    /// in the dictionary, it'll return <see langword="default"/>.
    /// </summary>
    /// <param name="key">Key in the dictionary.</param>
    /// <typeparam name="TKey">Type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">Type of the values in the dictionary.</typeparam>
    /// <returns>Value identified by the key if it's in the dictionary.</returns>
    public static TValue GetMaybe<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) =>
        !Equals(key, default) && dictionary.TryGetValue(key, out var value) ? value : default;

    /// <summary>
    /// Safely returns the value by key converted to the given type if it's in the dictionary. It'll return <see
    /// langword="default"/> if the key is not present in the dictionary or if the value can't be converted.
    /// </summary>
    /// <param name="key">Key in the dictionary.</param>
    /// <typeparam name="TValue">Type to convert to.</typeparam>
    /// <returns>Value identified by the key if it's in the dictionary.</returns>
    public static TValue GetMaybe<TValue>(this IDictionary<object, object> dictionary, object key) =>
        GetMaybe(dictionary, key) is TValue value ? value : default;

    /// <summary>
    /// Safely returns the value by key if it's in the dictionary. If the key is <see langword="default"/> or not found
    /// in the dictionary, it'll return <see langword="default"/>.
    /// </summary>
    /// <param name="key">Key in the dictionary.</param>
    /// <typeparam name="TKey">Type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">Type of the values in the dictionary.</typeparam>
    /// <returns>Value identified by the key if it's in the dictionary.</returns>
    public static TValue GetMaybeReadOnly<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key) =>
        !Equals(key, default) && dictionary.TryGetValue(key, out var value) ? value : default;

    /// <summary>
    /// Returns values from the dictionary identified by the given keys. In case of missing items it will also add these
    /// in one batch. Could be used as a simple memory cache where the items are fetched from the database in one query
    /// for example.
    /// </summary>
    /// <param name="keys">List of keys in the dictionary.</param>
    /// <param name="valuesFactory">
    /// Operation returning the missing items. This will always be executed using the missing keys only.
    /// </param>
    /// <param name="keySelector">
    /// Determines the new key for the dictionary by the missing item. The key must be unique.
    /// </param>
    /// <typeparam name="TKey">Type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">Type of the values in the dictionary.</typeparam>
    /// <returns>Values in the dictionary including the newly added ones.</returns>
    public static async Task<IEnumerable<TValue>> GetValuesOrAddRangeIfMissingAsync<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        IEnumerable<TKey> keys,
        Func<IEnumerable<TKey>, Task<IEnumerable<TValue>>> valuesFactory,
        Func<TValue, TKey> keySelector)
    {
        var missingKeys = keys.Where(key => !dictionary.ContainsKey(key));
        var missingItems = missingKeys.Any() ? await valuesFactory(missingKeys) : Enumerable.Empty<TValue>();
        foreach (var item in missingItems)
        {
            dictionary[keySelector(item)] = item;
        }

        return missingItems.Union(keys.SelectWhere(dictionary.GetMaybe));
    }

    /// <summary>
    /// Returns values from the dictionary identified by the given keys. In case of missing items it will also add these
    /// one by one. Could be used as a simple memory cache.
    /// </summary>
    /// <param name="keys">List of keys in the dictionary.</param>
    /// <param name="valueFactory">Operation returning a missing item for a missing key.</param>
    /// <typeparam name="TKey">Type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">Type of the values in the dictionary.</typeparam>
    /// <returns>Values in the dictionary including the newly added ones.</returns>
    public static async Task<IEnumerable<TValue>> GetValuesOrAddIfMissingAsync<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        IEnumerable<TKey> keys,
        Func<TKey, Task<TValue>> valueFactory) =>
        await keys.AwaitEachAsync(key => GetValueOrAddIfMissingAsync(dictionary, key, valueFactory));

    /// <summary>
    /// Returns a value from the dictionary identified by the given key. In case of it's missing it will add it. Could
    /// be used as a simple memory cache.
    /// </summary>
    /// <param name="key">Key in the dictionary.</param>
    /// <param name="valueFactory">Operation returning the missing item for a missing key.</param>
    /// <typeparam name="TKey">Type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">Type of the values in the dictionary.</typeparam>
    /// <returns>Value in the dictionary.</returns>
    public static async Task<TValue> GetValueOrAddIfMissingAsync<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TKey, Task<TValue>> valueFactory)
    {
        var value = dictionary.GetMaybe(key);

        if (!Equals(value, default(TValue))) return value;

        value = await valueFactory(key);
        dictionary[key] = value;

        return value;
    }

    /// <summary>
    /// Checks for a value from the dictionary identified by the given key. In case it's missing this method will add
    /// it.
    /// </summary>
    /// <param name="key">Key in the dictionary.</param>
    /// <param name="valueFactory">Operation returning the missing item for a missing key.</param>
    /// <typeparam name="TKey">Type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">Type of the values in the dictionary.</typeparam>
    public static async Task AddIfMissingAsync<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TKey, Task<TValue>> valueFactory)
    {
        if (dictionary.ContainsKey(key)) return;
        dictionary[key] = await valueFactory(key);
    }

    /// <summary>
    /// Adds several entries to the dictionary (e.g. from another dictionary). This uses <see
    /// cref="Dictionary{TKey,TValue}.Add"/> so duplicate keys are not permitted. It is safe if the <paramref
    /// name="additionalEntries"/> is <see langword="null"/>, nothing will happen.
    /// </summary>
    public static void AddRange<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        IEnumerable<KeyValuePair<TKey, TValue>> additionalEntries)
    {
        if (additionalEntries == null) return;
        foreach (var (key, value) in additionalEntries)
        {
            dictionary.Add(key, value);
        }
    }

    /// <summary>
    /// Adds a new item to the list identified by a key in the dictionary. If the item is already part of the list
    /// then it won't add it again.
    /// </summary>
    /// <param name="key">Key identifying the list in the dictionary.</param>
    /// <param name="value">Value to add to the list.</param>
    /// <typeparam name="TKey">Type of the key in the dictionary.</typeparam>
    /// <typeparam name="TValue">Type of the values in the list in the dictionary.</typeparam>
    public static void AddToList<TKey, TValue>(
        this IDictionary<TKey, IEnumerable<TValue>> dictionary,
        TKey key,
        TValue value)
    {
        var list = dictionary.GetMaybe(key) ?? Enumerable.Empty<TValue>();
        list = list.Union(new[] { value });
        dictionary[key] = list;
    }

    /// <summary>
    /// Converts the provided <paramref name="dictionary"/> into read-only. If the instance implements <see
    /// cref="IReadOnlyDictionary{TKey,TValue}"/> then the input is cast, otherwise copied into a new <see
    /// cref="Dictionary{TKey,TValue}"/> object.
    /// </summary>
    public static IReadOnlyDictionary<TKey, TValue> ToReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) =>
        dictionary as IReadOnlyDictionary<TKey, TValue> ?? new Dictionary<TKey, TValue>(dictionary);

    /// <summary>
    /// Creates a new dictionary based on <paramref name="dictionary"/> by taking each entry's value, and if it's not
    /// <see langword="null"/> and has at least one not <see langword="null"/> item then that item is added under the
    /// same key.
    /// </summary>
    [SuppressMessage(
        "Design",
        "MA0016:Prefer returning collection abstraction instead of implementation",
        Justification = "Better compatibility.")]
    public static Dictionary<TKey, TValue> WithFirstValues<TKey, TValue, TValues>(
        this IEnumerable<KeyValuePair<TKey, TValues>> dictionary,
        Func<TValues, TValue> select = null)
        where TValues : IEnumerable<TValue>
    {
        var result = new Dictionary<TKey, TValue>();

        foreach (var (key, values) in dictionary)
        {
            if (select == null)
            {
                if (values is not null && values.FirstOrDefault() is { } value)
                {
                    result[key] = value;
                }
            }
            else
            {
                if (select(values) is { } value)
                {
                    result[key] = value;
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Creates a new dictionary based on <paramref name="dictionary"/> where each value is a new instance of
    /// <typeparamref name="TValues"/> which is an <see cref="IList{T}"/> of <typeparamref name="TValue"/>. If the value
    /// is not <see langword="null"/> then it's added to the list.
    /// </summary>
    [SuppressMessage(
        "Design",
        "MA0016:Prefer returning collection abstraction instead of implementation",
        Justification = "Better compatibility.")]
    public static Dictionary<TKey, TValues> ToListValuedDictionary<TKey, TValue, TValues>(
        this IEnumerable<KeyValuePair<TKey, TValue>> dictionary,
        Func<TValue, TValues> select = null)
        where TValues : IList<TValue>, new()
    {
        var result = new Dictionary<TKey, TValues>();

        foreach (var (key, value) in dictionary)
        {
            if (select != null)
            {
                result[key] = select(value);
                continue;
            }

            var list = new TValues();
            if (value is not null) list.Add(value);
            result[key] = list;
        }

        return result;
    }

    /// <summary>
    /// Creates a new string keyed dictionary where the key is compared in a case-insensitive manner.
    /// </summary>
    public static IDictionary<string, TValue> ToDictionaryIgnoreCase<TValue>(
        this IEnumerable<KeyValuePair<string, TValue>> source) =>
        new Dictionary<string, TValue>(source, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Creates a new string keyed dictionary where the key is compared in a case-insensitive manner.
    /// </summary>
    public static IDictionary<string, TSource> ToDictionaryIgnoreCase<TSource>(
        this IEnumerable<TSource> source,
        Func<TSource, string> keySelector) =>
        new Dictionary<string, TSource>(
            source.Select(item => new KeyValuePair<string, TSource>(keySelector(item), item)),
            StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Creates a new string keyed dictionary where the key is compared in a case-insensitive manner.
    /// </summary>
    public static IDictionary<string, TValue> ToDictionaryIgnoreCase<TSource, TValue>(
        this IEnumerable<TSource> source,
        Func<TSource, string> keySelector,
        Func<TSource, TValue> valueSelector) =>
        new Dictionary<string, TValue>(
            source.Select(item => new KeyValuePair<string, TValue>(keySelector(item), valueSelector(item))),
            StringComparer.OrdinalIgnoreCase);
}
