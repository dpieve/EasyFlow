using EasyFlow.Common;
using EasyFlow.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EasyFlow.Services;

public interface IGeneralSettingsService
{
    public Result<GeneralSettings, Error> Get();

    public Task<Result<int, Error>> CreateAsync(GeneralSettings settings);

    public Task<Result<int, Error>> DeleteAsync(GeneralSettings settings);

    public Task<Result<int, Error>> UpdateAsync(GeneralSettings settings);

    public Task<Result<bool, Error>> UpdateSelectedTagAsync(Tag tag);

    public Result<Tag, Error> GetSelectedTag();
    public void UpdateSelectedTheme(Theme theme);
    public void UpdateSelectedColorTheme(ColorTheme colorTheme);

    public bool IsFocusDescriptionEnabled();

    public Task<Result<bool, Error>> UpdateSelectedLanguage(SupportedLanguage selectedLanguage);
    public Result<SupportedLanguage, Error> GetSelectedLanguage();
}

public class GeneralSettingsService : IGeneralSettingsService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    public GeneralSettingsService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public Result<GeneralSettings, Error> Get()
    {
        using var context = _contextFactory.CreateDbContext();
        var settings = context.GeneralSettings.FirstOrDefault();

        if (settings is null)
        {
            return GeneralSettingsServiceErrors.NotFound;
        }
        return settings;
    }

    public async Task<Result<int, Error>> CreateAsync(GeneralSettings settings)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        _ = await context.GeneralSettings.AddAsync(settings);
        var result = await context.SaveChangesAsync();
        if (result == 0)
        {
            return GeneralSettingsServiceErrors.NoEntityModified;
        }
        return result;
    }

    public async Task<Result<int, Error>> DeleteAsync(GeneralSettings settings)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        _ = context.GeneralSettings.Remove(settings);
        var result = await context.SaveChangesAsync();
        if (result == 0)
        {
            return GeneralSettingsServiceErrors.NoEntityModified;
        }
        return result;
    }

    public async Task<Result<int, Error>> UpdateAsync(GeneralSettings settings)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.GeneralSettings.Update(settings);
        var result = await context.SaveChangesAsync();
        if (result == 0)
        {
            return GeneralSettingsServiceErrors.NoEntityModified;
        }
        return result;
    }

    public async Task<Result<bool, Error>> UpdateSelectedTagAsync(Tag tag)
    {
        if (tag is null)
        {
            return GeneralSettingsServiceErrors.InvalidArgument;
        }

        using var context = await _contextFactory.CreateDbContextAsync();
        var settings = await context.GeneralSettings.FirstOrDefaultAsync();
        if (settings is null)
        {
            return GeneralSettingsServiceErrors.NotFound;
        }

        settings.SelectedTagId = tag.Id;
        settings.SelectedTag = tag;
        context.GeneralSettings.Update(settings);

        var result = await context.SaveChangesAsync();

        if (result == 0)
        {
            return GeneralSettingsServiceErrors.NoEntityModified;
        }

        return true;
    }

    public Result<Tag, Error> GetSelectedTag()
    {
        using var context = _contextFactory.CreateDbContext();
        var settings = context.GeneralSettings.FirstOrDefault();

        if (settings is null)
        {
            return GeneralSettingsServiceErrors.NotFound;
        }

        var tagId = settings.SelectedTagId;
        var tag = context.Tags.FirstOrDefault(t => t.Id == tagId);
        if (tag is null)
        {
            return GeneralSettingsServiceErrors.NotFound;
        }

        return tag;
    }

    public void UpdateSelectedTheme(Theme theme)
    {
        using var context = _contextFactory.CreateDbContext();
        var settings = context.GeneralSettings.FirstOrDefault();
        if (settings is null)
        {
            return;
        }

        settings.SelectedTheme = theme;
        context.GeneralSettings.Update(settings);
        _ = context.SaveChanges();
    }

    public void UpdateSelectedColorTheme(ColorTheme colorTheme)
    {
        using var context = _contextFactory.CreateDbContext();
        var settings = context.GeneralSettings.FirstOrDefault();
        if (settings is null)
        {
            return;
        }

        settings.SelectedColorTheme = colorTheme;
        context.GeneralSettings.Update(settings);
        _ = context.SaveChanges();
    }

    public bool IsFocusDescriptionEnabled()
    {
        using var context = _contextFactory.CreateDbContext();
        var settings = context.GeneralSettings.FirstOrDefault();
        if (settings is null)
        {
            return false;
        }
        return settings.IsFocusDescriptionEnabled;
    }

    public async Task<Result<bool, Error>> UpdateSelectedLanguage(SupportedLanguage selectedLanguage)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var settings = await context.GeneralSettings.FirstOrDefaultAsync();
        if (settings is null)
        {
            return GeneralSettingsServiceErrors.NotFound;
        }

        settings.SelectedLanguage = selectedLanguage.Code;
        context.GeneralSettings.Update(settings);
        var result = await context.SaveChangesAsync();
        if (result == 0)
        {
            return GeneralSettingsServiceErrors.NoEntityModified;
        }

        return result != 0;
    }

    public Result<SupportedLanguage, Error> GetSelectedLanguage()
    {
        using var context = _contextFactory.CreateDbContext();
        var settings = context.GeneralSettings.FirstOrDefault();
        if (settings is null)
        {
            return GeneralSettingsServiceErrors.NotFound;
        }

        var language = SupportedLanguage.FromCode(settings.SelectedLanguage);
        return language;
    }
}

public static class GeneralSettingsServiceErrors
{
    public static readonly Error NoEntityModified = new("GeneralSettings.NoEntityModified",
       "Settings was not modified.");

    public static readonly Error NotFound = new("GeneralSettings.NotFound",
      "No settings found.");

    public static readonly Error InvalidArgument = new("GeneralSettings.InvalidArgument",
      "The parameters are wrong.");
}