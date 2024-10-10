using EasyFlow.Domain.Entities;

namespace EasyFlow.Domain.Repositories;

public interface ISessionsRepository
{
    public Task<int> CreateAsync(Session session);

    public Task<int> UpdateAsync(Session session);

    public Task<List<Session>> GetAllAsync();

    public Task<int> DeleteAsync(int sessionId);

    public Task<bool> EditAsync(Session session);
}