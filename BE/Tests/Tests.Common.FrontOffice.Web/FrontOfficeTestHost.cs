using Common.Grpc.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tests.Common.Grpc;

namespace Tests.Common.FrontOffice.Web;

public class FrontOfficeTestHost : WebApplicationFactory<Program>
{
    public TService GetRequiredService<TService>() where TService : notnull => Services.GetRequiredService<TService>();
    
    public void ConnectGrpc(string endpoint, HttpClient client) => ((TestGrpcChannelFactory)Services.GetRequiredService<IGrpcChannelFactory>())
        .AddConnection(endpoint, client);
    
    protected override TestServer CreateServer(IServiceProvider serviceProvider)
    {
        var testServer = base.CreateServer(serviceProvider);
        testServer.BaseAddress = TestConstants.BaseUri.FrontOfficeTestHostUri;
        return testServer;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(IGrpcChannelFactory));
            services.TryAddSingleton<IGrpcChannelFactory, TestGrpcChannelFactory>();
        });
        
        builder.UseSetting("Grpc:Chronicle:Endpoint", TestConstants.BaseUri.ChronicleTestHostUri.ToString());
        builder.UseSetting("Grpc:Identity:Endpoint", TestConstants.BaseUri.IdentityTestHostUri.ToString());
    }
}