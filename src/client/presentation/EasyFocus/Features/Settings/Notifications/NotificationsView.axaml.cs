using Avalonia.ReactiveUI;
using EasyFocus.Domain.Entities;
using ReactiveUI;
using System;
using System.Diagnostics;
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
                Debug.WriteLine("VIEWMODEL IS NULL");
                return;
            }

            this.WhenAnyValue(v => v.ViewModel!.SelectedSound)
                .Subscribe(s =>
                {
                    switch (s)
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
                })
                .DisposeWith(d);
        });
    }
}