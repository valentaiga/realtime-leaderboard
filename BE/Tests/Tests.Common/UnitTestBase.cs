using Common.Grpc.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tests.Common.Grpc;
using Xunit;

namespace Tests.Common;

public abstract class UnitTestBase<TEntryPoint, THost> : IAsyncLifetime
    where THost: WebApplicationFactory<TEntryPoint>, new()
    where TEntryPoint : class
{
    public THost Host { get; private set; } = null!;

    public Task InitializeAsync()
    {
        Host = new THost();
        Host.WithWebHostBuilder(ConfigureTestHost);
        Host.StartServer();
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        Host.Dispose();
        return Task.CompletedTask;
    }

    protected virtual void ConfigureTestHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(IGrpcChannelFactory));
            services.TryAddSingleton<IGrpcChannelFactory, TestGrpcChannelFactory>();
        });
    }

    protected HttpClient CreateClient(WebApplicationFactoryClientOptions? options = null) =>
        options is null ? Host.CreateClient() : Host.CreateClient(options);

    protected TService GetRequiredService<TService>() where TService : notnull =>
        Host.Services.GetRequiredService<TService>();
}