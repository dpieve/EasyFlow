using EasyFlow.Domain.Entities;
using Serilog;
using System;
using System.Globalization;
using System.Threading;

namespace EasyFlow.Desktop.Services;

public interface ILanguageService
{
    public event Action LanguageChanged;

    public string GetString(string key);

    public void SetLanguage(SupportedLanguage language);
}

public class LanguageService : ILanguageService
{
    public event Action? LanguageChanged;

    public string GetString(string key)
    {
        var value = Assets.Resources.ResourceManager.GetString(key);
        if (value is null)
        {
            Log.Error("Failed to find translation for key: {Key}", key);
        }
        return value ?? string.Empty;
    }

    public void SetLanguage(SupportedLanguage language)
    {
        var culture = new CultureInfo(language.Code);
        Thread.CurrentThread.CurrentUICulture = culture;
        Thread.CurrentThread.CurrentCulture = culture;
        Assets.Resources.Culture = new CultureInfo(language.Code);
        OnLanguageChanged();
    }

    protected virtual void OnLanguageChanged()
    {
        LanguageChanged?.Invoke();
    }

    public static string GetDateFormat()
    {
        var culture = CultureInfo.CurrentCulture;

        if (culture.Name == SupportedLanguage.Portuguese.Code)
        {
            return "dd MMM yyyy";
        }
        else
        {
            return "MMM dd yyyy";
        }
    }
}

public static class ConstantTranslation
{
    public static string SideMenuFocus => Assets.Resources.SideMenuFocus;
    public static string SideMenuDashboard => Assets.Resources.SideMenuDashboard;
    public static string SideMenuSettings => Assets.Resources.SideMenuSettings;
    public static string Sessions => Assets.Resources.Sessions;
    public static string SessionsMinutes => Assets.Resources.SessionsMinutes;
    public static string VolumeSound => Assets.Resources.VolumeSound;
    public static string ChooseWhereToSave => Assets.Resources.ChooseWhereToSave;
    public static string Past48Hours => Assets.Resources.Past48Hours;
    public static string Past7Days => Assets.Resources.Past7Days;
    public static string Past30Days => Assets.Resources.Past30Days;
    public static string Past90Days => Assets.Resources.Past90Days;
    public static string PastYear => Assets.Resources.PastYear;
    public static string Past5Years => Assets.Resources.Past5Years;
    public static string Focus => Assets.Resources.Focus;
    public static string Break => Assets.Resources.Break;
    public static string LongBreak => Assets.Resources.LongBreak;
    public static string SkipToFocus => Assets.Resources.SkipToFocus;
    public static string SkipToBreak => Assets.Resources.SkipToBreak;
    public static string ReportColumns => Assets.Resources.ReportColumns;
    public static string Report => Assets.Resources.Report;
    public static string BarChart => Assets.Resources.BarChart;
    public static string SessionsList => Assets.Resources.SessionsList;
    public static string CanNotMoreThanMax => Assets.Resources.Tag_CanNotMoreThanMax;
    public static string ThemeDark => Assets.Resources.Dark;
    public static string ThemeLight => Assets.Resources.Light;
    public static string ThemeOrange => Assets.Resources.Orange;
    public static string ThemeRed => Assets.Resources.Red;
    public static string ThemeGreen => Assets.Resources.Green;
    public static string ThemeBlue => Assets.Resources.Blue;
}