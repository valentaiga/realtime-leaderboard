using System.Text.Json.Serialization;
using FrontOffice.Web.Identity;

namespace FrontOffice.Web;

[JsonSerializable(typeof(LoginRequest))]
[JsonSerializable(typeof(LoginResponse))]
[JsonSerializable(typeof(LogoutResponse))]
[JsonSerializable(typeof(RefreshTokenRequest))]
[JsonSerializable(typeof(RefreshTokenResponse))]
[JsonSerializable(typeof(UserShortInfo))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}