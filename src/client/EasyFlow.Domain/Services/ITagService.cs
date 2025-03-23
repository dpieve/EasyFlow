using EasyFlow.Domain.Entities;

namespace EasyFlow.Domain.Services;

public interface ITagService
{
    public Task<List<Tag>> GetTagsAsync();
    public Task<Tag?> GetTagAsync(int id);
    public Task<bool> DeleteTagAsync(Tag tag);
    public bool DeleteTag(Tag tag); 
    public Task<Tag> AddTagAsync(Tag tag);
    public Task<bool> UpdateTagAsync(Tag tag);
}