using System.Linq;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Awaits the tasks sequentially. An alternative to
        /// <see cref="Task.WhenAll(System.Collections.Generic.IEnumerable{System.Threading.Tasks.Task})"/> and
        /// Nito.AsyncEx.TaskExtensions.WhenAll when true multi-threaded asynchronicity is not desirable.
        /// </summary>
        /// <param name="source">A collection of items.</param>
        /// <param name="asyncOperation">An async function to call on each item.</param>
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

        public static IList<T> AsList<T>(this IEnumerable<T> collection) =>
            collection is IList<T> list ? list : new List<T>(collection);
    }
}
