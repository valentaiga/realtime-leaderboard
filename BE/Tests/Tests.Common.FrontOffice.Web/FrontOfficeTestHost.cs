using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace Tests.Common.FrontOffice.Web;

public class FrontOfficeTestHost : WebApplicationFactory<Program>
{
    protected override TestServer CreateServer(IServiceProvider serviceProvider)
    {
        var testServer = base.CreateServer(serviceProvider);
        testServer.BaseAddress = TestConstants.BaseUri.FrontOfficeTestHostUri;
        return testServer;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.UseSetting("Grpc:Chronicle:Endpoint", TestConstants.BaseUri.ChronicleTestHostUri.ToString());
        builder.UseSetting("Grpc:Identity:Endpoint", TestConstants.BaseUri.IdentityTestHostUri.ToString());
    }
}