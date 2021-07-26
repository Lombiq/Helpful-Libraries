namespace System
{
    public static class EnumExtensions
    {
        public static InvalidOperationException UnknownEnumException<T>(this T other)
            where T : Enum =>
            new($"Unknown {other.GetType().Name}: '{other}'");
    }
}
