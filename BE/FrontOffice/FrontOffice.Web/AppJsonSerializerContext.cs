using System.Text.Json.Serialization;
using Common.Filtering;
using Common.Grpc.Client;
using FrontOffice.Web.Api;
using FrontOffice.Web.Api.Identity;
using FrontOffice.Web.Api.Matches;
using FrontOffice.Web.Authentication;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace FrontOffice.Web;

[JsonSerializable(typeof(LoginRequest))]
[JsonSerializable(typeof(RegisterRequest))]
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
[JsonSerializable(typeof(GetMatchesRequest))]
[JsonSerializable(typeof(FilterDescriptor<Guid>))]
[JsonSerializable(typeof(FilterDescriptor<bool>))]
[JsonSerializable(typeof(FilterDescriptor<long>))]
[JsonSerializable(typeof(FilterDescriptor<DateTime>))]
[JsonSerializable(typeof(FilterResult<Match>))]
public partial class AppJsonSerializerContext : JsonSerializerContext
{
}