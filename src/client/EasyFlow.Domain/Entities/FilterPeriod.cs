namespace EasyFlow.Domain.Entities;
public sealed record FilterPeriod(int NumDays, string Text)
{
    public static readonly FilterPeriod Hours48 = new(NumDays: 2, "Past 48 hours");
    public static readonly FilterPeriod Days7 = new(NumDays: 7, "Past 7 days");
    public static readonly FilterPeriod Days30 = new(NumDays: 30, "Past 30 days");
    public static readonly FilterPeriod Year1 = new(NumDays: 365, "Past year");

    public static readonly IReadOnlyList<FilterPeriod> Filters =
    [
        Hours48,
        Days7,
        Days30,
        Year1,
    ];

    public static FilterPeriod FromNumDays(int numDays) => Filters.First(x => x.NumDays == numDays);

    public static int FromFilterPeriod(FilterPeriod filterPeriod) => filterPeriod.NumDays;
};