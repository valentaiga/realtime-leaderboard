using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Common.Primitives;

public static class DiExtensions
{
    public static IServiceCollection AddUnboundedChannel<TItem>(this IServiceCollection services, UnboundedChannelOptions? unboundedChannelOptions = null)
    {
        var matchFinishedChannel = unboundedChannelOptions is null
            ? Channel.CreateUnbounded<TItem>()
            : Channel.CreateUnbounded<TItem>(unboundedChannelOptions);
        services.TryAddSingleton(matchFinishedChannel);
        services.TryAddSingleton(matchFinishedChannel.Writer);
        services.TryAddSingleton(matchFinishedChannel.Reader);
        return services;
    }
}