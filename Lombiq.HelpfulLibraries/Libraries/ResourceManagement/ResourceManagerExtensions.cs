namespace OrchardCore.ResourceManagement
{
    public static class ResourceManagerExtensions
    {
        /// <summary>
        /// Registers a <c>stylesheet</c> resource by name at head.
        /// </summary>
        public static RequireSettings RegisterStyle(
            this IResourceManager resourceManager,
            string resourceName,
            string version = null) =>
            SetVersionIfAny(resourceManager.RegisterResource("stylesheet", resourceName), version);

        /// <summary>
        /// Registers a <c>script</c> resource by name at the given <paramref name="location"/> (at foot by default).
        /// </summary>
        public static RequireSettings RegisterScript(
            this IResourceManager resourceManager,
            string resourceName,
            string version = null,
            ResourceLocation location = ResourceLocation.Foot) =>
            SetVersionIfAny(resourceManager.RegisterResource("script", resourceName).AtLocation(location), version);

        private static RequireSettings SetVersionIfAny(RequireSettings requireSettings, string version)
        {
            if (!string.IsNullOrEmpty(version)) requireSettings.UseVersion(version);
            return requireSettings;
        }
    }
}
