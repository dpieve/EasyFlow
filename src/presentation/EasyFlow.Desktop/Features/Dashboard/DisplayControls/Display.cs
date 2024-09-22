using EasyFlow.Domain.Entities;

namespace EasyFlow.Desktop.Features.Dashboard.DisplayControls;
public sealed record Display(FilterPeriod FilterPeriod, Tag Tag, SessionType SessionType, DisplayType DisplayType);