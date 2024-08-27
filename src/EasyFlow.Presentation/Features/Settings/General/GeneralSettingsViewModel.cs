using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyFlow.Presentation.Common;
using SukiUI.Controls;
using System.Threading.Tasks;
using System;
using EasyFlow.Domain.Entities;
using MediatR;
using System.Reactive.Linq;
using EasyFlow.Application.Settings;
using ReactiveUI;
using EasyFlow.Presentation.Services;
using System.Diagnostics;

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

        this.WhenAnyValue(
                vm => vm.IsWorkSoundEnabled,
                vm => vm.IsBreakSoundEnabled,
                vm => vm.IsFocusDescriptionEnabled,
                vm => vm.Volume)
            .Skip(2)
            .DistinctUntilChanged()
            .Throttle(TimeSpan.FromMilliseconds(100))
            .Select(_ => System.Reactive.Unit.Default)
            .InvokeCommand(UpdateSettingsCommand);
    }

    public void Activate()
    {
        Observable
            .StartAsync(GetSettings)
            .Subscribe(settings =>
            {
                IsFocusDescriptionEnabled = settings.IsFocusDescriptionEnabled;
                IsWorkSoundEnabled = settings.IsWorkSoundEnabled;
                IsBreakSoundEnabled = settings.IsBreakSoundEnabled;
                Volume = settings.SoundVolume;
            });
    }

    public void Deactivate()
    {
    }

    [RelayCommand]
    private async Task UpdateSettings()
    {
        var settings = await GetSettings();

        settings.IsFocusDescriptionEnabled = IsFocusDescriptionEnabled;
        settings.IsWorkSoundEnabled = IsWorkSoundEnabled;
        settings.IsBreakSoundEnabled = IsBreakSoundEnabled;
        settings.SoundVolume = Volume;

        var command = new UpdateSettingsCommand
        {
            GeneralSettings = settings
        };

        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
        {
            await SukiHost.ShowToast("Failed to update", "Failed to update the settings", SukiUI.Enums.NotificationType.Error);
        }
    }

    [RelayCommand]
    private async Task BackupData()
    {
        var result = await BackupDbQueryHandler.Handle();
        if (result.IsSuccess)
        {
            await SukiHost.ShowToast("Backup saved", "Backup saved successfully", SukiUI.Enums.NotificationType.Success);
        }
        else
        {
            await SukiHost.ShowToast("Failed to Backup", "Backup failed", SukiUI.Enums.NotificationType.Error);
        }
    }

    [RelayCommand]
    private void DeleteData()
    {
        SukiHost.ShowDialog(new DeleteDataViewModel(_mediator, () =>
        {
            RestartApp();
        })
        , allowBackgroundClose: true);
    }

    private async Task<GeneralSettings> GetSettings()
    {
        var result = await _mediator.Send(new GetSettingsQuery());

        if (result.IsSuccess)
        {
            return result.Value;
        }

        await SukiHost.ShowToast("Failed to load", "Settings couldn't be loaded.", SukiUI.Enums.NotificationType.Error);
        return new GeneralSettings();
    }

    private void RestartApp()
    {
        try
        {
            var mainModule = Process.GetCurrentProcess().MainModule;
            if (mainModule is null)
            {
                return;
            }

            string exePath = mainModule.FileName;
            Process.Start(exePath);

            Process.GetCurrentProcess().Kill();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }
}