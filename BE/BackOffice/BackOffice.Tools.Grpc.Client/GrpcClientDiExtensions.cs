using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace BackOffice.Tools.Grpc.Client;

public static class GrpcClientDiExtensions
{
    public static IServiceCollection AddClient<TGrpcService>(this IServiceCollection services, string configSectionPath, Func<GrpcChannel, TGrpcService> factory) where TGrpcService : ClientBase
    {
        services.TryAddSingleton<GrpcChannelFactory>();
        services.AddOptions<GrpcClientOptions>()
            .Configure<IConfiguration>((opts, config) =>
            {
                var section = string.Equals("", configSectionPath, StringComparison.OrdinalIgnoreCase)
                    ? config
                    : config.GetSection(configSectionPath);
                section.Bind(opts);
            })
            .ValidateOnStart();

        services.AddSingleton<TGrpcService>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<GrpcClientOptions>>();
            var channelFactory = sp.GetRequiredService<GrpcChannelFactory>();
            var channel = channelFactory.Get(options.Value.Endpoint);
            return factory.Invoke(channel);
        });
        return services;
    }
}
