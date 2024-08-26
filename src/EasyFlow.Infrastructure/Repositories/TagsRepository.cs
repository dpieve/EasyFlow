using EasyFlow.Domain.Entities;
using EasyFlow.Domain.Repositories;
using EasyFlow.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace EasyFlow.Infrastructure.Repositories;

public sealed class TagsRepository : ITagsRepository
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public TagsRepository(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public Task<int> CountSessionsAsync(int tagId, SessionType? sessionType = null)
    {
        throw new NotImplementedException();
    }

    public async Task<int> CreateAsync(Tag tag)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var numTags = await context.Tags.CountAsync();
        
        if (numTags >= Tag.MaxNumTags)
        {
            return 0;
        }
        _ = await context.Tags.AddAsync(tag);

        var result = await context.SaveChangesAsync();
        
        if (result == 0)
        {
            return 0;
        }
        
        return tag.Id;
    }

    public Task<int> DeleteAsync(Tag tag)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Tag>> GetAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var tags = await context.Tags.ToListAsync();
        return tags;
    }

    public Task<int> UpdateAsync(Tag tag)
    {
        throw new NotImplementedException();
    }
}
