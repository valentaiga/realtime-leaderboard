using BackOffice.Identity.Data;
using Dapper;

namespace BackOffice.Identity.Database.Pgsql;

[DapperAot]
public class PgsqlUserRepository(DbConnectionFactory dbConnectionFactory, ILogger<PgsqlUserRepository> logger) : IUserRepository
{
    public Task<UserDto?> GetByUsernameAsync(string username, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<UserDto?> GetByIdAsync(long id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task Add(UserDto user, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}