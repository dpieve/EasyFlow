using EasyFlow.Desktop.Common;
using EasyFlow.Domain.Entities;
using MediatR;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace EasyFlow.Desktop.Features.Dashboard.SessionsList;

public sealed partial class SessionsListViewModel : ViewModelBase
{
    [Reactive]
    private bool _isSessionsListVisible;
    private readonly IMediator _mediator;

    public SessionsListViewModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public ObservableCollection<SessionListItem> Items { get; } = [];

    public void Update(List<Session> sessions)
    {
        Items.Clear();

        sessions.Sort((s1, s2) => s2.FinishedDate.CompareTo(s1.FinishedDate));

        foreach (var session in sessions)
        {
            if (session.Tag is null)
            {
                continue;
            }

            var item = new SessionListItem(session, OnDeleteRow, _mediator);
            Items.Add(item);
        }
    }

    public async Task OnDeleteRow(int sessionId)
    {
        var result = await _mediator.Send(new Application.Sessions.Delete.Command() { SessionId = sessionId });
        if (!result.IsSuccess)
        {
            return;
        }

        var item = Items.FirstOrDefault(i => i.Session.Id == sessionId);
        if (item is null)
        {
            return;
        }

        Items.Remove(item);
    }
}

public sealed partial class SessionListItem : ViewModelBase
{
    private readonly Func<int,Task>? _deletedRow;
    private readonly IMediator _mediator;

    [Reactive] private bool _isEditing;

    [Reactive] private string _description;

    [Reactive] private string _typingDescription;

    public SessionListItem(Session session, Func<int, Task>? deletedRow, IMediator mediator)
    {
        Session = session;
        _deletedRow = deletedRow;
        _mediator = mediator;
        
        ConclusionDate = session.FinishedDate;
        SessionType = session.SessionType;
        Tag = session.Tag ?? throw new ArgumentNullException(nameof(session));
        Duration = session.DurationMinutes;
        Description = session.Description;
        TypingDescription = session.Description;

        this.WhenAnyValue(vm => vm.IsEditing)
            .Skip(1)
            .Where(e => !e)
            .Select(_ => System.Reactive.Unit.Default)
            .InvokeCommand(UpdateSessionCommand);
    }
    public Session Session { get; init; }
    public DateTime ConclusionDate { get; }
    public SessionType SessionType { get; }
    public Tag Tag { get; }
    public int Duration { get; }

    [ReactiveCommand]
    private async Task DeleteRow()
    {
        if (_deletedRow is null)
        {
            return;
        }

        await _deletedRow(Session.Id);
    }

    [ReactiveCommand]
    private void EditDescription()
    {
        TypingDescription = Description;
        IsEditing = true;
    }

    [ReactiveCommand]
    private async Task UpdateSession()
    {
        if (string.IsNullOrEmpty(TypingDescription) 
            || string.IsNullOrWhiteSpace(TypingDescription)
            || Description == TypingDescription)
        {
            return;
        }

        Description = TypingDescription;
        Session.Description = Description;

        _ = await _mediator.Send(new Application.Sessions.Edit.Command() { Session = Session });
    }
}