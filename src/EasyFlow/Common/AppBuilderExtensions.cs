using Avalonia;
using CommunityToolkit.Mvvm.DependencyInjection;
using EasyFlow.Features.Focus;
using EasyFlow.Features.Settings;
using EasyFlow.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EasyFlow.Common;

public static class AppBuilderExtensions
{
    public static AppBuilder RegisterAppServices(this AppBuilder builder)
    {
        Ioc.Default
            .ConfigureServices(ConfigureServices());

        return builder;
    }

    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<MainViewModel>();
        
        services
            // Services
            .AddTransient(typeof(IGeneralSettingsService), typeof(GeneralSettingsService))
            .AddTransient(typeof(ITagService), typeof(TagService))
            // Pages
            .AddTransient(typeof(PageViewModelBase), typeof(FocusViewModel))
            .AddTransient(typeof(PageViewModelBase), typeof(SettingsViewModel));

        return services.BuildServiceProvider();
    }
}