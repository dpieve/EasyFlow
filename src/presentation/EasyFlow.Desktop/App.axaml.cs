using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using EasyFlow.Application.Settings;
using EasyFlow.Desktop.Services;
using EasyFlow.Domain.Entities;
using EasyFlow.Domain.Repositories;
using MediatR;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EasyFlow.Desktop;

public partial class App : Avalonia.Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
    }

    public MainWindow? MainWindow { get; private set; }

    public override void OnFrameworkInitializationCompleted()
    {
        Migrate();
        SetupLanguage();

        var mainViewModel = Ioc.Default.GetRequiredService<MainViewModel>();

        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktop:
                BindingPlugins.DataValidators.RemoveAt(0);

                MainWindow = new MainWindow
                {
                    DataContext = mainViewModel
                };

                desktop.MainWindow = MainWindow;
                desktop.Startup += OnStartup;
                desktop.Exit += OnExit;

                RegisterTrayIcon();

                break;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void OnStartup(object? sender, ControlledApplicationLifetimeStartupEventArgs e)
    {
        Trace.TraceInformation("OnStartup - started application");
    }

    private static async void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        await Task.Delay(200);
        Trace.TraceInformation("OnExit - closed application");
    }

    private static void Migrate()
    {
        var migrator = Ioc.Default.GetRequiredService<IDatabaseManagerRepository>();
        migrator.MigrateAsync().GetAwaiter().GetResult();
    }

    private static void SetupLanguage()
    {
        var mediator = Ioc.Default.GetRequiredService<IMediator>();
        var result = mediator.Send(new GetSettingsQuery()).GetAwaiter().GetResult();
        var settings = result.Value;
        var selectedLanguage = settings.SelectedLanguage;

        var languageService = Ioc.Default.GetRequiredService<ILanguageService>();
        languageService.SetLanguage(SupportedLanguage.FromCode(selectedLanguage));
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var exception = e.ExceptionObject as Exception;
        Trace.TraceError($"Unhandled Exception: {exception?.Message}");
    }

    private void Close_Click()
    {
        if (MainWindow is null)
        {
            return;
        }

        MainWindow.ShouldClose = true;
        MainWindow.Close();
    }

    private void Open_Click()
    {
        if (MainWindow is null)
        {
            return;
        }

        MainWindow.FromTray();
        MainWindow.Show();
    }

    private void RegisterTrayIcon()
    {
        var trayIcon = new TrayIcon
        {
            IsVisible = true,
            Command = new RelayCommand(Open_Click),
            Icon = new WindowIcon(new Bitmap("Assets/panda.png")),
            Menu = new NativeMenu
            {
                new NativeMenuItem
                {
                    Header = ConstantTranslation.OpenEasyFlow,
                    Command = new RelayCommand(Open_Click),
                },

                new NativeMenuItem
                {
                    Header = ConstantTranslation.CloseEasyFlow,
                    Command = new RelayCommand(Close_Click),
                }
            }
        };

        var trayIcons = new TrayIcons
        {
            trayIcon
        };

        SetValue(TrayIcon.IconsProperty, trayIcons);
    }
}