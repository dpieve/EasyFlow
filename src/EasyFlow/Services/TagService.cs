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
        var added = await context.Tags.AddAsync(tag);
        var result = await context.SaveChangesAsync();
        if (result == 0)
        {
            return TagServiceErrors.NoEntityModified;
        }
        return result;
    }

    public async Task<Result<int, Error>> DeleteAsync(Tag tag)
    {
        using var context = new AppDbContext();
        var removed = context.Tags.Remove(tag);
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
}

public static class TagServiceErrors
{
    public static readonly Error NoEntityModified = new("Tag.NoEntityModified",
       "Tag was not modified.");

    public static readonly Error NotFound = new("Tag.NotFound",
      "No tag found.");
}
