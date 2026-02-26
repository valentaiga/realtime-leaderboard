using System.Text.Json.Serialization;
using Common.Filtering;
using Common.Grpc.Client;
using FrontOffice.Web.Api;
using FrontOffice.Web.Api.Identity;
using FrontOffice.Web.Api.Player;
using FrontOffice.Web.Authentication;
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
[JsonSerializable(typeof(ApiError))]
[JsonSerializable(typeof(FilterRequest))]
[JsonSerializable(typeof(MatchFilterRequest))]
[JsonSerializable(typeof(FilterResult<Match>))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}