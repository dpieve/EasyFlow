using Avalonia;
using Avalonia.ReactiveUI;
using EasyFlow.Common;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using System;

namespace EasyFlow.Linux;

internal sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        AppInit(args);
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

                    services
                        .AddPlatform()
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