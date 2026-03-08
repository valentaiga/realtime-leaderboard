using BackOffice.Identity.Data;
using BackOffice.Identity.Database;
using Common.Primitives;
using Microsoft.AspNetCore.Identity;

namespace BackOffice.Identity.Identity;

public sealed class UserService(IUserRepository userRepository, IPasswordHasher<UserDto> passwordHasher)
{
    public async Task<LoginUserResult> LoginUserAsync(string username, string password, CancellationToken ct)
    {
        var user = await userRepository.GetByUsernameAsync(username, ct);
        if (user is null || passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) is not PasswordVerificationResult.Success)
            throw new BusinessException("User not found or has incorrect password", BusinessErrorCode.NotFound);

        return new LoginUserResult(user.Id);
    }

    public async Task<UserDto> GetUserByIdAsync(long userId, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(userId, ct)
            ?? throw new BusinessException("User not found or has incorrect password", BusinessErrorCode.NotFound);

        return user;
    }
}