namespace OrchardCore.ResourceManagement
{
    public static class ResourceManagerExtensions
    {
        /// <summary>
        /// Registers a <c>stylesheet</c> resource by name at head.
        /// </summary>
        public static RequireSettings RegisterStyle(this IResourceManager resourceManager, string resourceName) =>
            resourceManager.RegisterResource("stylesheet", resourceName);

        /// <summary>
        /// Registers a <c>script</c> resource by name at the given <paramref name="location"/> (at foot by default).
        /// </summary>
        public static RequireSettings RegisterScript(
            this IResourceManager resourceManager,
            string resourceName,
            ResourceLocation location = ResourceLocation.Foot) =>
            resourceManager.RegisterResource("script", resourceName).AtLocation(location);
    }
}
