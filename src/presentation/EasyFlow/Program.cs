using Avalonia;
using Avalonia.ReactiveUI;
using EasyFlow.Desktop;
using EasyFlow.Desktop.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Splat.Microsoft.Extensions.DependencyInjection;
using Splat;
using System;
using System.Threading;
using Velopack;
using ReactiveUI;
using Serilog;
using System.IO;
using EasyFlow.Infrastructure.Common;
using EasyFlow.Application.Common;
using EasyFlow.Desktop.Common;

namespace EasyFlow;

internal sealed class Program
{
    private const int _timeoutSeconds = 3;

    [STAThread]
    public static void Main(string[] args)
    {
        VelopackApp.Build().Run();

        var mutex = new Mutex(false, typeof(Program).FullName);

        try
        {
            if (!mutex.WaitOne(TimeSpan.FromSeconds(_timeoutSeconds), true))
            {
                return;
            }

            IHost Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
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

            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        finally
        {
            mutex.ReleaseMutex();
        }
    }
    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .RegisterServices()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}