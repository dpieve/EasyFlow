using DynamicData;
using EasyFlow.Application.Common;
using EasyFlow.Desktop.Common;
using EasyFlow.Desktop.Features.Focus.RunningTimer;
using EasyFlow.Desktop.Services;
using EasyFlow.Domain.Entities;
using MediatR;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using SukiUI.Dialogs;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace EasyFlow.Desktop.Features.Focus.AdjustTimers;

public sealed partial class AdjustTimersViewModel : ActivatablePageViewModelBase
{
    private readonly IMediator _mediator;
    private readonly ILanguageService _languageService;
    private readonly IToastService _toastService;
    private readonly ISukiDialogManager _dialog;
    private readonly INotificationService _notificationService;

    [Reactive] private Tag? _selectedTag;

    public AdjustTimersViewModel(
        GeneralSettings generalSettings,
        IMediator mediator,
        ILanguageService languageService,
        IToastService toastService,
        ISukiDialogManager dialog,
        INotificationService notificationService,
        IScreen hostScreen,
        string urlPath = nameof(AdjustTimersViewModel))
        : base(hostScreen, urlPath)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _languageService = languageService;
        _toastService = toastService;
        _dialog = dialog;
        _notificationService = notificationService;

        Timers = new TimersViewModel(_mediator, generalSettings, _languageService, _toastService);

        this.WhenAnyValue(vm => vm.SelectedTag)
            .WhereNotNull()
            .Skip(1)
            .InvokeCommand(TagSelectedCommand);
    }

    public TimersViewModel Timers { get; }
    public ObservableCollection<Tag> Tags { get; } = [];

    public override void HandleActivation(CompositeDisposable d)
    {
        Observable
            .StartAsync(GetTags)
            .Where(result => result.IsSuccess)
            .Select(result => result.Value)
            .InvokeCommand(ReloadCommand);
    }

    [ReactiveCommand]
    private async Task Reload(List<Tag> tags)
    {
        Tags.Clear();
        Tags.AddRange(tags);

        var result = await _mediator.Send(new Application.Settings.Get.Query());
        var selectedTagId = result.Value.SelectedTagId;
        SelectedTag = Tags.First(tag => tag.Id == selectedTagId);
    }

    [ReactiveCommand]
    private void StepForward(TimerType timerType)
    {
        Timers.Adjust(timerType, AdjustFactor.StepForward);
    }

    [ReactiveCommand]
    private void StepBackward(TimerType timerType)
    {
        Timers.Adjust(timerType, AdjustFactor.StepBackward);
    }

    [ReactiveCommand]
    private void LongStepForward(TimerType timerType)
    {
        Timers.Adjust(timerType, AdjustFactor.LongStepForward);
    }

    [ReactiveCommand]
    private void LongStepBackward(TimerType timerType)
    {
        Timers.Adjust(timerType, AdjustFactor.LongStepBackward);
    }

    [ReactiveCommand]
    private async Task Start()
    {
        await HostScreen.Router.Navigate.Execute(new RunningTimerViewModel(_mediator, _languageService, _toastService, _dialog, _notificationService, HostScreen));
    }

    [ReactiveCommand]
    private void OpenLongBreakSettings()
    {
        _dialog.CreateDialog()
                .WithViewModel(dialog => new LongBreakSettingsViewModel(dialog, Timers!.SessionsBeforeLongBreak, (int longBreakSessions) =>
                {
                    Timers.SessionsBeforeLongBreak = longBreakSessions;
                }))
                .Dismiss().ByClickingBackground()
                .TryShow();
    }

    [ReactiveCommand]
    private async Task TagSelected(Tag tag)
    {
        var result = await _mediator.Send(new Application.Settings.Get.Query());
        var settings = result.Value;
        settings.SelectedTagId = tag.Id;
        settings.SelectedTag = tag;

        _ = await _mediator.Send(new Application.Settings.Edit.Command() { Settings = settings });
    }

    private async Task<Result<List<Tag>>> GetTags()
    {
        return await _mediator.Send(new Application.Tags.Get.Query());
    }
}