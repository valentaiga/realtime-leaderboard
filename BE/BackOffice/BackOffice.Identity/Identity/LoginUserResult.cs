namespace BackOffice.Identity.Identity;

public readonly struct LoginUserResult(ulong userId)
{
    public ulong UserId { get; } = userId;
}