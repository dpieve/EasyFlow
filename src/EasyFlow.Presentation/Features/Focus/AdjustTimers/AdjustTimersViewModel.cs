using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using EasyFlow.Application.Common;
using EasyFlow.Application.Settings;
using EasyFlow.Application.Tags;
using EasyFlow.Domain.Entities;
using EasyFlow.Presentation.Common;
using EasyFlow.Presentation.Features.Focus.RunningTimer;
using EasyFlow.Presentation.Services;
using MediatR;
using ReactiveUI;
using SimpleRouter;
using SukiUI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace EasyFlow.Presentation.Features.Focus.AdjustTimers;

public sealed partial class AdjustTimersViewModel : ViewModelBase, IRoute, IActivatableRoute
{
    private readonly IMediator _mediator;
    private readonly ILanguageService _languageService;

    [ObservableProperty] private Tag? _selectedTag;

    public AdjustTimersViewModel(
        GeneralSettings generalSettings,
        IRouterHost routerHost,
        IMediator mediator,
        ILanguageService languageService)
    {
        RouterHost = routerHost ?? throw new ArgumentNullException(nameof(routerHost));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _languageService = languageService;

        Timers = new TimersViewModel(_mediator, generalSettings, languageService);
        
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
        Observable
            .StartAsync(GetTags)
            .Where(result => result.IsSuccess)
            .Select(result => result.Value)
            .InvokeCommand(ReloadCommand);
    }

    void IActivatableRoute.OnDeactivated()
    {
    }

    [RelayCommand]
    private async Task Reload(List<Tag> tags)
    {
        Tags.Clear();
        Tags.AddRange(tags);

        var result = await _mediator.Send(new GetSettingsQuery());
        var selectedTagId = result.Value.SelectedTagId;
        SelectedTag = Tags.First(tag => tag.Id == selectedTagId);
    }

    [RelayCommand]
    private void StepForward(TimerType timerType)
    {
        Timers.Adjust(timerType, AdjustFactor.StepForward);
    }

    [RelayCommand]
    private void StepBackward(TimerType timerType)
    {
        Timers.Adjust(timerType, AdjustFactor.StepBackward);
    }

    [RelayCommand]
    private void LongStepForward(TimerType timerType)
    {
        Timers.Adjust(timerType, AdjustFactor.LongStepForward);
    }

    [RelayCommand]
    private void LongStepBackward(TimerType timerType)
    {
        Timers.Adjust(timerType, AdjustFactor.LongStepBackward);
    }

    [RelayCommand]
    private void Start()
    {
        RouterHost.Router.NavigateTo<RunningTimerViewModel>(RouterHost, _mediator, _languageService);
    }

    [RelayCommand]
    private void OpenLongBreakSettings()
    {
        SukiHost.ShowDialog(new LongBreakSettingsViewModel(Timers!.SessionsBeforeLongBreak, (int longBreakSessions) =>
        {
            Timers.SessionsBeforeLongBreak = longBreakSessions;
        }),
        allowBackgroundClose: true);
    }

    [RelayCommand]
    private async Task TagSelected(Tag tag)
    {
        var result = await _mediator.Send(new GetSettingsQuery());
        var settings = result.Value;
        settings.SelectedTagId = tag.Id;
        settings.SelectedTag = tag;

        _ = await _mediator.Send(new UpdateSettingsCommand() { GeneralSettings = settings });
    }
    private async Task<Result<List<Tag>>> GetTags()
    {
        return await _mediator.Send(new GetTagsQuery());
    }
}