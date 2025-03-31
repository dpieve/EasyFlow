using EasyFocus.Domain.Entities;

namespace EasyFocus.Domain.Services;

public interface ISessionService
{
    public Task<List<Session>> GetSessionsAsync();

    public Task<Session?> GetSessionAsync(int id);

    public Task<bool> DeleteAsync(Session session);

    public Task<Session> AddAsync(Session session);

    public Task<bool> UpdateAsync(Session session);
}