using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using EasyFlow.Desktop.Common;
using EasyFlow.Desktop.Services;
using EasyFlow.Domain.Entities;
using EasyFlow.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using Splat;
using System.Diagnostics;

namespace EasyFlow.Desktop;

public partial class App : Avalonia.Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
    }

    public MainWindow? MainWindow { get; private set; }

    public override async void OnFrameworkInitializationCompleted()
    {
        await InitializeDb();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            BindingPlugins.DataValidators.RemoveAt(0);

            var mainViewModel = Locator.Current.GetServiceOrThrow<MainViewModel>();

            MainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };

            desktop.MainWindow = MainWindow;
            desktop.Startup += OnStartup;
            desktop.Exit += OnExit;

            RegisterTrayIcon();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static async Task InitializeDb()
    {
        try
        {
            var context = Locator.Current.GetServiceOrThrow<DataContext>();
            if (context is not null)
            {
                await context.Database.MigrateAsync();
                await Seed.SeedData(context);

                var settings = await context.GeneralSettings.FirstOrDefaultAsync();
                if (settings is not null)
                {
                    var selectedLanguage = settings.SelectedLanguage;
                    var languageService = Locator.Current.GetServiceOrThrow<ILanguageService>();
                    languageService.SetLanguage(SupportedLanguage.FromCode(selectedLanguage));
                }
            }
        }
        catch (Exception ex)
        {
            Trace.TraceError($"Error while starting the application {ex.Message}");
        }
    }

    private static async void OnStartup(object? sender, ControlledApplicationLifetimeStartupEventArgs e)
    {
        Trace.TraceInformation("OnStartup - started application");
    }

    private static async void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        await Task.Delay(200);
        Trace.TraceInformation("OnExit - closed application");
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

    public void Open_Click()
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
        var icon = new Bitmap(AssetLoader.Open(new Uri("avares://EasyFlow.Desktop/Assets/panda.png")));
        var trayIcon = new TrayIcon
        {
            IsVisible = true,
            Command = ReactiveCommand.Create(Open_Click),
            Icon = new WindowIcon(icon),
            Menu =
            [
                new NativeMenuItem
                {
                    Header = ConstantTranslation.OpenEasyFlow,
                    Command =  ReactiveCommand.Create(Open_Click),
                },

                new NativeMenuItem
                {
                    Header = ConstantTranslation.CloseEasyFlow,
                    Command = ReactiveCommand.Create(Close_Click),
                }
            ]
        };

        var trayIcons = new TrayIcons
        {
            trayIcon
        };

        SetValue(TrayIcon.IconsProperty, trayIcons);
    }
}