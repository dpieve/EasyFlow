using CommunityToolkit.Mvvm.ComponentModel;
using EasyFlow.Presentation.Common;
using System.Reactive.Disposables;
using SukiUI.Controls;
using MediatR;
using EasyFlow.Presentation.Services;
using EasyFlow.Presentation.Features.Dashboard.DisplayControls;
using EasyFlow.Presentation.Features.Dashboard.BarChart;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.Input;
using EasyFlow.Application.Sessions;
using System.Threading.Tasks;
using ReactiveUI;
using System.Linq;
using EasyFlow.Presentation.Features.Dashboard.SessionsList;

namespace EasyFlow.Presentation.Features.Dashboard;

public partial class DashboardViewModel : PageViewModelBase
{
    private readonly IMediator _mediator;
    private readonly ILanguageService _languageService;

    private CompositeDisposable? _disposables;

    [ObservableProperty]
    private string _infoTitle = string.Empty;

    [ObservableProperty]
    private bool _isNotFoundSessionsVisible;

    [ObservableProperty]
    private bool _isBusy;

    public DashboardViewModel(
        IMediator mediator,
        ILanguageService languageService)
        : base(ConstantTranslation.SideMenuDashboard, Material.Icons.MaterialIconKind.ChartBar, (int)PageOrder.Dashboard)
    {
        _mediator = mediator;
        _languageService = languageService;

        DisplayControls = new DisplayControlsViewModel(_mediator, _languageService);
        BarChart = new BarChartViewModel(_languageService);
        SessionsList = new SessionsListViewModel(_languageService);

        DisplayControls.ChangedControls
            .Do(display => InfoTitle = $"{display.Tag.Name} - {display.SessionType.ToCustomString()} - {display.FilterPeriod.Text} - {display.DisplayType.ToCustomString()}")
            .InvokeCommand(UpdateCommand);

        IsBusy = true;
    }

    public DisplayControlsViewModel DisplayControls { get; }
    public BarChartViewModel BarChart { get; }
    public SessionsListViewModel SessionsList { get; }

    protected override void OnActivated()
    {
        _disposables ??= [];
        DisplayControls.Activated();

        Observable.StartAsync(() => Update(DisplayControls.GetDisplayControls()));
    }

    protected override void OnDeactivated()
    {
        SukiHost.ClearAllToasts();
        DisplayControls.Deactivated();
    }

    [RelayCommand]
    private async Task Update(Display display)
    {
        IsNotFoundSessionsVisible = false;
        BarChart.IsBarChartVisible = false;
        SessionsList.IsSessionsListVisible = false;

        IsBusy = true;

        var result = await _mediator.Send(new GetSessionsByPeriodQuery() { NumDays = display.FilterPeriod.NumDays });

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
    }
}
