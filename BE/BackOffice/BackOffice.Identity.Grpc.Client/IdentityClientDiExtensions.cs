using BackOffice.Tools.Grpc.Client;
using Microsoft.Extensions.DependencyInjection;

namespace BackOffice.Identity.Grpc.Client;

public static class IdentityClientDiExtensions
{
    public static IServiceCollection AddIdentityGrpcClient(this IServiceCollection services, string configSectionPath) =>
        services.AddClient<IdentityApi.IdentityApiClient>(configSectionPath, channel => new IdentityApi.IdentityApiClient(channel));
}
