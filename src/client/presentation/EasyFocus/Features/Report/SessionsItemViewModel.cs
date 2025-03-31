using EasyFocus.Common;
using EasyFocus.Domain.Entities;
using EasyFocus.Domain.Services;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace EasyFocus.Features.Report;

public sealed partial class SessionItemViewModel : ViewModelBase
{
    private readonly ISessionService _sessionService;

    [Reactive] private DateTime _finishedDate;
    [Reactive] private int _completedSeconds;
    [Reactive] private int _durationSeconds;
    [Reactive] private string _description;
    [Reactive] private SessionType _sessionType;
    [Reactive] private string _tagName;
    [Reactive] private bool _isCompleted;

    [Reactive] private bool _isEditing;
    [Reactive] private string _typingDescription = string.Empty;

    public SessionItemViewModel(Session session, ISessionService sessionService)
    {
        Session = session;
        _sessionService = sessionService;

        FinishedDate = session.FinishedDateTime;
        CompletedSeconds = session.CompletedSeconds;
        DurationSeconds = session.DurationSeconds;
        Description = session.Description;
        SessionType = session.SessionType;
        TagName = session.TagName;

        this.WhenAnyValue(vm => vm.CompletedSeconds, vm => vm.DurationSeconds,
                                    (completed, duration) => completed >= duration)
            .Subscribe(isCompleted => IsCompleted = isCompleted);

        this.WhenAnyValue(vm => vm.IsEditing)
            .Skip(1)
            .DistinctUntilChanged()
            .Where(e => !e)
            .Select(_ => Unit.Default)
            .InvokeCommand(UpdateSessionCommand);
    }

    public Session Session { get; }

    [ReactiveCommand]
    private void DeleteRow()
    {
        Debug.WriteLine("DeleteRow");
    }

    [ReactiveCommand]
    private void EditDescription()
    {
        Debug.WriteLine("EditRow");

        TypingDescription = Description;
        IsEditing = true;
    }

    [ReactiveCommand]
    private async Task UpdateSession()
    {
        Debug.WriteLine("UpdateSession");
        Description = TypingDescription;

        Session.Description = Description;
        await _sessionService.UpdateAsync(Session);
    }
}