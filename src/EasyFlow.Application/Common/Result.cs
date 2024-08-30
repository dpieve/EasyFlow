namespace EasyFlow.Application.Common;

public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public Error Error { get; }

    private Result(bool isSuccess, T? value, Error error)
    {
        IsSuccess = isSuccess;
        Value = value ?? throw new ArgumentNullException(nameof(value));
        Error = error;
    }

    public static Result<T> Success(T value) => new Result<T>(true, value, DefaultErrors.NotError);

    public static Result<T> Failure(Error error) => new Result<T>(false, default, error);
}
public sealed record Error(string Code, string? Message = null);

public static class DefaultErrors
{
    public static readonly Error NotError = new("Default.NotError",
      "There wasn't any errors");
}