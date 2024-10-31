using Avalonia.Controls.Notifications;
using EasyFlow.Desktop.Common;
using EasyFlow.Desktop.Services;
using EasyFlow.Domain.Entities;
using MediatR;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using SukiUI.Dialogs;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace EasyFlow.Desktop.Features.Settings.General;

public partial class GeneralSettingsViewModel : ActivatableViewModelBase
{
    private readonly IMediator _mediator;
    private readonly IRestartAppService _restartAppService;
    private readonly ILanguageService _languageService;
    private readonly IToastService _toastService;
    private readonly ISukiDialogManager _dialog;
    [Reactive]
    private bool _isFocusDescriptionEnabled;

    [Reactive]
    private bool _isWorkSoundEnabled;

    [Reactive]
    private bool _isBreakSoundEnabled;

    [Reactive]
    //[NotifyPropertyChangedFor(nameof(VolumeLabel))]
    private int _volume;

    [Reactive]
    private bool _isBackupBusy;

    [Reactive]
    private bool _isDeleteBusy;

    public GeneralSettingsViewModel(
        IMediator mediator, 
        IRestartAppService restartAppService, 
        ILanguageService languageService,
        IToastService toastService,
        ISukiDialogManager dialog)
    {
        _mediator = mediator;
        _restartAppService = restartAppService;
        _languageService = languageService;
        _toastService = toastService;
        _dialog = dialog;
        
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

    public override void HandleActivation(CompositeDisposable d)
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

    [ReactiveCommand]
    private async Task UpdateSettings()
    {
        var settings = await GetSettings();

        settings.IsFocusDescriptionEnabled = IsFocusDescriptionEnabled;
        settings.IsWorkSoundEnabled = IsWorkSoundEnabled;
        settings.IsBreakSoundEnabled = IsBreakSoundEnabled;
        settings.SoundVolume = Volume;

        _ = await _mediator.Send(new Application.Settings.Edit.Command { Settings = settings });
    }

    [ReactiveCommand]
    private async Task BackupData()
    {
        IsBackupBusy = true;
        var result = await BackupDbQueryHandler.Handle();
        if (result.IsSuccess)
        {
            _toastService.Display(_languageService.GetString("Success"), _languageService.GetString("SuccessGeneratedBackup"), NotificationType.Success);
        }
        IsBackupBusy = false;
    }

    [ReactiveCommand]
    private void DeleteData()
    {
        IsDeleteBusy = true;

        _dialog.CreateDialog().WithViewModel(dialog => new DeleteDataViewModel(dialog, _mediator,
        () =>
        {
            _restartAppService.Restart();
            IsDeleteBusy = false;
        },
        () => IsDeleteBusy = false))
        .TryShow();
    }

    private async Task<GeneralSettings> GetSettings()
    {
        var result = await _mediator.Send(new Application.Settings.Get.Query());

        if (result.IsSuccess)
        {
            return result.Value;
        }

        return new GeneralSettings();
    }
}