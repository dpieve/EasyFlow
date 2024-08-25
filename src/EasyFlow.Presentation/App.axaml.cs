using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using EasyFlow.Presentation.Common;
using EasyFlow.Presentation.Data;
using EasyFlow.Presentation.Services;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace EasyFlow.Presentation;

public partial class App : Application
{
    public static readonly string DbName = "EasyFlow.ds";
    public static readonly string BasePath = AppDomain.CurrentDomain.BaseDirectory;
    public static readonly string DbFullPath = Path.Combine(BasePath, DbName);

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var migrator = Ioc.Default.GetRequiredService<IDatabaseManager>();
        migrator.Migrate();

        var mainViewModel = Ioc.Default.GetRequiredService<MainViewModel>();

        var generalSettingsService = Ioc.Default.GetRequiredService<IGeneralSettingsService>();
        var result = generalSettingsService.GetSelectedLanguage();
        if (result.Error is not null)
        {
            Presentation.Assets.Resources.Culture = new CultureInfo(SupportedLanguage.English.Code);
            Debug.WriteLine("Failed to get the selected language");
        }
        else
        {
            var selectedLanguage = result!.Value!.Code;
            Presentation.Assets.Resources.Culture = new CultureInfo(selectedLanguage);
        }

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
}