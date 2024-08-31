namespace EasyFlow.Domain.Repositories;

public interface IDatabaseManagerRepository
{
    public Task MigrateAsync();

    public Task<bool> ResetAsync();

    public bool Reset();
}