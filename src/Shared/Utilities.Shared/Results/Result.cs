namespace Utilities.Shared.Results;

/// <summary>
/// Represents the result of an operation, which can be either a success with a value or a failure with an error.
/// </summary>
public class Result<T>
{
    public T? Value { get; }
    public bool IsSuccess { get; }
    public Error? Error { get; }

    private Result(T? value, bool isSuccess, Error? error)
    {
        Value = value;
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result<T> Success(T value) => new Result<T>(value, true, null);
    public static Result<T> Failure(Error error) => new Result<T>(default, false, error);
}
