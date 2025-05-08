using EasyFocus.Domain.Entities;
using EasyFocus.Domain.Repositories;
using EasyFocus.Domain.Services;

namespace EasyFocus.Application;

public sealed class SessionService : ISessionService
{
    private readonly IAppRepository _appRepository;

    public SessionService(IAppRepository appRepository)
    {
        _appRepository = appRepository;
    }

    public async Task<Session> AddAsync(Session session)
    {
        session.Id = _appRepository.GetNextSessionId();
        _appRepository.AddSession(session);
        await _appRepository.SaveData();
        return session;
    }

    public async Task<bool> DeleteAsync(Session session)
    {
        var result = _appRepository.DeleteSession(session);
        await _appRepository.SaveData();
        return result;
    }

    public Task<Session?> GetSessionAsync(int id)
    {
        var sessions = _appRepository.GetSessions();
        var session = sessions.Find(s => s.Id == id);
        return Task.FromResult(session);
    }

    public Task<List<Session>> GetSessionsAsync()
    {
        return Task.FromResult(_appRepository.GetSessions());
    }

    public async Task<bool> UpdateAsync(Session session)
    {
        var sessions = _appRepository.GetSessions();
        var index = sessions.FindIndex(s => s.Id == session.Id);
        if (index >= 0)
        {
            _appRepository.UpdateSession(index, session);
            await _appRepository.SaveData();
            return true;
        }
        return false;
    }
}
