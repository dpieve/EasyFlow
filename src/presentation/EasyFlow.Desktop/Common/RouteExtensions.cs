using SimpleRouter;

namespace EasyFlow.Desktop.Common;

internal static class RouteExtensions
{
    public static void Activate(this IRoute route)
    {
        var prevActivatable = route as IActivatableRoute;
        prevActivatable?.OnActivated();
    }

    public static void Deactivate(this IRoute route)
    {
        var prevActivatable = route as IActivatableRoute;
        prevActivatable?.OnDeactivated();
    }
}