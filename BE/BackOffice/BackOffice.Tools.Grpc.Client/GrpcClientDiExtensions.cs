using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BackOffice.Tools.Grpc.Client;

public static class GrpcClientDiExtensions
{
    public static IServiceCollection AddClient<TGrpcService>(this IServiceCollection services, string configSectionPath, Func<GrpcChannel, TGrpcService> factory) where TGrpcService : ClientBase
    {
        services.TryAddSingleton<GrpcChannelFactory>();
        services.AddSingleton<TGrpcService>(sp =>
        {
            var options = new GrpcClientOptions();
            var configuration = sp.GetRequiredService<IConfiguration>();
            configuration.GetRequiredSection(configSectionPath).Bind(options);

            var channelFactory = sp.GetRequiredService<GrpcChannelFactory>();
            var channel = channelFactory.Get(options.Endpoint);
            return factory.Invoke(channel);
        });
        return services;
    }
}
