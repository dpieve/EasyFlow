using Avalonia;
using Avalonia.ReactiveUI;
using EasyFlow.Common;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;

namespace EasyFlow.Linux;

internal sealed class Program
{
    [STAThread]
    public static int Main(string[] args)
    {
        var host = BuildHost();

        var builder = BuildAvaloniaApp();
        if (args.Contains("--drm"))
        {
            SilenceConsole();
            return builder.StartLinuxDrm(args: args, card: null, scaling: 1.0);
        }

        return builder.StartWithClassicDesktopLifetime(args);
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

    private static void SilenceConsole()
    {
        new Thread(() =>
        {
            Console.CursorVisible = false;
            while (true)
                Console.ReadKey(true);
        })
        { IsBackground = true }.Start();
    }
}