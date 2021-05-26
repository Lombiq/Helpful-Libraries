using Fluid;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace OrchardCore.Liquid
{
    public static class LiquidTemplateManagerExtensions
    {
        /// <summary>
        /// Renders the given Liquid expression to string.
        /// </summary>
        /// <param name="expressionToRender">The expression to render.</param>
        public static Task<string> RenderLiquidExpressionAsync(
            this ILiquidTemplateManager liquidTemplateManager,
            string expressionToRender) =>
            liquidTemplateManager.RenderAsync(expressionToRender, NullEncoder.Default);

        /// <summary>
        /// Adds context to the <see cref="LiquidTemplateContext"/> with the given name and the given values converted
        /// to JSON.
        /// </summary>
        /// <param name="key">The key the values will be available under.</param>
        /// <param name="values">The values that will be converted to JSON.</param>
        public static void SetJsonToTemplateContext(
            this ILiquidTemplateManager liquidTemplateManager,
            string key,
            object values) =>
            liquidTemplateManager.Context.SetValue(key, JObject.FromObject(values));
    }
}
