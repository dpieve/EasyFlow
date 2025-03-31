using Avalonia;
using Avalonia.ReactiveUI;
using EasyFlow.Common;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Threading;

namespace EasyFlow.Windows;

internal sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var mutex = new Mutex(false, typeof(Program).FullName);

        try
        {
            if (!mutex.WaitOne(TimeSpan.FromSeconds(3), true))
            {
                Trace.TraceInformation($"Another instance is already running. Exiting...");
                return;
            }

            Run();
        }
        finally
        {
            mutex.ReleaseMutex();
        }
    }

    public static void Run()
    {
        var host = BuildHost();
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(Array.Empty<string>());
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();

    public static IHost BuildHost()
    {
        return Host.CreateDefaultBuilder()
            .UseDefaultServiceProvider(options =>
            {
                options.ValidateScopes = true;
                options.ValidateOnBuild = true;
            })
            .ConfigureServices((_, services) =>
            {
                services.UseMicrosoftDependencyResolver();

                var resolver = Locator.CurrentMutable;
                resolver.InitializeSplat();
                resolver.InitializeReactiveUI();

                services
                    .AddPlatform()
                    .AddPresentation();
            })
            .Build();
    }
}