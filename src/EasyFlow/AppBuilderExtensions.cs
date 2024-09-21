using Avalonia;
using CommunityToolkit.Mvvm.DependencyInjection;
using EasyFlow.Application.Common;
using EasyFlow.Infrastructure.Common;
using EasyFlow.Presentation.Common;
using EasyFlow.Presentation.Services;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.IO;

namespace EasyFlow;

public static class AppBuilderExtensions
{
    public static AppBuilder RegisterServices(this AppBuilder builder)
    {
        Ioc.Default
            .ConfigureServices(ConfigureServices());

        return builder;
    }

    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Warning()
            .WriteTo.Debug()
            .WriteTo.File(Path.Combine("logs", "logs.txt"), rollingInterval: RollingInterval.Day)
            .CreateLogger();

        services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

        INotificationService notificationService = new NotificationService();

        services.AddSingleton(notificationService);

        services
            .AddInfrastructure()
            .AddApplication()
            .AddPresentation();

        return services.BuildServiceProvider();
    }
}
