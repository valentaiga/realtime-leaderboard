using Dapper;
using Npgsql;
using Tests.Common;
using Tests.Common.BackOffice.Chronicle;
using Tests.Common.BackOffice.Chronicle.Migrations;
using Tests.Common.BackOffice.Identity;
using Tests.Common.BackOffice.Identity.Migrations;
using Tests.Common.BackOffice.Matchmaker;
using Tests.Common.FrontOffice.Web;

namespace Tests.IntegrationTests;

public class IntegrationTestBase(IntegrationTestFixture fixture) : IClassFixture<IntegrationTestFixture>
{
    protected readonly IntegrationTestFixture Fixture = fixture;
}

public class IntegrationTestFixture : IDisposable
{
    public FrontOfficeTestHost Web => _web;
    public MatchmakerTestHost Mm => _mm;
    public IdentityTestHost Identity => _identity;
    public ChronicleTestHost Chronicle => _chronicle;
    public ChronicleMigrationsTestHost ChronicleMigrations => _chronicleMigrations;

    private readonly FrontOfficeTestHost _web;
    private readonly MatchmakerTestHost _mm;
    private readonly IdentityTestHost _identity;
    private readonly IdentityMigrationsTestHost _identityMigrations;
    private readonly ChronicleTestHost _chronicle;
    private readonly ChronicleMigrationsTestHost _chronicleMigrations;

    public IntegrationTestFixture()
    {
        DropDb();
        _chronicleMigrations = new ChronicleMigrationsTestHost();
        _chronicleMigrations.StartServer();
        _identityMigrations = new IdentityMigrationsTestHost();
        _identityMigrations.StartServer();

        _web = new FrontOfficeTestHost();
        _mm = new MatchmakerTestHost();
        _identity = new IdentityTestHost();
        _chronicle = new ChronicleTestHost();
        
        _web.UseKestrel(TestConstants.BaseUri.FrontOfficeTestHostUri.Port);
        _mm.UseKestrel(TestConstants.BaseUri.MatchmakerTestHostUri.Port);
        _identity.UseKestrel(TestConstants.BaseUri.IdentityTestHostUri.Port);
        _chronicle.UseKestrel(TestConstants.BaseUri.ChronicleTestHostUri.Port);

        _web.StartServer();
        _mm.StartServer();
        _identity.StartServer();
        _chronicle.StartServer();
    }

    public void Dispose()
    {
        _web.Dispose();
        _mm.Dispose();
        _identity.Dispose();
        _chronicle.Dispose();
        _chronicleMigrations.Dispose();
        _identityMigrations.Dispose();
    }

    public static void CleanDb()
    {
        using var conn = new NpgsqlConnection(TestConstants.TestsConnectionString);
        conn.Open();
        
        // chronicle db
        conn.Execute("delete from migrations_history");
        conn.Execute("delete from match_players");
        conn.Execute("delete from matches");
        conn.Execute("delete from users");
    }

    private static void DropDb()
    {
        using var conn = new NpgsqlConnection(TestConstants.TestsConnectionString);
        conn.Open();
        
        // chronicle db
        conn.Execute("DROP SEQUENCE IF EXISTS MatchSeq");
        conn.Execute("DROP TABLE IF EXISTS migrations_history");
        conn.Execute("DROP TABLE IF EXISTS match_players");
        conn.Execute("DROP TABLE IF EXISTS matches");
        conn.Execute("DROP TABLE IF EXISTS users");
    }
}