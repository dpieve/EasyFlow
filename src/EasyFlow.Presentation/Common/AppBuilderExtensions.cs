using Avalonia;
using CommunityToolkit.Mvvm.DependencyInjection;
using EasyFlow.Presentation.Data;
using EasyFlow.Presentation.Features.Dashboard;
using EasyFlow.Presentation.Features.Focus;
using EasyFlow.Presentation.Features.Settings;
using EasyFlow.Presentation.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EasyFlow.Presentation.Common;

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