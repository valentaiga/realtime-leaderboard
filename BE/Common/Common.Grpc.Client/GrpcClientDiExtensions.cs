using Common.Grpc.Client.Interceptors;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Common.Grpc.Client;

public static class GrpcClientDiExtensions
{
    public static IServiceCollection AddGrpcClient<TGrpcService>(this IServiceCollection services, IConfiguration configuration, string configSectionPath, Func<CallInvoker, TGrpcService> factory) where TGrpcService : ClientBase
    {
        services.TryAddSingleton<GrpcChannelFactory>();
        services.AddOptions<GrpcClientOptions>(configSectionPath)
            .Configure(options => configuration.GetSection(configSectionPath).Bind(options)).Services
            .AddSingleton(sp =>
            {
                var optionsMonitor = sp.GetRequiredService<IOptionsMonitor<GrpcClientOptions>>();
                var options = optionsMonitor.Get(configSectionPath);
                var channelFactory = sp.GetRequiredService<GrpcChannelFactory>();
                var channel = channelFactory.Get(options.Endpoint);
                var invoker = channel
                    .Intercept(new ClientErrorHandlerInterceptor());
                return factory.Invoke(invoker);
            });

        return services;
    }
}
