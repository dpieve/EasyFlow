using EasyFlow.Desktop.Services;
using EasyFlow.Domain.Entities;

namespace EasyFlow.Desktop.Features.Dashboard;

public static class SessionTypeExtensions
{
    public static string ToCustomString(this SessionType type)
    {
        return type switch
        {
            SessionType.Focus => ConstantTranslation.Focus,
            SessionType.Break => ConstantTranslation.Break,
            SessionType.LongBreak => ConstantTranslation.LongBreak,
            _ => string.Empty,
        };
    }

    public static SessionType SessionTypeFromString(this string text)
    {
        if (text == ConstantTranslation.Focus)
        {
            return SessionType.Focus;
        }
        if (text == ConstantTranslation.Break)
        {
            return SessionType.Break;
        }

        return SessionType.LongBreak;
    }
}