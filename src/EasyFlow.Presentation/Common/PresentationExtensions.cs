using EasyFlow.Presentation.Features.Dashboard;
using EasyFlow.Presentation.Features.Focus;
using EasyFlow.Presentation.Features.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace EasyFlow.Presentation.Common;
public static class PresentationExtensions
{
    public static ServiceCollection AddPresentation(this ServiceCollection services)
    {
        services.AddSingleton<MainViewModel>();

        // Pages
        services
            .AddTransient(typeof(PageViewModelBase), typeof(SettingsViewModel))
            .AddTransient(typeof(PageViewModelBase), typeof(DashboardViewModel))
            .AddTransient(typeof(PageViewModelBase), typeof(FocusViewModel));

        return services;
    }
}