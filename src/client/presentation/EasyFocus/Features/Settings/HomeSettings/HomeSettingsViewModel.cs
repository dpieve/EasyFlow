using EasyFocus.Common;
using EasyFocus.Domain.Services;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Serilog;
using System.Reactive.Disposables;
using System.Threading.Tasks;

namespace EasyFocus.Features.Settings.HomeSettings;

public sealed partial class HomeSettingsViewModel : ViewModelBase, IActivatableViewModel
{
    private readonly IBrowserService _browserService;

    [Reactive] private string _errorMessage = string.Empty;

    public ViewModelActivator Activator { get; } = new();

    public HomeSettingsViewModel(IBrowserService browserService)
    {
        _browserService = browserService;
        this.WhenActivated(Activated);
    }
    private void Activated(CompositeDisposable d)
    {
        ErrorMessage = string.Empty;
    }


    [ReactiveCommand]
    private void OnFocusTime()
    {
        Log.Debug("On Focus Time");
    }

    [ReactiveCommand]
    private void OnNotifications()
    {
        Log.Debug("On Notifications Time");
    }

    [ReactiveCommand]
    private void OnReport()
    {
        Log.Debug("On Report Time");
    }

    [ReactiveCommand]
    private void OnTags()
    {
        Log.Debug("On Tags");
    }

    [ReactiveCommand]
    private void OnBackground()
    {
        Log.Debug("On Background");
    }

    [ReactiveCommand]
    private async Task OnSupportFeedback()
    {
        var url = "https://github.com/dpieve/EasyFocus/issues";
        bool opened = await _browserService.OpenUrlAsync(url);
        if (opened)
        {
            ErrorMessage = "Write on Github. Thank you!";
        }
        else
        {
            ErrorMessage = "Failed to open Github. Please visit " + url;
        }
    }

    public void CleanMessage()
    {
        ErrorMessage = string.Empty;
    }
}