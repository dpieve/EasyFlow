﻿using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyFlow.Presentation.Common;
using SukiUI.Controls;
using System.Diagnostics;
using System.Threading.Tasks;
using System;
using System.IO;
using EasyFlow.Domain.Entities;
using MediatR;

namespace EasyFlow.Presentation.Features.Settings.General;

public partial class GeneralSettingsViewModel : ViewModelBase
{
    private readonly IMediator _mediator;

    [ObservableProperty]
    private bool _isFocusDescriptionEnabled;

    [ObservableProperty]
    private bool _isWorkSoundEnabled;

    [ObservableProperty]
    private bool _isBreakSoundEnabled;

    [ObservableProperty]
    private int _volume;

    public GeneralSettingsViewModel(IMediator mediator)
    {
        _mediator = mediator;

        //var settings = LoadSettings();
        //IsFocusDescriptionEnabled = settings.IsFocusDescriptionEnabled;
        //IsWorkSoundEnabled = settings.IsWorkSoundEnabled;
        //IsBreakSoundEnabled = settings.IsBreakSoundEnabled;
        //Volume = settings.SoundVolume;

        //this.WhenAnyValue(
        //        vm => vm.IsWorkSoundEnabled,
        //        vm => vm.IsBreakSoundEnabled,
        //        vm => vm.IsFocusDescriptionEnabled,
        //        vm => vm.Volume)
        //    .Skip(1)
        //    .Select(_ => System.Reactive.Unit.Default)
        //    .InvokeCommand(PersistSettingsCommand);
    }

    public void Activate()
    {
        Debug.WriteLine("Activated GeneralSettingsViewModel");
    }

    public void Deactivate()
    {
        Debug.WriteLine("Deactivated GeneralSettingsViewModel");
    }

    [RelayCommand]
    private async Task PersistSettings()
    {
        //var result = _generalSettingsService.Get();

        //if (result.Error is not null)
        //{
        //    await SukiHost.ShowToast("Failed to load", "Failed to load the settings", SukiUI.Enums.NotificationType.Error);
        //    return;
        //}

        //var settings = result.Value!;

        //settings.IsFocusDescriptionEnabled = IsFocusDescriptionEnabled;
        //settings.IsWorkSoundEnabled = IsWorkSoundEnabled;
        //settings.IsBreakSoundEnabled = IsBreakSoundEnabled;
        //settings.SoundVolume = Volume;

        //var resultUpdate = await _generalSettingsService.UpdateAsync(settings);
        //if (resultUpdate.Error is not null)
        //{
        //    await SukiHost.ShowToast("Failed to update", "Failed to update the settings", SukiUI.Enums.NotificationType.Error);
        //    return;
        //}
        
        Debug.WriteLine("Persisted settings");
    }

    [RelayCommand]
    private void ClearData()
    {
        //SukiHost.ShowDialog(new ClearDataViewModel(_databaseMigrator, () =>
        //{
        //    string exePath = Process.GetCurrentProcess().MainModule.FileName;
        //    Process.Start(exePath);
        //    Process.GetCurrentProcess().Kill();
        //}),
        //allowBackgroundClose: true);
    }

    [RelayCommand]
    private async Task SaveData()
    {
        try
        {
            var topLevel = TopLevel.GetTopLevel(((IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime).MainWindow);

            if (topLevel is null)
            {
                await SukiHost.ShowToast("Failed to save", "Failed to save a backup file. It didn't find the window", SukiUI.Enums.NotificationType.Error);
                return;
            }

            var fileOptions = new FilePickerSaveOptions()
            {
                Title = "Choose a name and a path to save the data file",
                DefaultExtension = "ds",
                ShowOverwritePrompt = true,
                SuggestedFileName = "EasyFlow",
                FileTypeChoices = [
                    new("Database file (.ds)")
                    {
                        Patterns = [ "ds" ],
                        MimeTypes = ["ds" ]
                    }
                ]
            };

            var files = await topLevel.StorageProvider.SaveFilePickerAsync(fileOptions);

            if (files is not null)
            {
                var path = files.TryGetLocalPath();

                if (path is not null)
                {
                    Debug.WriteLine($"path is {path}");

                    var dbPath = Paths.DbFullPath;
                    File.Copy(dbPath, path, true);

                    await SukiHost.ShowToast("Data saved successfully", $"Data saved to {path}", SukiUI.Enums.NotificationType.Success);
                }
                else
                {
                    Debug.WriteLine("Couldn't find the path to backup the file");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await SukiHost.ShowToast("Failed to backup", "Failed to save a backup file. Try again.", SukiUI.Enums.NotificationType.Error);
        }
    }

    //private Presentation.Data.GeneralSettings LoadSettings()
    //{
    //    var result = _generalSettingsService.Get();

    //    if (result.Error is not null)
    //    {
    //        SukiHost.ShowToast("Failed to load", "Failed to load the settings", SukiUI.Enums.NotificationType.Error);
    //        return new();
    //    }

    //    var settings = result.Value!;
    //    return settings;
    //}
}