using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using EasyFlow.Application.Settings;
using EasyFlow.Desktop.Services;
using EasyFlow.Domain.Entities;
using EasyFlow.Domain.Repositories;
using EasyFlow.Desktop;
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

    public override void OnFrameworkInitializationCompleted()
    {
        Migrate();
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
        Trace.TraceInformation("OnStartup - started application");
    }

    private async void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
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

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var exception = e.ExceptionObject as Exception;
        Trace.TraceError($"Unhandled Exception: {exception?.Message}");
    }
}