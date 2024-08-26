namespace EasyFlow.Application.Common;

public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public Error Error { get; }

    private Result(bool isSuccess, T value, Error error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value) => new Result<T>(true, value, null);

    public static Result<T> Failure(Error error) => new Result<T>(false, default(T), error);
}
public sealed record Error(string Code, string? Message = null);

// Example
//public static class GeneralSettingsServiceErrors
//{
//    public static readonly Error NoEntityModified = new("GeneralSettings.NoEntityModified",
//       "Settings was not modified.");

//    public static readonly Error NotFound = new("GeneralSettings.NotFound",
//      "No settings found.");

//    public static readonly Error InvalidArgument = new("GeneralSettings.InvalidArgument",
//      "The parameters are wrong.");
//}