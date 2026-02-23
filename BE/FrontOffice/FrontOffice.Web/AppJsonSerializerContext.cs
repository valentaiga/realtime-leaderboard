using System.Text.Json.Serialization;
using Common.Grpc.Client;
using FrontOffice.Web.Authentication;
using FrontOffice.Web.Identity;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace FrontOffice.Web;

[JsonSerializable(typeof(LoginRequest))]
[JsonSerializable(typeof(LoginResponse))]
[JsonSerializable(typeof(LogoutResponse))]
[JsonSerializable(typeof(RefreshTokenRequest))]
[JsonSerializable(typeof(RefreshTokenResponse))]
[JsonSerializable(typeof(UserShortInfo))]
[JsonSerializable(typeof(GrpcClientOptions))]
[JsonSerializable(typeof(JwtOptions))]
[JsonSerializable(typeof(CorsPolicy))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}