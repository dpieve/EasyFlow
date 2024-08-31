namespace EasyFlow.Presentation.Features.Dashboard.DisplayControls;

// TODO: Translate
public static class DisplayTypeExtensions
{
    public static string ToCustomString(this DisplayType type)
    {
        return type switch
        {
            DisplayType.BarChart => "Bar Chart",
            DisplayType.SessionsList => "Sessions List",
            _ => string.Empty,
        };
    }

    public static DisplayType DisplayTypeFromString(this string text)
    {
        if (text == "Bar Chart")
        {
            return DisplayType.BarChart;
        }
        if (text == "Sessions List")
        {
            return DisplayType.SessionsList;
        }

        return DisplayType.BarChart;
    }
}
