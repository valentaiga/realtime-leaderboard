namespace FrontOffice.Web.Api.Identity;

public record LoginResponse(string Token, UserShortInfo User);