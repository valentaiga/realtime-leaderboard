using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;

namespace Tests.Common.Extensions;

public static class HttpResponseExtensions
{
    public static void AssertErrorResponse(this HttpResponseMessage response, HttpStatusCode statusCode)
    {
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(statusCode);
    }

    public static async Task<TErrorResponse> AssertErrorResponseAsync<TErrorResponse>(this HttpResponseMessage response, HttpStatusCode statusCode)
    {
        AssertErrorResponse(response, statusCode);
        var error = await response.Content.ReadFromJsonAsync<TErrorResponse>();
        error.Should().NotBeNull();
        return error;
    }

    public static void AssertSuccessResponse(this HttpResponseMessage response)
    {
        response.ReasonPhrase.Should().Be("OK");
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    public static async Task<TResponse> AssertSuccessResponseAsync<TResponse>(this HttpResponseMessage response)
    {
        response.ReasonPhrase.Should().Be("OK");
        response.IsSuccessStatusCode.Should().BeTrue();
        var typedResponse = await response.Content.ReadFromJsonAsync<TResponse>();
        typedResponse.Should().NotBeNull();
        return typedResponse;
    }
}