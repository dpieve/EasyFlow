using EasyFlow.Common;
using EasyFlow.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyFlow.Services;

public interface ITagService
{
    public Task<Result<int, Error>> CreateAsync(Tag tag);

    public Task<Result<int, Error>> DeleteAsync(Tag tag);

    public Task<Result<int, Error>> UpdateAsync(Tag tag);

    public Result<List<Tag>, Error> GetAll();

    public Task<Result<int, Error>> CountSessions(int tagId, SessionType? sessionType = null);
}

public sealed class TagService : ITagService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public TagService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<Result<int, Error>> CreateAsync(Tag tag)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        _ = await context.Tags.AddAsync(tag);
        var result = await context.SaveChangesAsync();
        if (result == 0)
        {
            return TagServiceErrors.NoEntityModified;
        }
        return result;
    }

    public async Task<Result<int, Error>> DeleteAsync(Tag tag)
    {
        var canDeleteTag = await CanDeleteTag();
        if (!canDeleteTag)
        {
            return TagServiceErrors.CannotDeleteLessThanTwo;
        }

        _ = RemoveSelection(tag);

        using var context = await _contextFactory.CreateDbContextAsync();

        _ = context.Tags.Remove(tag);

        var result = await context.SaveChangesAsync();
        if (result == 0)
        {
            return TagServiceErrors.NoEntityModified;
        }
        return result;
    }

    public Result<List<Tag>, Error> GetAll()
    {
        using var context = _contextFactory.CreateDbContext();
        var tags = context.Tags.Include(t => t.Sessions).ToList();
        return tags;
    }

    public async Task<Result<int, Error>> UpdateAsync(Tag tag)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.Tags.Update(tag);
        var result = await context.SaveChangesAsync();
        if (result == 0)
        {
            return TagServiceErrors.NoEntityModified;
        }
        return result;
    }

    public async Task<Result<int, Error>> CountSessions(int tagId, SessionType? sessionType = null)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var tag = await context.Tags
                    .Where(t => t.Id == tagId)
                    .Include(t => t.Sessions)
                    .FirstOrDefaultAsync();

        if (tag is null)
        {
            return TagServiceErrors.NotFound;
        }

        if (sessionType is null)
        {
            return tag.Sessions.Count;
        }

        return tag.Sessions.Count(s => s.SessionType == sessionType);
    }

    private async Task<bool> RemoveSelection(Tag tag)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var settings = await context.GeneralSettings.FirstOrDefaultAsync();
        if (settings is null)
        {
            return false;
        }
        var selectedTagId = settings.SelectedTagId;
        if (selectedTagId != tag.Id)
        {
            return false;
        }

        var firstTag = await context.Tags.FirstOrDefaultAsync(t => t.Id != tag.Id);
        settings.SelectedTag = firstTag!;
        settings.SelectedTagId = firstTag!.Id;
        var result = await context.SaveChangesAsync();
        return result != 0;
    }

    private async Task<bool> CanDeleteTag()
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var numTags = await context.Tags.CountAsync();
        if (numTags <= 1)
        {
            return false;
        }
        return true;
    }
}

public static class TagServiceErrors
{
    public static readonly Error NoEntityModified = new("Tag.NoEntityModified",
       "Tag was not modified.");

    public static readonly Error NotFound = new("Tag.NotFound",
      "No tag found.");

    public static readonly Error CannotDeleteLessThanTwo = new("Tag.CannotDeleteLessThanTwo",
      "There must exist at least one tag.");
}