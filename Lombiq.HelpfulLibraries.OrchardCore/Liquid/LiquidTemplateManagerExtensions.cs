using System;

namespace OrchardCore.Liquid;

[Obsolete("" +
    "Use ILiquidTemplateManager.RenderStringAsync() instead of RenderLiquidExpressionAsync() and " +
    "new Fluid.Values.ObjectValue(myObject) instead of SetJsonToTemplateContext().")]
public static class LiquidTemplateManagerExtensions
{
}
