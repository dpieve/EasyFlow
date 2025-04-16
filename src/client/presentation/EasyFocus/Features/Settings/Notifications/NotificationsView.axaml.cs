using Avalonia.ReactiveUI;
using EasyFocus.Domain.Entities;
using ReactiveUI;
using Serilog;
using System;
using System.Reactive.Disposables;

namespace EasyFocus.Features.Settings.Notifications;

public partial class NotificationsView : ReactiveUserControl<NotificationsViewModel>
{
    public NotificationsView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            if (ViewModel is null)
            {
                Log.Fatal("NotificationViewModel is null");
                return;
            }

            this.WhenAnyValue(v => v.ViewModel!.SelectedSound)
                .Subscribe(s =>
                {
                    SetSoundButtonClasses(s);
                    Log.Debug("View: Selected sound");
                })
                .DisposeWith(d);
        });
    }

    private void SetSoundButtonClasses(Sound sound)
    {
        switch (sound)
        {
            case Sound.Audio1:
                Sound1Button.Classes.Add("Selected");
                Sound2Button.Classes.Remove("Selected");
                MuteButton.Classes.Remove("Selected");
                break;

            case Sound.Audio2:
                Sound1Button.Classes.Remove("Selected");
                Sound2Button.Classes.Add("Selected");
                MuteButton.Classes.Remove("Selected");
                break;

            case Sound.None:
                Sound1Button.Classes.Remove("Selected");
                Sound2Button.Classes.Remove("Selected");
                MuteButton.Classes.Add("Selected");
                break;
        }
    }
}