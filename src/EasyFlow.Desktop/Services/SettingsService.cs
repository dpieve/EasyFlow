using EasyFlow.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Reactive.Subjects;

namespace EasyFlow.Desktop.Services;

public interface ISettingsService : IDisposable
{
    /// <summary>
    /// Update the settings
    /// </summary>
    /// <param name="generalSettings">New settings</param>
    Task UpdateAsync(GeneralSettings generalSettings);

    /// <summary>
    /// Current Settings
    /// </summary>
    GeneralSettings Settings { get; }

    /// <summary>
    /// Listen to changes in the settings.
    /// </summary>
    IObservable<GeneralSettings> ObserveSettings { get; }
}

public sealed partial class SettingsService : ISettingsService
{
    private readonly IMediator _mediator;
    private readonly ILogger<SettingsService> _logger;
    private GeneralSettings _generalSettings;

    private readonly Subject<GeneralSettings> _settingsSubject = new();

    public SettingsService(IMediator mediator, ILogger<SettingsService> logger, GeneralSettings? generalSettings = null)
    {
        _mediator = mediator;
        _logger = logger;
        _generalSettings = generalSettings ?? new GeneralSettings();
    }

    public GeneralSettings Settings => _generalSettings;

    public IObservable<GeneralSettings> ObserveSettings => _settingsSubject;

    public void Dispose()
    {
        _settingsSubject.Dispose();
    }

    public async Task UpdateAsync(GeneralSettings generalSettings)
    {
        var result = await _mediator.Send(new Application.Settings.Edit.Command { Settings = generalSettings });

        if (!result.IsSuccess)
        {
            _logger.LogError("Failed to update settings, {Error}", result.Error);
            return;
        }

        _generalSettings = generalSettings;
        _settingsSubject.OnNext(generalSettings);
    }
}