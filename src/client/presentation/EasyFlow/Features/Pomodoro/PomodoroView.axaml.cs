using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using EasyFlow.Domain.Entities;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive.Disposables;

namespace EasyFlow.Features.Pomodoro;

public partial class PomodoroView : ReactiveUserControl<PomodoroViewModel>
{
    public PomodoroView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            if (ViewModel is null)
            {
                Debug.WriteLine("VIEWMODEL IS NULL");
                return;
            }

            this.WhenAnyValue(v => v.ViewModel!.SmallWindowMode)
                .Subscribe(s =>
                {
                    SettingsButton.IsVisible = !s;
                })
                .DisposeWith(d);

            this.WhenAnyValue(v => v.ViewModel!.SessionType)
                .Subscribe(s =>
                {
                    Debug.WriteLine("View: SessionType = " + s);

                    switch (s)
                    {
                        case SessionType.Pomodoro:
                            PomodoroSessionButton.Background = Brushes.Red;
                            ShortBreakSessionButton.Background = Brushes.Transparent;
                            LongBreakSessionButton.Background = Brushes.Transparent;
                            break;

                        case SessionType.ShortBreak:
                            PomodoroSessionButton.Background = Brushes.Transparent;
                            ShortBreakSessionButton.Background = Brushes.Red;
                            LongBreakSessionButton.Background = Brushes.Transparent;
                            break;

                        case SessionType.LongBreak:
                            PomodoroSessionButton.Background = Brushes.Transparent;
                            ShortBreakSessionButton.Background = Brushes.Transparent;
                            LongBreakSessionButton.Background = Brushes.Red;
                            break;
                    }
                })
                .DisposeWith(d);

            this.WhenAnyValue(v => v.ViewModel!.PomodorosCompleted)
                .Subscribe(pc =>
                {
                    SessionsProgressBar.Value = pc % ViewModel.Settings.FocusTime.PomodoroSessionsBeforeLongBreak;
                })
                .DisposeWith(d);
        });
    }

    private void UserControl_PointerPressed_1(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        if (SettingsButton.IsChecked == false)
        {
            return;
        }

        var clickedBorderElement = e.Source as Decorator;
        if (clickedBorderElement is not null)
        {
            var decoratorSettingsPanel = SettingsPanel as Decorator;
            if (clickedBorderElement == decoratorSettingsPanel)
            {
                Debug.WriteLine("Clicked on the settings border");
                return;
            }

            Debug.WriteLine("Clicked on a border outside");
            SettingsButton.IsChecked = false;
            return;
        }

        var clickedElement = e.Source as Control;
        if (clickedElement is null)
        {
            return;
        }

        var isAncestor = SettingsPanel.IsVisualAncestorOf(clickedElement);
        if (isAncestor)
        {
            Debug.WriteLine("Clicked on the settings items");
            return;
        }

        SettingsButton.IsChecked = false;
        Debug.WriteLine("Clicked outside, closing settings");
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        SettingsButton.IsChecked = !SettingsButton.IsChecked;

        if (SettingsButton.IsChecked == true)
        {
            if (ViewModel is null)
            {
                return;
            }

            ViewModel.Settings.HomeSettings.OnFocusTimeCommand.Execute().Subscribe();
        }
    }
}