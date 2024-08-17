using EasyFlow.Common;
using EasyFlow.Data;
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
}
public sealed class TagService : ITagService
{
    public async Task<Result<int, Error>> CreateAsync(Tag tag)
    {
        using var context = new AppDbContext();
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
        _ = RemoveSelection(tag);

        using var context = new AppDbContext();

        var numTags = context.Tags.Count();
        if (numTags <= 1)
        {
            return TagServiceErrors.CannotDeleteLessThanTwo;
        }

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
        using var context = new AppDbContext();
        var tags = context.Tags.ToList();
        return tags;
    }

    public async Task<Result<int, Error>> UpdateAsync(Tag tag)
    {
        using var context = new AppDbContext();
        context.Tags.Update(tag);
        var result = await context.SaveChangesAsync();
        if (result == 0)
        {
            return TagServiceErrors.NoEntityModified;
        }
        return result;
    }

    private async Task<bool> RemoveSelection(Tag tag)
    {
        using var context = new AppDbContext();

        var settings = context.GeneralSettings.FirstOrDefault();
        if (settings is null)
        {
            return false;
        }
        var selectedTagId = settings.SelectedTagId;
        if (selectedTagId != tag.Id)
        {
            return false;
        }

        var firstTag = context.Tags.FirstOrDefault();
        settings.SelectedTag = firstTag!;
        settings.SelectedTagId = firstTag!.Id;
        var result = await context.SaveChangesAsync();
        return result != 0;
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
