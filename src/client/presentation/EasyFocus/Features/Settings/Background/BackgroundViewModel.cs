﻿using EasyFocus.Common;
using EasyFocus.Domain.Entities;
using EasyFocus.Domain.Services;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Serilog;

using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace EasyFocus.Features.Settings.Background;

public sealed partial class BackgroundViewModel : ViewModelBase
{
    private AppSettings _settings;
    private readonly IAppSettingsService _settingsService;
    [Reactive] private string _selectedBackground;

    public BackgroundViewModel(AppSettings settings, IAppSettingsService settingsService)
    {
        _settings = settings;
        _settingsService = settingsService;
        SelectedBackground = _settings.BackgroundPath;

        this.WhenAnyValue(vm => vm.SelectedBackground)
            .Skip(1)
            .Select(_ => Unit.Default)
            .InvokeCommand(SaveChangesCommand);
    }

    [ReactiveCommand]
    private void OnBack()
    {
        Log.Debug("Background OnBack");
    }

    [ReactiveCommand]
    private void OnSelectBackground(string backgroundId)
    {
        Log.Debug($"Background OnSelectBackground {backgroundId}");

        SelectedBackground = $"background{backgroundId}.png";
    }

    [ReactiveCommand]
    private async Task SaveChanges()
    {
        _settings.BackgroundPath = _selectedBackground;
        await _settingsService.UpdateSettingsAsync(_settings);
    }
}