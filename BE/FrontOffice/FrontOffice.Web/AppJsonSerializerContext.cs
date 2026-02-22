using System.Text.Json.Serialization;
using BackOffice.Tools.Grpc.Client;
using FrontOffice.Web.Authentication;
using FrontOffice.Web.Identity;

namespace FrontOffice.Web;

[JsonSerializable(typeof(LoginRequest))]
[JsonSerializable(typeof(LoginResponse))]
[JsonSerializable(typeof(LogoutResponse))]
[JsonSerializable(typeof(RefreshTokenRequest))]
[JsonSerializable(typeof(RefreshTokenResponse))]
[JsonSerializable(typeof(UserShortInfo))]
[JsonSerializable(typeof(GrpcClientOptions))]
[JsonSerializable(typeof(JwtOptions))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}