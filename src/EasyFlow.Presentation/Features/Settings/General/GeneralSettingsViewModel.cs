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
        var result = await BackupDbQueryHandler.Handle();
        if (result.IsSuccess)
        {
            await SukiHost.ShowToast(_languageService.GetString("Success"), _languageService.GetString("SuccessGeneratedBackup"), SukiUI.Enums.NotificationType.Success);
        }
    }

    [RelayCommand]
    private void DeleteData()
    {
        SukiHost.ShowDialog(new DeleteDataViewModel(_mediator, () =>
        {
            _restartAppService.Restart();
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

        return new GeneralSettings();
    }
}