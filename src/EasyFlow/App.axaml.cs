using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using EasyFlow.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
        _ = CreateDatabase();
        InitialDatabaseSeed();

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

    private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        Debug.WriteLine("Exit application");
    }

    private static bool CreateDatabase()
    {
        using var context = new AppDbContext();
        var result = context.Database.EnsureCreated();
        //context.Database.Migrate();
        if (!result)
        {
            Debug.WriteLine("Db already created.");
        }
        else
        {
            Debug.WriteLine("Db created.");
        }

        return result;
    }

    private static void InitialDatabaseSeed()
    {
        bool isDbInitialized = false;

        {
            using var context = new AppDbContext();
            isDbInitialized = context.GeneralSettings.Any();

            if (!isDbInitialized)
            {
                var defaultSettings = new GeneralSettings();

                context.GeneralSettings.Add(defaultSettings);
                context.SaveChanges();
            }
        }

        if (isDbInitialized)
        {
            return;
        }

        {
            using var context = new AppDbContext();

            var initialTags = new List<Tag>
            {
                new() { Name = "Work" },
                new() { Name = "Study" },
                new() { Name = "Meditate" },
                new() { Name = "Exercises" },
            };

            context.Tags.AddRange(initialTags);
            context.SaveChanges();
        }

        {
            using var context = new AppDbContext();

            var tag = context.Tags.FirstOrDefault();
            var settings = context.GeneralSettings.FirstOrDefault();
            if (settings is not null && tag is not null)
            {
                settings.SelectedTagId = tag.Id;
                settings.SelectedTag = tag;
                context.GeneralSettings.Update(settings);
            }
        }
        
    }
}