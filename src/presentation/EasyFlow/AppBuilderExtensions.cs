using Avalonia;
using EasyFlow.Application.Common;
using EasyFlow.Desktop.Common;
using EasyFlow.Desktop.Services;
using EasyFlow.Infrastructure.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Splat;
using System;
using System.ComponentModel;
using System.IO;

namespace EasyFlow;

public static class AppBuilderExtensions
{
    public static AppBuilder RegisterServices(this AppBuilder builder)
    {
        //Ioc.Default
        //    .ConfigureServices(ConfigureServices());

        

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
