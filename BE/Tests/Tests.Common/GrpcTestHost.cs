using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Common;

public class GrpcTestHost<TGrpcService, TServiceBehaviour> : IDisposable, IAsyncDisposable
    where TGrpcService : class
    where TServiceBehaviour : class, new()
{
    public TServiceBehaviour ServiceBehaviour => _app.Services.GetRequiredService<TServiceBehaviour>();

    private readonly WebApplication _app;
    private Task? _runningTask;

    protected GrpcTestHost(ushort listeningPort, Action<TServiceBehaviour>? configureBehaviour = null)
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.ConfigureKestrel(kestrelServerOptions =>
            kestrelServerOptions.ListenAnyIP(listeningPort, listenOptions =>
                listenOptions.Protocols = HttpProtocols.Http2));

        var behaviour = new TServiceBehaviour();
        configureBehaviour?.Invoke(behaviour);
        builder.Services.AddSingleton(behaviour);

        builder.Services.AddGrpc();

        _app = builder.Build();
        _app.MapGrpcService<TGrpcService>();
        _runningTask = _app.RunAsync();
    }

    public Task StartAsync()
    {
        if (_runningTask is not null)
            return Task.CompletedTask;

        _runningTask = _app.RunAsync();
        return Task.CompletedTask;
    }

    public Task StopAsync() => _runningTask is null ? Task.CompletedTask : _app.StopAsync();

    public async ValueTask DisposeAsync()
    {
        await StopAsync();
        await _app.DisposeAsync();
    }

    public void Dispose() => DisposeAsync().GetAwaiter().GetResult();
}