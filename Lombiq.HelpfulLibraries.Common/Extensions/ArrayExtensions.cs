namespace System;

public static class ArrayExtensions
{
    /// <summary>
    /// A fluid alternative to <see cref="Array.Exists{T}(T[], Predicate{T})"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// According to <see href="https://rules.sonarsource.com/csharp/RSPEC-6605">SonarSource's rule RSPEC-6605</see>
    /// it's better to use this instead of the general `Any()` extension method.
    /// </para>
    /// </remarks>
    public static bool Exist<T>(this T[] array, Predicate<T> match) => Array.Exists(array, match);

    /// <summary>
    /// A fluid alternative to <see cref="Array.Find{T}(T[], Predicate{T})"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// According to <see href="https://rules.sonarsource.com/csharp/RSPEC-6602">SonarSource's rule RSPEC-6602</see>
    /// it's better to use this instead of the general `FirstOrDefault()` extension method.
    /// </para>
    /// </remarks>
    public static T Find<T>(this T[] array, Predicate<T> match) => Array.Find(array, match);

    /// <summary>
    /// A fluid alternative to <see cref="Array.FindAll{T}(T[], Predicate{T})"/>.
    /// </summary>
    public static T[] FindAll<T>(this T[] array, Predicate<T> match) => Array.FindAll(array, match);

    /// <summary>
    /// A fluid alternative to <see cref="Array.FindIndex{T}(T[], Predicate{T})"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// According to <see href="https://rules.sonarsource.com/csharp/RSPEC-6603">SonarSource's rule RSPEC-6603</see>
    /// it's better to use this instead of the general `All()` extension method.
    /// </para>
    /// </remarks>
    public static bool TrueForAll<T>(this T[] array, Predicate<T> match) => Array.TrueForAll(array, match);
}
