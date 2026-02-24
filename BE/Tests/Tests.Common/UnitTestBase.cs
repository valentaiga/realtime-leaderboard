using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Tests.Common;

public abstract class UnitTestBase<TEntryPoint, THost> : IAsyncLifetime
    where THost: WebApplicationFactory<TEntryPoint>, new()
    where TEntryPoint : class
{
    private THost _host = null!;

    public Task InitializeAsync()
    {
        _host = new THost();
        _host.WithWebHostBuilder(ConfigureTestHost);
        _host.StartServer();
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _host.Dispose();
        return Task.CompletedTask;
    }

    protected virtual void ConfigureTestHost(IWebHostBuilder builder)
    {
    }

    protected HttpClient CreateClient(WebApplicationFactoryClientOptions? options = null) =>
        options is null ? _host.CreateClient() : _host.CreateClient(options);

    protected TService GetRequiredService<TService>() where TService : notnull =>
        _host.Services.GetRequiredService<TService>();

    protected void AssertErrorResponse(HttpResponseMessage response, HttpStatusCode statusCode)
    {
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(statusCode);
    }

    protected async Task<TErrorResponse> AssertErrorResponseAsync<TErrorResponse>(HttpResponseMessage response, HttpStatusCode statusCode)
    {
        AssertErrorResponse(response, statusCode);
        var error = await response.Content.ReadFromJsonAsync<TErrorResponse>();
        error.Should().NotBeNull();
        return error;
    }

    protected void AssertSuccessResponse(HttpResponseMessage response)
    {
        response.ReasonPhrase.Should().Be("OK");
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    protected async Task<TResponse> AssertSuccessResponseAsync<TResponse>(HttpResponseMessage response)
    {
        response.ReasonPhrase.Should().Be("OK");
        response.IsSuccessStatusCode.Should().BeTrue();
        var typedResponse = await response.Content.ReadFromJsonAsync<TResponse>();
        typedResponse.Should().NotBeNull();
        return typedResponse;
    }
}