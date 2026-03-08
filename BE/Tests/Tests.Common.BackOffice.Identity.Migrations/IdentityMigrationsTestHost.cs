using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace Tests.Common.BackOffice.Identity.Migrations;

public class IdentityMigrationsTestHost : WebApplicationFactory<Program>
{
    public const string BaseUrl = "http://localhost:2051";

    protected override TestServer CreateServer(IServiceProvider serviceProvider)
    {
        var testServer = base.CreateServer(serviceProvider);
        testServer.BaseAddress = new Uri(BaseUrl);
        return testServer;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.UseSetting("ConnectionStrings:IdentityDb", TestConstants.TestsConnectionString);
    }
}