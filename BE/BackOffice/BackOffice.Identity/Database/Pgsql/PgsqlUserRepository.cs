using BackOffice.Identity.Data;
using Dapper;

namespace BackOffice.Identity.Database.Pgsql;

[DapperAot]
public class PgsqlUserRepository(DbConnectionFactory dbConnectionFactory, ILogger<PgsqlUserRepository> logger) : IUserRepository
{
    public async Task<UserDto?> GetByUsernameAsync(string username, CancellationToken ct)
    {
        try
        {
            await using var conn = dbConnectionFactory.GetConnection("IdentityDb");
            await conn.OpenAsync(ct);

            return await conn.QueryFirstOrDefaultAsync<UserDto>(
                """
                SELECT id, username, password_hash
                FROM users
                WHERE username = @username
                """,
                new
                {
                    username
                }).WaitAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting user");
            throw;
        }
    }

    public async Task<UserDto?> GetByIdAsync(long id, CancellationToken ct)
    {
        try
        {
            await using var conn = dbConnectionFactory.GetConnection("IdentityDb");
            await conn.OpenAsync(ct);

            return await conn.QueryFirstOrDefaultAsync<UserDto>(
                """
                SELECT id, username, password_hash
                FROM users
                WHERE id = @id
                """,
                new
                {
                    id
                }).WaitAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting user");
            throw;
        }
    }

    public async Task Add(UserDto user, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(user);

        try
        {
            await using var conn = dbConnectionFactory.GetConnection("IdentityDb");
            await conn.OpenAsync(ct);

            await conn.ExecuteScalarAsync(
                """
                INSERT INTO users (id, username, password_hash)
                VALUES (@id, @username, @passwordHash)
                """,
                new
                {
                    user.Id,
                    user.Username,
                    user.PasswordHash
                }).WaitAsync(ct);
        }
        catch (Exception ex) // todo vm: handle 'id duplicate' as business exception
        {
            logger.LogError(ex, "Error adding user");
            throw;
        }
    }
}