using Npgsql;

namespace BackOffice.Chronicle.Database.Pgsql;

public class DbConnectionFactory(IConfiguration configuration)
{
    // todo vm: use pgBouncer for db connections (or this might be overhead for such small project)
    public NpgsqlConnection GetConnection(string connectionStringName)
    {
        var connectionString = configuration.GetConnectionString(connectionStringName)
            ?? throw new InvalidOperationException($"Connection string '{connectionStringName}' not configured");
        return new NpgsqlConnection(connectionString);
    }
}
