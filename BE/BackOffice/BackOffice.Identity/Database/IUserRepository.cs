using BackOffice.Identity.Data;

namespace BackOffice.Identity.Database;

public interface IUserRepository
{
    Task<UserDto?> GetByUsernameAsync(string username, CancellationToken ct);
    Task<UserDto?> GetByIdAsync(long id, CancellationToken ct);

    Task Add(UserDto user, CancellationToken ct);
}