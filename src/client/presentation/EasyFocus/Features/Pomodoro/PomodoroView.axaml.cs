using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using EasyFocus.Domain.Entities;
using ReactiveUI;
using Serilog;
using System;
using System.Reactive.Disposables;

namespace EasyFocus.Features.Pomodoro;

public partial class PomodoroView : ReactiveUserControl<PomodoroViewModel>
{
    public PomodoroView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            if (ViewModel is null)
            {
                Log.Fatal("PomodoroViewModel is null");
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
                    SetSessionBackground(s);
                    Log.Debug("View: Selected SessionType = {SessionType}", s);
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

    private void SetSessionBackground(SessionType currentSession)
    {
        switch (currentSession)
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
    }

    private void Pomodoro_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        CloseSettingsIfClickedOutside(e);
    }

    private void PomodoroTimerButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ToggleSettings();
    }

    private void ToggleSettings()
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

    private void CloseSettingsIfClickedOutside(Avalonia.Input.PointerPressedEventArgs e)
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
                Log.Debug("Clicked on the settings");
                return;
            }

            Log.Debug("Clicked outside the settings");
            SettingsButton.IsChecked = false;
            return;
        }

        if (e.Source is not Control clickedElement)
        {
            return;
        }

        bool isAncestor = SettingsPanel.IsVisualAncestorOf(clickedElement);
        if (isAncestor)
        {
            Log.Debug("Clicked on the settings items");
            return;
        }

        SettingsButton.IsChecked = false;
        Log.Debug("Clicked outside the settings");
    }
}