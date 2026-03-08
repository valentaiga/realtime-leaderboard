using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace Tests.Common.BackOffice.Chronicle.Migrations;

public class ChronicleMigrationsTestHost : WebApplicationFactory<Program>
{
    public const string BaseUrl = "http://localhost:2050";

    protected override TestServer CreateServer(IServiceProvider serviceProvider)
    {
        var testServer = base.CreateServer(serviceProvider);
        testServer.BaseAddress = new Uri(BaseUrl);
        return testServer;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.UseSetting("ConnectionStrings:ChronicleDb", TestConstants.TestsConnectionString);
    }
}