using EasyFocus.Common;
using EasyFocus.Features.Settings.Background;
using EasyFocus.Features.Settings.FocusTime;
using EasyFocus.Features.Settings.HomeSettings;
using EasyFocus.Features.Settings.Notifications;
using EasyFocus.Features.Settings.Tags;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System;
using System.Reactive.Linq;

namespace EasyFocus.Features.Settings;

public sealed partial class SettingsViewModel : ViewModelBase
{
    [Reactive] private ViewModelBase _currentViewModel;

    public SettingsViewModel(
        HomeSettingsViewModel homeSettings,
        FocusTimeViewModel focusTime,
        NotificationsViewModel notifications,
        TagsViewModel tags,
        BackgroundViewModel background)
    {
        HomeSettings = homeSettings;
        FocusTime = focusTime;
        Notifications = notifications;
        Tags = tags;
        Background = background;

        CurrentViewModel = HomeSettings;

        ListenToEvents();
    }

    public HomeSettingsViewModel HomeSettings { get; }
    public FocusTimeViewModel FocusTime { get; }
    public NotificationsViewModel Notifications { get; }
    public TagsViewModel Tags { get; }
    public BackgroundViewModel Background { get; }

    public void Restart()
    {
        HomeSettings.CleanMessage();
        CurrentViewModel = HomeSettings;
    }

    private void ListenToEvents()
    {
        HomeSettings.OnFocusTimeCommand
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => CurrentViewModel = FocusTime);

        FocusTime.OnBackCommand
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => CurrentViewModel = HomeSettings);

        HomeSettings.OnNotificationsCommand
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => CurrentViewModel = Notifications);

        Notifications.OnBackCommand
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => CurrentViewModel = HomeSettings);

        HomeSettings.OnTagsCommand
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => CurrentViewModel = Tags);

        Tags.OnBackCommand
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => CurrentViewModel = HomeSettings);

        HomeSettings.OnBackgroundCommand
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => CurrentViewModel = Background);

        Background.OnBackCommand
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ => CurrentViewModel = HomeSettings);
    }
}