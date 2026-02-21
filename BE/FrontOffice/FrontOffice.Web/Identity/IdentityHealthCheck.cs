using BackOffice.Identity.Grpc;
using BackOffice.Tools.Grpc.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace FrontOffice.Web.Identity;

public class IdentityHealthCheck(IOptions<GrpcClientOptions<IdentityApi.IdentityApiClient>> options)
    : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var data = new Dictionary<string, object>
        {
            { "Endpoint", options.Value.Endpoint }
        };
        var result = new HealthCheckResult(HealthStatus.Healthy, data: data);
        return Task.FromResult(result);
    }
}
