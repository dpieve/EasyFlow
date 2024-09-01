using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyFlow.Application.Settings;
using EasyFlow.Domain.Entities;
using EasyFlow.Presentation.Common;
using EasyFlow.Presentation.Services;
using MediatR;
using ReactiveUI;
using SukiUI.Controls;
using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace EasyFlow.Presentation.Features.Settings.General;

public partial class GeneralSettingsViewModel : ViewModelBase
{
    private readonly IMediator _mediator;
    private readonly IRestartAppService _restartAppService;
    private readonly ILanguageService _languageService;

    [ObservableProperty]
    private bool _isFocusDescriptionEnabled;

    [ObservableProperty]
    private bool _isWorkSoundEnabled;

    [ObservableProperty]
    private bool _isBreakSoundEnabled;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(VolumeLabel))]
    private int _volume;

    [ObservableProperty]
    private bool _isBackupBusy;

    [ObservableProperty]
    private bool _isDeleteBusy;

    public GeneralSettingsViewModel(IMediator mediator, IRestartAppService restartAppService, ILanguageService languageService)
    {
        _mediator = mediator;
        _restartAppService = restartAppService;
        _languageService = languageService;

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

    public string VolumeLabel => @$"{ConstantTranslation.VolumeSound} {Volume}%";

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
        Trace.TraceInformation("Deactivating GeneralSettingsViewModel");
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

        _ = await _mediator.Send(command);
    }

    [RelayCommand]
    private async Task BackupData()
    {
        IsBackupBusy = true;
        var result = await BackupDbQueryHandler.Handle();
        if (result.IsSuccess)
        {
            await SukiHost.ShowToast(_languageService.GetString("Success"), _languageService.GetString("SuccessGeneratedBackup"), SukiUI.Enums.NotificationType.Success);
        }
        IsBackupBusy = false;
    }

    [RelayCommand]
    private void DeleteData()
    {
        IsDeleteBusy = true;

        SukiHost.ShowDialog(new DeleteDataViewModel(_mediator,
        () =>
        {
            _restartAppService.Restart();
            IsDeleteBusy = false;
        },
        () => IsDeleteBusy = false)
        , allowBackgroundClose: false);
    }

    private async Task<GeneralSettings> GetSettings()
    {
        var result = await _mediator.Send(new GetSettingsQuery());

        if (result.IsSuccess)
        {
            return result.Value;
        }

        return new GeneralSettings();
    }
}