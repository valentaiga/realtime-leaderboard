using BackOffice.MQ.Messages.Player;
using Common.MQ.Kafka.Producer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Tests.Common.Kafka;

namespace Tests.Common.BackOffice.Identity;

public class IdentityTestHost : WebApplicationFactory<Program>
{
    public TestKafkaProducer<long, PlayerMessage> PlayerMessageKafkaProducer => (TestKafkaProducer<long, PlayerMessage>)Services.GetRequiredService<IKafkaProducer<long, PlayerMessage>>();

    protected override TestServer CreateServer(IServiceProvider serviceProvider)
    {
        var testServer = base.CreateServer(serviceProvider);
        testServer.BaseAddress = TestConstants.BaseUri.IdentityTestHostUri;
        return testServer;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            services.ReplaceKafkaTopicCreator();
            services.ReplaceKafkaProducerWithInMemoryQueue<long, PlayerMessage>();
        });
        builder.UseSetting("ConnectionStrings:IdentityDb", TestConstants.TestsConnectionString);
        builder.UseSetting("EnableOpenTelemetry", "false");
    }

    protected override void ConfigureClient(HttpClient client)
    {
        base.ConfigureClient(client);
        client.BaseAddress = TestConstants.BaseUri.IdentityTestHostUri;
    }
}