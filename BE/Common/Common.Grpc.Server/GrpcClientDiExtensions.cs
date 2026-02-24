using Common.Grpc.Server.Interceptors;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Grpc.Server;

public static class GrpcClientDiExtensions
{
    public static IServiceCollection AddGrpcServices(this IServiceCollection services)
    {
        services.AddGrpc(options => 
            options.Interceptors.Add<ServerErrorHandlerInterceptor>());
        
        return services;
    }
}
