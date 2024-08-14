using EasyFlow.Common;
using EasyFlow.Services;

namespace EasyFlow.Features.Settings.General;
public partial class GeneralSettingsViewModel : ViewModelBase
{
    private readonly IGeneralSettingsService _generalSettings;

    public GeneralSettingsViewModel(IGeneralSettingsService generalSettings)
    {
        _generalSettings = generalSettings;
    }

    public void Activate()
    {

    }

    public void Deactivate()
    {

    }
}
