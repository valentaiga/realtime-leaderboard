using Common.Primitives;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace Common.Grpc.Server.Interceptors;

public class ServerErrorHandlerInterceptor(ILoggerFactory loggerFactory) : Interceptor
{
    private readonly ILogger<ServerErrorHandlerInterceptor> _logger = loggerFactory.CreateLogger<ServerErrorHandlerInterceptor>();

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (OperationCanceledException)
        {
            throw new RpcException(new Status(StatusCode.Cancelled, "Operation cancelled"), string.Empty);
        }
        catch (BusinessException exc)
        {
            throw new RpcException(new Status(ToErrorStatusCode(exc.ErrorCode), exc.Message), string.Empty);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, exc.Message);
            throw new RpcException(new Status(StatusCode.Unavailable, "Service unavailable"), string.Empty);
        }
    }

    private static StatusCode ToErrorStatusCode(int errorCode)
    {
        var statusCode = (StatusCode)errorCode;
        return statusCode == StatusCode.OK ? StatusCode.Unknown : statusCode;
    }
}