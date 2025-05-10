using Splat;

namespace EasyFocus.Common;

internal static class ServiceLocatorExtensions
{
    public static T GetServiceOrThrow<T>(this IReadonlyDependencyResolver resolver)
    {
        return Locator.Current.GetService<T>() ?? throw new System.InvalidOperationException("Service was not found. Make sure to register it.");
    }
}