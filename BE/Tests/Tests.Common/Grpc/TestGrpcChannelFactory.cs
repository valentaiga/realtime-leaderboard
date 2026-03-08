using Common.Grpc.Client;
using Grpc.Net.Client;

namespace Tests.Common.Grpc;

public class TestGrpcChannelFactory : IGrpcChannelFactory, IDisposable
{
    private readonly Dictionary<string, GrpcChannel> _channels = new();

    public void AddConnection(string endpoint, HttpClient httpClient)
    {
        _channels[endpoint] = GrpcChannel.ForAddress(endpoint, new GrpcChannelOptions
        {
            HttpClient = httpClient,
        });
    }
    
    public GrpcChannel Get(string endpoint) => _channels.GetValueOrDefault(endpoint)
                                               ?? throw new InvalidOperationException($"Endpoint '{endpoint}' not configured for tests");

    public void Dispose()
    {
        foreach (var channel in _channels)
            channel.Value.Dispose();
    }
}