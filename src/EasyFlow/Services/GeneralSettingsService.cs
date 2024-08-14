using EasyFlow.Common;
using EasyFlow.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EasyFlow.Services;

public interface IGeneralSettingsService
{
    public Task<Result<int, Error>> CreateAsync(GeneralSettings settings);

    public Task<Result<int, Error>> DeleteAsync(GeneralSettings settings);

    public Task<Result<int, Error>> UpdateAsync(GeneralSettings settings);

    public Result<GeneralSettings, Error> Get();
}

public class GeneralSettingsService : IGeneralSettingsService
{
    public async Task<Result<int, Error>> CreateAsync(GeneralSettings settings)
    {
        using var context = new AppDbContext();
        var added = await context.GeneralSettings.AddAsync(settings);
        var result = await context.SaveChangesAsync();
        if (result == 0)
        {
            return GeneralSettingsServiceErrors.NoEntityModified;
        }
        return result;
    }

    public async Task<Result<int, Error>> DeleteAsync(GeneralSettings settings)
    {
        using var context = new AppDbContext();
        var removed = context.GeneralSettings.Remove(settings);
        var result = await context.SaveChangesAsync();
        if (result == 0)
        {
            return GeneralSettingsServiceErrors.NoEntityModified;
        }
        return result;
    }

    public Result<GeneralSettings, Error> Get()
    {
        using var context = new AppDbContext();
        var settings = context.GeneralSettings.FirstOrDefault();

        if (settings is null)
        {
            return GeneralSettingsServiceErrors.NotFound;
        }
        return settings;
    }

    public async Task<Result<int, Error>> UpdateAsync(GeneralSettings settings)
    {
        using var context = new AppDbContext();
        context.GeneralSettings.Update(settings);
        var result = await context.SaveChangesAsync();
        if (result == 0)
        {
            return GeneralSettingsServiceErrors.NoEntityModified;
        }
        return result;
    }
}

public static class GeneralSettingsServiceErrors
{
    public static readonly Error NoEntityModified = new("GeneralSettings.NoEntityModified",
       "Settings was not modified.");

    public static readonly Error NotFound = new("GeneralSettings.NotFound",
      "No settings found.");
}