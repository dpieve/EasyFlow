using EasyFlow.Desktop.Common;
using EasyFlow.Desktop.Features.Dashboard;
using EasyFlow.Desktop.Features.Dashboard.BarChart;
using EasyFlow.Desktop.Features.Dashboard.DisplayControls;
using EasyFlow.Desktop.Features.Dashboard.SessionsList;
using EasyFlow.Desktop.Services;
using MediatR;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace EasyFlow.Desktop.Features.Dashboard;

public partial class DashboardViewModel : ActivatableSideMenuViewModelBase
{
    private readonly IMediator _mediator;
    private readonly ILanguageService _languageService;
    private readonly IToastService _toastService;
    private CompositeDisposable? _disposables;

    [Reactive]
    private string _infoTitle = string.Empty;

    [Reactive]
    private bool _isNotFoundSessionsVisible;

    [Reactive]
    private bool _isBusy;

    public DashboardViewModel(
        IMediator mediator,
        ILanguageService languageService,
        IToastService toastService)
        : base(ConstantTranslation.SideMenuDashboard, Material.Icons.MaterialIconKind.ChartBar, (int)PageOrder.Dashboard)
    {
        _mediator = mediator;
        _languageService = languageService;
        _toastService = toastService;

        DisplayControls = new DisplayControlsViewModel(_mediator, _languageService, _toastService);
        BarChart = new BarChartViewModel(_languageService);
        SessionsList = new SessionsListViewModel(mediator);

        DisplayControls.ChangedControls
            .Do(display => InfoTitle = $"{display.Tag.Name} - {display.SessionType.ToCustomString()} - {display.FilterPeriod.Text} - {display.DisplayType.ToCustomString()}")
            .InvokeCommand(UpdateCommand);

        IsBusy = true;
    }

    public DisplayControlsViewModel DisplayControls { get; }
    public BarChartViewModel BarChart { get; }
    public SessionsListViewModel SessionsList { get; }

    public override void HandleActivation(CompositeDisposable d)
    {
        _disposables ??= [];
        DisplayControls.Activated();

        Observable.StartAsync(() => Update(DisplayControls.GetDisplayControls()));

        Trace.TraceInformation("Dashboard OnActivated");
    }

    public override void HandleDeactivation()
    {
        _toastService.DismissAll();
        DisplayControls.Deactivated();

        Trace.TraceInformation("Dashboard OnDeactivated");
    }

    [ReactiveCommand]
    private async Task Update(Display display)
    {
        IsNotFoundSessionsVisible = false;
        BarChart.IsBarChartVisible = false;
        SessionsList.IsSessionsListVisible = false;

        IsBusy = true;

        var result = await _mediator.Send(new Application.Sessions.GetByPeriod.Query() { NumDays = display.FilterPeriod.NumDays });

        if (!result.IsSuccess)
        {
            IsNotFoundSessionsVisible = true;
            IsBusy = false;
            return;
        }

        var sessions = result.Value
                            .Where(s => s.SessionType == display.SessionType)
                            .Where(s => s.TagId == display.Tag.Id)
                            .ToList();

        if (sessions.Count == 0)
        {
            IsNotFoundSessionsVisible = true;
            IsBusy = false;
            return;
        }

        switch (display.DisplayType)
        {
            case DisplayType.BarChart:
                BarChart.Update(sessions);
                BarChart.IsBarChartVisible = true;
                break;

            case DisplayType.SessionsList:
                SessionsList.Update(sessions);
                SessionsList.IsSessionsListVisible = true;
                break;

            default:
                break;
        }

        IsBusy = false;

        Trace.TraceInformation("Dashboard Updated");
    }
}