namespace FrontOffice.Web.Identity;

public record LoginResponse(string Token, UserShortInfo User);