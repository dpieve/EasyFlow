using EasyFlow.Domain.Entities;
using System;
using System.Globalization;
using System.Threading;

namespace EasyFlow.Presentation.Services;

public interface ILanguageService   
{
    public event Action LanguageChanged;
    public string GetString(string key);
    public void SetLanguage(SupportedLanguage language);
}

public class LanguageService : ILanguageService
{
    public event Action LanguageChanged;

    public string GetString(string key)
    {
        var value = Assets.Resources.ResourceManager.GetString(key);
        return value ?? string.Empty;
    }

    public void SetLanguage(SupportedLanguage language)
    {
        var culture = new CultureInfo(language.Code);
        Thread.CurrentThread.CurrentUICulture = culture;
        Thread.CurrentThread.CurrentCulture = culture;
        OnLanguageChanged();
    }

    protected virtual void OnLanguageChanged()
    {
        LanguageChanged?.Invoke();
    }
}

public static class  ConstantTranslation
{
    public static string SideMenuFocus => Assets.Resources.SideMenuFocus;
    public static string SideMenuDashboard => Assets.Resources.SideMenuDashboard;
    public static string SideMenuSettings => Assets.Resources.SideMenuSettings;
}
