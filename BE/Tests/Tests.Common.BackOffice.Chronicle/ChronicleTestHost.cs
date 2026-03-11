using BackOffice.MQ.Messages.MatchStatus;
using BackOffice.MQ.Messages.PlayerUpdate;
using Common.MQ.Kafka.Consumer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Tests.Common.Kafka;

namespace Tests.Common.BackOffice.Chronicle;

public class ChronicleTestHost : WebApplicationFactory<Program>
{
    protected override TestServer CreateServer(IServiceProvider serviceProvider)
    {
        var testServer = base.CreateServer(serviceProvider);
        testServer.BaseAddress = TestConstants.BaseUri.ChronicleTestHostUri;
        return testServer;
    }

    protected override void ConfigureClient(HttpClient client)
    {
        base.ConfigureClient(client);
        client.BaseAddress = TestConstants.BaseUri.IdentityTestHostUri;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services => services
            .ReplaceKafkaConsumerWithInMemoryQueue<string, MatchStatusMessage>()
            .ReplaceKafkaConsumerWithInMemoryQueue<long, PlayerUpdateMessage>());

        builder.UseSetting("ConnectionStrings:ChronicleDb", TestConstants.TestsConnectionString);
    }

    public TestKafkaConsumer<string, MatchStatusMessage> MatchStatusConsumer =>
        (TestKafkaConsumer<string, MatchStatusMessage>)Services.GetRequiredService<IKafkaConsumer<string, MatchStatusMessage>>();

    public TestKafkaConsumer<long, PlayerUpdateMessage> PlayerUpdateConsumer =>
        (TestKafkaConsumer<long, PlayerUpdateMessage>)Services.GetRequiredService<IKafkaConsumer<long, PlayerUpdateMessage>>();
}