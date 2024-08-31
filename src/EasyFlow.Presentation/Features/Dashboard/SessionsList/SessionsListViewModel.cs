using CommunityToolkit.Mvvm.ComponentModel;
using EasyFlow.Domain.Entities;
using EasyFlow.Presentation.Common;
using EasyFlow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace EasyFlow.Presentation.Features.Dashboard.SessionsList;

public sealed partial class SessionsListViewModel : ViewModelBase
{
    private readonly ILanguageService _languageService;

    [ObservableProperty]
    private bool _isSessionsListVisible;

    public SessionsListViewModel(ILanguageService languageService)
    {
        _languageService = languageService;
    }

    public ObservableCollection<SessionListItem> Items { get; } = [];

    public void Update(List<Session> sessions)
    {
        Items.Clear();
        foreach (var session in sessions)
        {
            if (session.Tag is null)
            {
                continue;
            }

            var item = new SessionListItem(session);
            Items.Add(item);
        }
    }
}

public sealed class SessionListItem
{
    public SessionListItem(Session session)
    {
        ConclusionDate = session.FinishedDate;
        SessionType = session.SessionType;
        Tag = session.Tag ?? throw new ArgumentNullException(nameof(session));
        Duration = session.DurationMinutes;
        Description = session.Description;
    }

    public DateTime ConclusionDate { get; }
    public SessionType SessionType { get; }
    public Tag Tag { get; }
    public int Duration { get; }
    public string Description { get; }

}