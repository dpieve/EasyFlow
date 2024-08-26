namespace EasyFlow.Domain.Repositories;
public interface IDatabaseManagerRepository
{
    public Task MigrateAsync();
    public void Migrate();
    public bool Reset();
}