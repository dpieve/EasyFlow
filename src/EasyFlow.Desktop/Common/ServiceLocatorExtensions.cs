using Splat;

namespace EasyFlow.Desktop.Common;

internal static class ServiceLocatorExtensions
{
    public static T GetServiceOrThrow<T>(this IReadonlyDependencyResolver resolver, string? contract = null)
    {
        return Locator.Current.GetService<T>() ?? throw new System.InvalidOperationException("Service was not found.");
    }
}