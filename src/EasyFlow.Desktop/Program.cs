using Avalonia;
using Avalonia.ReactiveUI;
using EasyFlow.Application.Common;
using EasyFlow.Desktop.Common;
using EasyFlow.Desktop.Services;
using EasyFlow.Infrastructure.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Serilog;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Velopack;

namespace EasyFlow.Desktop;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        VelopackApp.Build().Run();

        var mutex = new Mutex(false, typeof(Program).FullName);

        try
        {
            const int timeoutSeconds = 3;

            if (!mutex.WaitOne(TimeSpan.FromSeconds(timeoutSeconds), true))
            {
                Trace.TraceInformation($"Another instance is already running. Exiting...");
                return;
            }

            AppInit(args);
        }
        finally
        {
            mutex.ReleaseMutex();
        }
    }

    private static void AppInit(string[] args)
    {
        _ = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                {
                    services.UseMicrosoftDependencyResolver();
                    var resolver = Locator.CurrentMutable;
                    resolver.InitializeSplat();
                    resolver.InitializeReactiveUI();

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
                })
                .Build();

        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI()
            .StartWithClassicDesktopLifetime(args);
    }
}