using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasyFlow.Common;
using EasyFlow.Data;
using EasyFlow.Features.Focus.RunningTimer;
using EasyFlow.Services;
using ReactiveUI;
using SimpleRouter;
using SukiUI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace EasyFlow.Features.Focus.AdjustTimers;

public sealed partial class AdjustTimersViewModel : ViewModelBase, IRoute, IActivatableRoute
{
    private readonly ITagService _tagService;
    private readonly IGeneralSettingsService _generalSettingsService;

    [ObservableProperty]
    private Tag? _selectedTag;

    public AdjustTimersViewModel(
        IRouterHost routerHost,
        ITagService? tagService = null,
        IGeneralSettingsService? generalSettingsService = null)
    {
        RouterHost = routerHost ?? throw new ArgumentNullException(nameof(routerHost));
        _tagService = tagService ?? throw new ArgumentNullException(nameof(tagService));
        _generalSettingsService = generalSettingsService ?? throw new ArgumentNullException(nameof(generalSettingsService));

        Timers = new(_generalSettingsService);

        this.WhenAnyValue(vm => vm.SelectedTag)
            .WhereNotNull()
            .Skip(1)
            .InvokeCommand(TagSelectedCommand);
    }

    public TimersViewModel Timers { get; }
    public ObservableCollection<Tag> Tags { get; } = [];
    public string RouteName => nameof(AdjustTimersViewModel);
    public IRouterHost RouterHost { get; }

    void IActivatableRoute.OnActivated()
    {
        var result = _tagService.GetAll();
        if (result.Error is not null)
        {
            SukiHost.ShowToast("Failed to load", "Failed to load the tags");
        }
        else
        {
            var tags = result.Value!;
            Reload(tags);
        }

        Debug.WriteLine("Activated AdjustTimersViewModel");
    }

    void IActivatableRoute.OnDeactivated()
    {
        Debug.WriteLine("Deactivated AdjustTimersViewModel");
    }

    [RelayCommand]
    private void IncreaseTimer(TimerType timerType)
    {
        Timers.Adjust(timerType, AdjustFactor.Increase);
    }

    [RelayCommand]
    private void DecreaseTimer(TimerType timerType)
    {
        Timers.Adjust(timerType, AdjustFactor.Decrease);
    }

    [RelayCommand]
    private void Start()
    {
        RouterHost.Router.NavigateTo<RunningTimerViewModel>(RouterHost);
    }

    [RelayCommand]
    private void OpenLongBreakSettings()
    {
        SukiHost.ShowDialog(new LongBreakSettingsViewModel(Timers.SessionsBeforeLongBreak, (int longBreakSessions) =>
        {
            Timers.SessionsBeforeLongBreak = longBreakSessions;
        }), 
        allowBackgroundClose: false);
    }

    [RelayCommand]
    private async Task TagSelected(Tag? tag)
    {
        if (tag is null)
        {
            return;
        }

        var result = await _generalSettingsService.UpdateSelectedTagAsync(tag);

        if (result.Error is not null)
        {
            await SukiHost.ShowToast("Failed to update", $"Failed to save the new updated selected tag. {result.Error.Message!}", SukiUI.Enums.NotificationType.Error);
        }
    }

    private void Reload(List<Tag> tags)
    {
        Tags.Clear();
        foreach (var tag in tags)
        {
            Tags.Add(tag);
        }

        InitialTagSelection();
    }

    private void InitialTagSelection()
    {
        var result = _generalSettingsService.GetSelectedTag();
        if (result.Error is not null)
        {
            SukiHost.ShowToast("Failed to save", $"Failed to load the settings. {result.Error.Message!}");
            return;
        }
        var selectedTag = result.Value!;
        SelectedTag = Tags.FirstOrDefault(tag => tag.Id == selectedTag.Id);
        Debug.WriteLine($"Selected tag {SelectedTag?.Name}");
    }

}