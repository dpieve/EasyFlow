using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using EasyFlow.Data;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace EasyFlow;

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