using System.Linq;

namespace System.Collections.Generic;

public static class CollectionExtensions
{
    /// <summary>
    /// Returns the Cartesian product (also known as cross product) of <paramref name="collection1"/> with <paramref
    /// name="collection2"/>, or if that's <see langword="null"/> then with itself. This means that each possible
    /// pairing of the collections are returned. This can replace two nested for loops with a single foreach loop.
    /// </summary>
    public static IEnumerable<(T Left, T Right)> CartesianProduct<T>(
        this ICollection<T> collection1,
        ICollection<T> collection2 = null) =>
        collection1.SelectMany(_ => collection2 ?? collection1, (left, right) => (left, right));

    /// <summary>
    /// Appends every item from <paramref name="source"/> to <paramref name="target"/> using <see
    /// cref="ICollection{T}.Add(T)"/>.
    /// </summary>
    public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> source)
    {
        foreach (var item in source) target.Add(item);
    }

    /// <summary>
    /// Removes every item <paramref name="collection"/> where <paramref name="predicate"/> returns <see
    /// langword="true"/> or if it's <see langword="null"/>.
    /// </summary>
    public static void RemoveAll<T>(this ICollection<T> collection, Func<T, bool> predicate = null)
    {
        foreach (var item in collection)
        {
            if (predicate == null || predicate(item))
            {
                collection.Remove(item);
            }
        }
    }

    /// <summary>
    /// Clears the collection and then adds the values from <paramref name="newValues"/> instead.
    /// </summary>
    public static void SetItems<T>(this ICollection<T> collection, IEnumerable<T> newValues)
    {
        collection.Clear();
        collection.AddRange(newValues);
    }
}
