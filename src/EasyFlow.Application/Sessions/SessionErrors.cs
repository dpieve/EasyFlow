using EasyFlow.Application.Common;

namespace EasyFlow.Application.Sessions;

public static partial class SessionsErrors
{
    public static readonly Error CreateFail = new("CreateFail",
      "Failed to create a session.");

    public static readonly Error DeleteFail = new("DeleteFail",
      "Failed to delete a session.");

    public static readonly Error EditFail = new("EditFail",
      "Failed to edit a session.");

    public static readonly Error BadRequest = new("BadRequest",
      "Failed to create a session, invalid parameters.");

    public static readonly Error NotFound = new("NotFound",
      "The session was not found.");
}