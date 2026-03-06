namespace BackOffice.Identity.Identity;

public readonly struct LoginUserResult(long userId)
{
    public long UserId { get; } = userId;
}