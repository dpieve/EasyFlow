using System.Collections.Generic;

namespace EasyFlow.Features.Dashboard;

public sealed record FilterPeriod(int NumDays, string Text)
{
    public static readonly FilterPeriod Hours48 = new(NumDays: 2, "Past 48 hours");
    public static readonly FilterPeriod Days7 = new(NumDays: 7, "Past 7 days");
    public static readonly FilterPeriod Days30 = new(NumDays: 30, "Past 30 days");
    public static readonly FilterPeriod Days90 = new(NumDays: 90, "Past 90 days");
    public static readonly FilterPeriod Year1 = new(NumDays: 365, "Past year");
    public static readonly FilterPeriod Year5 = new(NumDays: 5 * 365, "Past 5 years");

    public static readonly IReadOnlyList<FilterPeriod> Filters = new List<FilterPeriod>
    {
        Hours48,
        Days7,
        Days30,
        Days90,
        Year1,
        Year5
    };
};