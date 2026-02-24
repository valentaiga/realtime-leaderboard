using System.Net;
using System.Text.Json.Serialization.Metadata;
using Common.Primitives;
using FrontOffice.Web.Api;
using Grpc.Core;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;

namespace FrontOffice.Web.Middleware;

public class ExceptionHandleMiddleware(IOptions<JsonOptions> jsonOptions, ILogger<ExceptionHandleMiddleware> logger) : IMiddleware
{
    private readonly JsonTypeInfo _apiErrorTypeInfo = jsonOptions.Value.SerializerOptions.GetTypeInfo(typeof(ApiError));
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (BusinessException exc)
        {
            context.Response.StatusCode = GetHttpStatusCode(exc);
            await context.Response.WriteAsJsonAsync(new ApiError(exc.Message), _apiErrorTypeInfo, "application/json", context.RequestAborted);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception exc)
        {
            logger.LogError(exc, "Unhandled exception");
            await context.Response.WriteAsJsonAsync(new ApiError(exc.Message), _apiErrorTypeInfo, "application/json", context.RequestAborted);
        }
    }

    private static int GetHttpStatusCode(BusinessException exc) => (StatusCode)exc.ErrorCode switch
    {
        StatusCode.NotFound or StatusCode.InvalidArgument => (int)HttpStatusCode.BadRequest,
        StatusCode.Unauthenticated => (int)HttpStatusCode.Unauthorized,
        StatusCode.PermissionDenied => (int)HttpStatusCode.Forbidden,
        StatusCode.Unimplemented => (int)HttpStatusCode.NotImplemented,
        _ => (int)HttpStatusCode.ServiceUnavailable
    };
}