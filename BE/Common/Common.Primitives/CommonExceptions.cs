namespace Common.Primitives;

public static class CommonExceptions
{
    public static BusinessException ServiceUnavailable => new("Service is unavailable", 500); 
}