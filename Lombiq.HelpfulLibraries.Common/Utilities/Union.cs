namespace Lombiq.HelpfulLibraries.Common.Utilities;

/// <summary>
/// A union of two different types. Only either Left or Right can be set at the same time.
/// </summary>
public class Union<T1, T2>
{
    public T1 Left { get; }
    public T2 Right { get; }
    public bool LeftIsSet { get; }

    internal Union(T1 left, T2 right, bool leftIsSet)
    {
        Left = left;
        Right = right;
        LeftIsSet = leftIsSet;
    }

    public object Either() => LeftIsSet ? Left : Right;

    public (T1 Left, T2 Right) ToTuple() => (Left, Right);

    public static implicit operator Union<T1, T2>(T1 left) => new(left, default, leftIsSet: true);

    public static implicit operator Union<T1, T2>(T2 right) => new(default, right, leftIsSet: false);
}

public static class Union
{
    public static Union<T1, T2> Neither<T1, T2>() =>
        // The value of leftIsSet doesn't matter.
        new(default, default, leftIsSet: true);
}
