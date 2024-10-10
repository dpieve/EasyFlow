using EasyFlow.Desktop.Services;
using System.Collections.Generic;
using System.Linq;

namespace EasyFlow.Desktop.Features.Dashboard;

public sealed record FilterPeriod(int NumDays, string Text)
{
    public static readonly FilterPeriod Hours48 = new(NumDays: 2, ConstantTranslation.Past48Hours);
    public static readonly FilterPeriod Days7 = new(NumDays: 7, ConstantTranslation.Past7Days);
    public static readonly FilterPeriod Days30 = new(NumDays: 30, ConstantTranslation.Past30Days);
    public static readonly FilterPeriod Days90 = new(NumDays: 90, ConstantTranslation.Past90Days);
    public static readonly FilterPeriod Year1 = new(NumDays: 365, ConstantTranslation.PastYear);
    public static readonly FilterPeriod Years5 = new(NumDays: 5 * 365, ConstantTranslation.Past5Years);

    public static readonly IReadOnlyList<FilterPeriod> Filters =
    [
        Hours48,
        Days7,
        Days30,
        Days90,
        Year1,
        Years5
    ];

    public static FilterPeriod FromNumDays(int numDays) => Filters.First(x => x.NumDays == numDays);

    public static int FromFilterPeriod(FilterPeriod filterPeriod) => filterPeriod.NumDays;
};