using System.Linq;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Awaits the tasks sequentially. An alternative to <see
        /// cref="Task.WhenAll(IEnumerable{Task})"/> and
        /// <c>Nito.AsyncEx.TaskExtensions.WhenAll</c> when true multi-threaded asynchronicity is not desirable.
        /// </summary>
        /// <param name="source">A collection of items.</param>
        /// <param name="asyncOperation">An <see langword="async"/> function to call on each item.</param>
        /// <typeparam name="TItem">The type of the input collection's items.</typeparam>
        /// <typeparam name="TResult">The type of the output collection's items.</typeparam>
        /// <returns>When awaited the task contains the results which were added one-by-one.</returns>
        public static async Task<IList<TResult>> AwaitEachAsync<TItem, TResult>(
            this IEnumerable<TItem> source,
            Func<TItem, Task<TResult>> asyncOperation)
        {
            var results = new List<TResult>();
            foreach (var item in source) results.Add(await asyncOperation(item));
            return results;
        }

        /// <summary>
        /// Awaits the tasks sequentially while the action returns <see langword="false"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the <see langword="foreach"/> was never broken.</returns>
        public static async Task<bool> AwaitWhileAsync<TItem>(
            this IEnumerable<TItem> source,
            Func<TItem, Task<bool>> asyncWhileOperation)
        {
            foreach (var item in source)
            {
                if (!await asyncWhileOperation(item)) return false;
            }

            return true;
        }

        /// <summary>
        /// Awaits the tasks sequentially until the action returns <see langword="true"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the <see langword="foreach"/> was never broken.</returns>
        public static async Task<bool> AwaitUntilAsync<TItem>(
            this IEnumerable<TItem> source,
            Func<TItem, Task<bool>> asyncUntilOperation)
        {
            foreach (var item in source)
            {
                if (await asyncUntilOperation(item)) return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether any element of a sequence satisfies a condition, asynchronously, like LINQ Any().
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if any elements in the source sequence pass the test in the specified predicate;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public static async Task<bool> AnyAsync<TItem>(
            this IEnumerable<TItem> source,
            Func<TItem, Task<bool>> asyncUntilOperation) =>
            !await AwaitUntilAsync(source, asyncUntilOperation);

        /// <summary>
        /// Attempts to cast <paramref name="collection"/> into <see cref="List{T}"/>. If that's not possible then
        /// converts it into one. Not to be confused with <see cref="Enumerable.ToList{TSource}"/> that always creates a
        /// separate <see cref="List{T}"/> regardless of source type. This extension is more suitable when the <paramref
        /// name="collection"/> is expected to be <see cref="List{T}"/> but has to be stored as <see
        /// cref="IEnumerable{T}"/>.
        /// </summary>
        public static IList<T> AsList<T>(this IEnumerable<T> collection) =>
            collection is IList<T> list ? list : new List<T>(collection);

        /// <summary>
        /// Transforms the specified <paramref name="collection"/> with the <paramref name="select"/> function and
        /// returns the items that are not null. Or if the <paramref name="where"/> function is given then those that
        /// return <see langword="true"/> with it.
        /// </summary>
        public static IEnumerable<TOut> SelectWhere<TIn, TOut>(
            this IEnumerable<TIn> collection,
            Func<TIn, TOut> select,
            Func<TOut, bool> where = null)
        {
            foreach (var item in collection)
            {
                var converted = select(item);
                if (where?.Invoke(converted) ?? !(converted is null)) yield return converted;
            }
        }

        /// <summary>
        /// Returns a dictionary created from the <paramref name="collection"/>. If there are key clashes, the item
        /// later in the enumeration overwrites the earlier one.
        /// </summary>
        public static Dictionary<TKey, TValue> ToDictionaryOverwrite<TIn, TKey, TValue>(
            this IEnumerable<TIn> collection,
            Func<TIn, TKey> keySelector,
            Func<TIn, TValue> valueSelector)
        {
            var dictionary = new Dictionary<TKey, TValue>();
            foreach (var item in collection) dictionary[keySelector(item)] = valueSelector(item);
            return dictionary;
        }

        /// <summary>
        /// Returns a dictionary created from the <paramref name="collection"/>. If there are key clashes, the item
        /// later in the enumeration overwrites the earlier one.
        /// </summary>
        public static Dictionary<TKey, TIn> ToDictionaryOverwrite<TIn, TKey>(
            this IEnumerable<TIn> collection,
            Func<TIn, TKey> keySelector) =>
            ToDictionaryOverwrite(collection, keySelector, item => item);
    }
}
