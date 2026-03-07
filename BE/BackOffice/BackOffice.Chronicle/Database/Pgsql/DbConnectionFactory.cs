using Npgsql;

namespace BackOffice.Chronicle.Database.Pgsql;

public class DbConnectionFactory(IConfiguration configuration)
{
    public NpgsqlConnection GetConnection(string connectionStringName)
    {
        var connectionString = configuration.GetConnectionString(connectionStringName)
            ?? throw new InvalidOperationException($"Connection string '{connectionStringName}' not configured");
        return new NpgsqlConnection(connectionString);
    }
}
