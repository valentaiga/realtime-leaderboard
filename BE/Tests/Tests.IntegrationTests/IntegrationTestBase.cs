using Tests.Common.BackOffice.Chronicle;
using Tests.Common.BackOffice.Identity;
using Tests.Common.BackOffice.Matchmaker;
using Tests.Common.FrontOffice.Web;

namespace Tests.IntegrationTests;

public class IntegrationTestBase(IntegrationTestFixture fixture) : IClassFixture<IntegrationTestFixture>
{
    protected readonly IntegrationTestFixture Fixture = fixture;
}

public class IntegrationTestFixture : IDisposable
{
    public FrontOfficeWebHost Web => _web;
    public MatchmakerTestHost Mm => _mm;
    public IdentityTestHost Identity => _identity;
    public ChronicleTestWebHost Chronicle => _chronicle;

    private readonly FrontOfficeWebHost _web;
    private readonly MatchmakerTestHost _mm;
    private readonly IdentityTestHost _identity;
    private readonly ChronicleTestWebHost _chronicle;

    public IntegrationTestFixture()
    {
        _web = new FrontOfficeWebHost();
        _mm = new MatchmakerTestHost();
        _identity = new IdentityTestHost();
        _chronicle = new ChronicleTestWebHost();

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
    }
}