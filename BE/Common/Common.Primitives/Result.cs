namespace Common.Primitives;

public readonly struct Result<T>
{
    public readonly T? Data;
    public readonly string? ErrorMessage;
    public readonly int? ErrorCode;
    public bool IsSuccess => ErrorMessage is null;

    internal Result(T? data, string? errorMessage, int? errorCode)
    {
        Data = data;
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
    }
}

public readonly struct Result
{
    public readonly string? ErrorMessage;
    public readonly int? ErrorCode;

    private Result(string? errorMessage, int? errorCode)
    {
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
    }

    public bool IsSuccess => ErrorMessage is null;

    public static Result<T> Error<T>(string errorMessage, int errorCode) => new(default, errorMessage, errorCode);
    public static Result<T> Success<T>(T data) => new(data, null, null);
    public static Result Success() => new(null, null);
    public static Result Error(string errorMessage, int errorCode) => new(errorMessage, errorCode);
}