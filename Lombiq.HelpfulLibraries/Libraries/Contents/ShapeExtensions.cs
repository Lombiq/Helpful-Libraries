namespace OrchardCore.DisplayManagement.Implementation
{
    public static class ShapeExtensions
    {
        public static T Get<T>(this IShape shape, string name = null)
            where T : class =>
            shape.Properties[name ?? typeof(T).Name] as T;

        public static T Get<T>(this ShapeDisplayContext context, string name = null)
            where T : class =>
            Get<T>(context.Shape, name);
    }
}
