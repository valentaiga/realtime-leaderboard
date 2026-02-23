using System.Collections.Concurrent;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;

namespace Common.Grpc.Client;

public sealed class GrpcChannelFactory(ILogger<GrpcChannelFactory> logger) : IDisposable
{
    private readonly ConcurrentDictionary<string, GrpcChannel> _channels = new(concurrencyLevel: 2, capacity: 4);

    public GrpcChannel Get(string endpoint) =>
        _channels.GetOrAdd(endpoint, s =>
        {
            logger.LogInformation("Grpc channel for {GrpcEndpoint} created", endpoint);
            return GrpcChannel.ForAddress(s, new GrpcChannelOptions
            {
                HttpHandler = new SocketsHttpHandler // for best performance
                {
                    PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                    KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                    KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                    EnableMultipleHttp2Connections = true
                }
            });
        });

    public void Dispose()
    {
        var channels = _channels.Values;
        foreach (var channel in channels)
            channel.Dispose();
    }
}
