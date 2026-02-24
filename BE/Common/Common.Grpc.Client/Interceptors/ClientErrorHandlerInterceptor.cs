using Common.Primitives;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Common.Grpc.Client.Interceptors;

public class ClientErrorHandlerInterceptor : Interceptor
{
    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        var call = continuation(request, context);

        return new AsyncUnaryCall<TResponse>(
            HandleResponse(call.ResponseAsync),
            call.ResponseHeadersAsync,
            call.GetStatus,
            call.GetTrailers,
            call.Dispose);
    }

    private static async Task<TResponse> HandleResponse<TResponse>(Task<TResponse> inner)
    {
        try
        {
            return await inner;
        }
        catch (RpcException exc) when (exc.StatusCode is StatusCode.Unknown or StatusCode.Unavailable)
        {
            throw CommonExceptions.ServiceUnavailable;
        }
        catch (RpcException exc)
        {
            throw new BusinessException(exc.Status.Detail, (int)exc.Status.StatusCode);
        }
    }
}