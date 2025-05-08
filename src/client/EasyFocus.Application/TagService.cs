using EasyFocus.Domain.Entities;
using EasyFocus.Domain.Repositories;
using EasyFocus.Domain.Services;

namespace EasyFocus.Application;

public sealed class TagService : ITagService
{
    private readonly IAppRepository _appRepository;

    public TagService(IAppRepository appData)
    {
        _appRepository = appData;
    }

    public Task<Tag> AddTagAsync(Tag tag)
    {
        tag.Id = _appRepository.GetNextTagId();
        _appRepository.AddTag(tag);
        _appRepository.SaveData();
        return Task.FromResult(tag);
    }

    public Task<bool> DeleteTagAsync(Tag tag)
    {
        var result = _appRepository.DeleteTag(tag);
        _appRepository.SaveData();
        return Task.FromResult(result);
    }

    public bool DeleteTag(Tag tag)
    {
        var result = _appRepository.DeleteTag(tag);
        _appRepository.SaveData();
        return true;
    }

    public Task<Tag?> GetTagAsync(int id)
    {
        var tags = _appRepository.GetTags();
        var tag = tags.Find(t => t.Id == id);
        return Task.FromResult(tag);
    }

    public Task<List<Tag>> GetTagsAsync()
    {
        return Task.FromResult(_appRepository.GetTags());
    }

    public Task<bool> UpdateTagAsync(Tag tag)
    {
        var tags = _appRepository.GetTags();
        var index = tags.FindIndex(t => t.Id == tag.Id);
        if (index >= 0)
        {
            _appRepository.UpdateTag(index, tag);
            _appRepository.SaveData();
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
}