namespace EasyFlow.Domain.Repositories;
public interface IDatabaseManagerRepository
{
    public Task MigrateAsync();
    public void Migrate();

    public Task<bool> ResetAsync();
    public bool Reset();
}