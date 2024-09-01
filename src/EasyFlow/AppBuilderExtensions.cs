using Avalonia;
using CommunityToolkit.Mvvm.DependencyInjection;
using EasyFlow.Application.Common;
using EasyFlow.Infrastructure.Common;
using EasyFlow.Presentation.Common;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

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
            .WriteTo.File("logs.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

        services
            .AddInfrastructure()
            .AddApplication()
            .AddPresentation();

        return services.BuildServiceProvider();
    }
}