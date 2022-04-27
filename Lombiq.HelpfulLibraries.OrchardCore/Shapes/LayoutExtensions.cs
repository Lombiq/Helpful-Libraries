using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Layout;
using System.Threading.Tasks;

namespace Lombiq.HelpfulLibraries.OrchardCore.Contents;

public static class LayoutExtensions
{
    /// <summary>
    /// Adds the <paramref name="shape"/> to the zone called <paramref name="zoneName"/>, optionally at the given
    /// <paramref name="position"/>.
    /// </summary>
    public static async Task AddShapeToZoneAsync(
        this ILayoutAccessor layoutAccessor,
        string zoneName,
        IShape shape,
        string position = "")
    {
        var layout = await layoutAccessor.GetLayoutAsync();
        var zone = layout.Zones[zoneName];
        if (zone != null) await zone.AddAsync(shape, position);
    }

    public static Task AddShapeToSideMenuAsync(this ILayoutAccessor layoutAccessor, IShape shape) =>
        AddShapeToZoneAsync(layoutAccessor, "SideMenu", shape);
}
