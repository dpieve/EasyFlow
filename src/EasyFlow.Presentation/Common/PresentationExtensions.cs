using EasyFlow.Presentation.Features.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace EasyFlow.Presentation.Common;
public static class PresentationExtensions
{
    public static ServiceCollection AddPresentation(this ServiceCollection services)
    {
        services.AddSingleton<MainViewModel>();

        services
            // Pages
            .AddTransient(typeof(PageViewModelBase), typeof(SettingsViewModel));
        
        return services;
    }
}