using EasyFocus.Domain.Entities;
using EasyFocus.Domain.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyFocus.Resources.Mockups;
public class TagServiceMockup : ITagService
{
    private readonly List<Tag> _tags = new();
    private int _nextId = 1;

    public Task<Tag> AddTagAsync(Tag tag)
    {
        tag.Id = _nextId++;
        _tags.Add(tag);
        return Task.FromResult(tag);
    }

    public bool DeleteTag(Tag tag)
    {
        return _tags.Remove(tag);
    }

    public Task<bool> DeleteTagAsync(Tag tag)
    {
        bool removed = _tags.Remove(tag);
        return Task.FromResult(removed);
    }

    public Task<Tag?> GetTagAsync(int id)
    {
        var found = _tags.FirstOrDefault(t => t.Id == id);
        return Task.FromResult(found);
    }

    public Task<List<Tag>> GetTagsAsync()
    {
        return Task.FromResult(_tags.ToList());
    }

    public Task<bool> UpdateTagAsync(Tag tag)
    {
        var index = _tags.FindIndex(t => t.Id == tag.Id);
        if (index == -1)
        {
            return Task.FromResult(false);
        }

        _tags[index] = tag;
        return Task.FromResult(true);
    }
}

