using Avalonia;
using Avalonia.ReactiveUI;
using EasyFlow.Common;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using System;

namespace EasyFlow.Windows;

internal sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var host = BuildHost();
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
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