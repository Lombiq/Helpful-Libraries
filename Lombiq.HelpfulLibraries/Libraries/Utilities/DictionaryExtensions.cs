using System.Linq;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Safely returns the value by key if it's in the dictionary. If the key is <see langword="default" /> or not
        /// found in the dictionary, it'll return <see langword="default" />.
        /// </summary>
        /// <param name="key">Key in the dictionary.</param>
        /// <typeparam name="TKey">Type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">Type of the values in the dictionary.</typeparam>
        /// <returns>Value identified by the key if it's in the dictionary.</returns>
        public static TValue GetMaybe<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) =>
            !Equals(key, default) && dictionary.TryGetValue(key, out var value) ? value : default;

        /// <summary>
        /// Safely returns the value by key converted to the given type if it's in the dictionary. It'll return
        /// <see langword="default" /> if the key is not present in the dictionary or if the value can't be converted.
        /// </summary>
        /// <param name="key">Key in the dictionary.</param>
        /// <typeparam name="TValue">Type to convert to.</typeparam>
        /// <returns>Value identified by the key if it's in the dictionary.</returns>
        public static TValue GetMaybe<TValue>(this IDictionary<object, object> dictionary, object key) =>
            GetMaybe(dictionary, key) is TValue value ? value : default;

        /// <summary>
        /// Safely returns the value by key if it's in the dictionary. If the key is <see langword="default" /> or not
        /// found in the dictionary, it'll return <see langword="default" />.
        /// </summary>
        /// <param name="key">Key in the dictionary.</param>
        /// <typeparam name="TKey">Type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">Type of the values in the dictionary.</typeparam>
        /// <returns>Value identified by the key if it's in the dictionary.</returns>
        public static TValue GetMaybeReadOnly<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key) =>
            !Equals(key, default) && dictionary.TryGetValue(key, out var value) ? value : default;

        /// <summary>
        /// Returns values from the dictionary identified by the given keys. In case of missing items it will also add
        /// these in one batch. Could be used as a simple memory cache where the items are fetched from the database in
        /// one query for example.
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
        /// Returns values from the dictionary identified by the given keys. In case of missing items it will also add
        /// these one by one. Could be used as a simple memory cache.
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
        /// Returns a value from the dictionary identified by the given key. In case of it's missing it will add it.
        /// Could be used as a simple memory cache.
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
        /// Checks for a value from the dictionary identified by the given key. In case it's missing this method will
        /// add it.
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
    }
}
