using Avalonia;
using CommunityToolkit.Mvvm.DependencyInjection;
using EasyFlow.Data;
using EasyFlow.Features.Dashboard;
using EasyFlow.Features.Focus;
using EasyFlow.Features.Settings;
using EasyFlow.Services;
using Microsoft.EntityFrameworkCore;
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

        services.AddDbContextFactory<AppDbContext>(
            options =>
                {  
                    options.UseSqlite($"Data Source={App.DbFullPath}"); 
                    options.EnableDetailedErrors();
                    options.EnableSensitiveDataLogging();
                }, 
            lifetime: ServiceLifetime.Scoped);

        services.AddSingleton<MainViewModel>();
        
        services.AddSingleton<IDatabaseManager, DatabaseManager>();
        
        services
            // Services
            .AddTransient(typeof(IGeneralSettingsService), typeof(GeneralSettingsService))
            .AddTransient(typeof(ITagService), typeof(TagService))
            .AddTransient(typeof(IPlaySoundService), typeof(PlaySoundService))
            .AddTransient(typeof(ISessionService), typeof(SessionService))
            // Pages
            .AddTransient(typeof(PageViewModelBase), typeof(FocusViewModel))
            .AddTransient(typeof(PageViewModelBase), typeof(DashboardViewModel))
            .AddTransient(typeof(PageViewModelBase), typeof(SettingsViewModel));

        return services.BuildServiceProvider();
    }
}