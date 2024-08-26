using EasyFlow.Domain.Entities;

namespace EasyFlow.Domain.Repositories;

public interface ITagsRepository
{
    public Task<int> CreateAsync(Tag tag);

    public Task<bool> DeleteAsync(Tag tag);

    public Task<bool> UpdateAsync(Tag tag);

    public Task<List<Tag>> GetAsync();

    public Task<int> CountSessionsAsync(int tagId, SessionType? sessionType = null);
}
