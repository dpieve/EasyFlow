using EasyFlow.Presentation.Services;

namespace EasyFlow.Presentation.Features.Dashboard.DisplayControls;

public static class DisplayTypeExtensions
{
    public static string ToCustomString(this DisplayType type)
    {
        return type switch
        {
            DisplayType.BarChart => ConstantTranslation.BarChart,
            DisplayType.SessionsList => ConstantTranslation.SessionsList,
            _ => string.Empty,
        };
    }

    public static DisplayType DisplayTypeFromString(this string text)
    {
        if (text == ConstantTranslation.BarChart)
        {
            return DisplayType.BarChart;
        }
        if (text == ConstantTranslation.SessionsList)
        {
            return DisplayType.SessionsList;
        }

        return DisplayType.BarChart;
    }
}