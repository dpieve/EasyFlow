using EasyFocus.Domain.Entities;
using EasyFocus.Domain.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyFocus.Resources.Mockups;

public class SessionServiceMockup : ISessionService
{
    private readonly List<Session> _sessions = [];
    private int _nextId = 1;

    public Task<Session> AddAsync(Session session)
    {
        session.Id = _nextId++;
        _sessions.Add(session);
        return Task.FromResult(session);
    }

    public Task<bool> DeleteAsync(Session session)
    {
        bool removed = _sessions.Remove(session);
        return Task.FromResult(removed);
    }

    public Task<Session?> GetSessionAsync(int id)
    {
        var found = _sessions.FirstOrDefault(s => s.Id == id);
        return Task.FromResult(found);
    }

    public Task<List<Session>> GetSessionsAsync()
    {
        return Task.FromResult(_sessions.ToList());
    }

    public Task<bool> UpdateAsync(Session session)
    {
        var index = _sessions.FindIndex(s => s.Id == session.Id);
        if (index == -1)
        {
            return Task.FromResult(false);
        }

        _sessions[index] = session;
        return Task.FromResult(true);
    }
}