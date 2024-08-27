using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using EasyFlow.Application.Settings;
using EasyFlow.Domain.Entities;
using EasyFlow.Domain.Repositories;
using MediatR;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

namespace EasyFlow.Presentation;

public partial class App : Avalonia.Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var migrator = Ioc.Default.GetRequiredService<IDatabaseManagerRepository>();
        migrator.MigrateAsync().GetAwaiter().GetResult();

        SetupLanguage();

        var mainViewModel = Ioc.Default.GetRequiredService<MainViewModel>();

        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktop:
                BindingPlugins.DataValidators.RemoveAt(0);
                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainViewModel
                };
                desktop.Startup += OnStartup;
                desktop.Exit += OnExit;
                break;

            case ISingleViewApplicationLifetime singleViewPlatform:
                singleViewPlatform.MainView = new MainView
                {
                    DataContext = mainViewModel
                };
                break;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void OnStartup(object? sender, ControlledApplicationLifetimeStartupEventArgs e)
    {
        Debug.WriteLine("Startup application");
    }

    private async void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        await Task.Delay(200);
        Debug.WriteLine("Exit application");
    }

    private static void SetupLanguage()
    {
        var mediator = Ioc.Default.GetRequiredService<IMediator>();
        var result = mediator.Send(new GetSettingsQuery()).GetAwaiter().GetResult();
        if (result.IsSuccess)
        {
            var settings = result.Value;
            var selectedLanguage = settings.SelectedLanguage;
            Assets.Resources.Culture = new CultureInfo(selectedLanguage);
        }
        else
        {
            Assets.Resources.Culture = new CultureInfo(SupportedLanguage.English.Code);
        }
    }
}