using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using EasyFlow.Data;
using System;
using System.Diagnostics;
using System.IO;

namespace EasyFlow;

public partial class App : Application
{
    public static readonly string FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    public static readonly string DbName = "EasyFlow.ds";
    public static readonly string DbFullPath = Path.Combine(FolderPath, DbName);
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
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
        using var context = new AppDbContext();
        var result = context.Database.EnsureCreated();
        if (!result)
        {
            Debug.WriteLine("Db already created.");
        }
        else
        {
            Debug.WriteLine("Db created.");
        }
    }
    private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        Debug.WriteLine("Exit application");
    }
}