using EasyFocus.Common;
using EasyFocus.Domain.Entities;
using EasyFocus.Domain.Services;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Serilog;

using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace EasyFocus.Features.Settings.Notifications;

public sealed partial class NotificationsViewModel : ViewModelBase
{
    [Reactive] private int _volume;
    [Reactive] private Sound _selectedSound;
    [Reactive] private bool _notificationOnCompletionEnabled;
    [Reactive] private bool _notificationWhenSkippingSession;

    private AppSettings _appSettings;
    private readonly ISettingsService _settingsServices;

    public NotificationsViewModel(AppSettings settings, ISettingsService settingsServices)
    {
        _appSettings = settings;
        _settingsServices = settingsServices;

        Volume = _appSettings.AlarmVolume;
        SelectedSound = _appSettings.AlarmSound;
        NotificationOnCompletionEnabled = _appSettings.NotificationOnCompletion;
        NotificationWhenSkippingSession = _appSettings.NotificationAfterSkippedSessions;

        this.WhenAnyValue(vm => vm.NotificationOnCompletionEnabled,
            vm => vm.NotificationWhenSkippingSession,
            vm => vm.Volume,
            vm => vm.SelectedSound)
            .Skip(1)
            .Select(_ => Unit.Default)
            .InvokeCommand(SaveChangesCommand);
    }

    [ReactiveCommand]
    private void OnBack()
    {
        Log.Debug("Notications OnBack");
    }

    [ReactiveCommand]
    private void SelectSound1()
    {
        SelectedSound = Sound.Audio1;
    }

    [ReactiveCommand]
    private void SelectSound2()
    {
        SelectedSound = Sound.Audio2;
    }

    [ReactiveCommand]
    private void SelectMute()
    {
        SelectedSound = Sound.None;
    }

    [ReactiveCommand]
    private async Task SaveChanges()
    {
        _appSettings.AlarmSound = SelectedSound;
        _appSettings.AlarmVolume = Volume;
        _appSettings.NotificationOnCompletion = NotificationOnCompletionEnabled;
        _appSettings.NotificationAfterSkippedSessions = NotificationWhenSkippingSession;

        await _settingsServices.UpdateSettingsAsync(_appSettings);
    }
}