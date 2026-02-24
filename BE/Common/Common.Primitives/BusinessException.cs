namespace Common.Primitives;

public class BusinessException : Exception
{
    public BusinessException(string errorMessage, int errorCode) : base(errorMessage)
    {
        ErrorCode = errorCode;
    }

    public BusinessException(string errorMessage, BusinessErrorCode errorCode) : base(errorMessage)
    {
        ErrorCode = (int)errorCode;
    }

    public int ErrorCode { get; }
}

public enum BusinessErrorCode
{
    NotFound = 5,
    PermissionDenied = 7,
    Unauthenticated = 10,
    Unimplemented = 12,
    InternalServerError = 14,
}
