using Avalonia;
using Avalonia.Browser;
using Avalonia.ReactiveUI;
using EasyFocus.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Serilog;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using System.Runtime.Versioning;
using System.Threading.Tasks;

[assembly: SupportedOSPlatform("browser")]

namespace EasyFocus.Browser;

internal sealed partial class Program
{
    private static async Task Main(string[] args)
    {
        await AppInit(args);
    }

    private static Task AppInit(string[] args)
    {
        _ = Host.CreateDefaultBuilder()
               .ConfigureServices((_, services) =>
               {
                   services.UseMicrosoftDependencyResolver();

                   Log.Logger = new LoggerConfiguration()
#if DEBUG
                        .MinimumLevel.Verbose()
#else
                        .MinimumLevel.Information()
#endif
                        .Enrich.FromLogContext()
                        .WriteTo.Debug()
                        .CreateLogger();

                   services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

                   var resolver = Locator.CurrentMutable;
                   resolver.InitializeSplat();
                   resolver.InitializeReactiveUI();

                   services
                       .AddBrowser()
                       .AddPresentation();
               })
               .Build();

        return AppBuilder.Configure<App>()
            .WithInterFont()
            .UseReactiveUI()
            .StartBrowserAppAsync("out");
    }
}