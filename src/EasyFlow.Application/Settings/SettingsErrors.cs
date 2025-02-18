using EasyFlow.Application.Common;

namespace EasyFlow.Application.Settings;

public static class SettingsErrors
{
    public static readonly Error NotFound = new("NotFound",
       "Failed to find the settings");

    public static readonly Error BadRequest = new("BadRequest",
      "Failed to edit the settings");
}