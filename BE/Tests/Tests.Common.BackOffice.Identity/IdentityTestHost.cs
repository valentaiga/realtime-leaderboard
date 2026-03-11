using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace Tests.Common.BackOffice.Identity;

public class IdentityTestHost : WebApplicationFactory<Program>
{
    protected override TestServer CreateServer(IServiceProvider serviceProvider)
    {
        var testServer = base.CreateServer(serviceProvider);
        testServer.BaseAddress = TestConstants.BaseUri.IdentityTestHostUri;
        return testServer;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.UseSetting("ConnectionStrings:IdentityDb", TestConstants.TestsConnectionString);
        builder.UseSetting("EnableOpenTelemetry", "false");
    }

    protected override void ConfigureClient(HttpClient client)
    {
        base.ConfigureClient(client);
        client.BaseAddress = TestConstants.BaseUri.IdentityTestHostUri;
    }
}