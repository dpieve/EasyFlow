using Avalonia.Styling;
using EasyFlow.Domain.Entities;
using SukiUI.Models;

namespace EasyFlow.Presentation.Common;
public static class ThemeExtensions
{
    public static Theme ToTheme(this ThemeVariant themeVariant)
    {
        if (themeVariant.ToString() == "Dark")
        {
            return Theme.Dark;
        }
        return Theme.Light;
    }

    public static ColorTheme ToColorTheme(this SukiColorTheme sukiColorTheme)
    {
        return sukiColorTheme.DisplayName switch
        {
            "Orange" => ColorTheme.Orange,
            "Red" => ColorTheme.Red,
            "Green" => ColorTheme.Green,
            "Blue" => ColorTheme.Blue,
            _ => ColorTheme.Red
        };
    }

    public static ThemeVariant ToThemeVariant(this Theme theme)
    {
        return theme switch
        {
            Theme.Dark => ThemeVariant.Dark,
            Theme.Light => ThemeVariant.Light,
            _ => ThemeVariant.Dark
        };
    }
}
