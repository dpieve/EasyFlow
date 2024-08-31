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

    public async Task<bool> DeleteAsync(Tag tag)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var numTags = await context.Tags.CountAsync();

        if (numTags <= Tag.MinNumTags)
        {
            return false;
        }

        var sessions = await context.Sessions.Where(s => s.TagId == tag.Id).ToListAsync();

        context.Sessions.RemoveRange(sessions);
        context.Tags.Remove(tag);

        var result = await context.SaveChangesAsync();

        if (result == 0)
        {
            return false;
        }

        return true;
    }

    public async Task<List<Tag>> GetAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var tags = await context.Tags.Include(t => t.Sessions).ToListAsync();
        return tags;
    }

    public async Task<bool> UpdateAsync(Tag tag)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        _ = context.Tags.Update(tag);

        var result = await context.SaveChangesAsync();

        if (result == 0)
        {
            return false;
        }

        return true;
    }
}