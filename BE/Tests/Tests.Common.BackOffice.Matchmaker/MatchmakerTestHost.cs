using BackOffice.MQ.Messages.MatchStatus;
using Common.MQ.Kafka.Configurator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tests.Common.Kafka;

namespace Tests.Common.BackOffice.Matchmaker;

public class MatchmakerTestHost : WebApplicationFactory<Program>
{
    protected override TestServer CreateServer(IServiceProvider serviceProvider)
    {
        var testServer = base.CreateServer(serviceProvider);
        testServer.BaseAddress = TestConstants.BaseUri.MatchmakerTestHostUri;
        return testServer;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.UseSetting("FakeActivity:IsEnabled", "false");
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(IKafkaTopicCreator));
            services.AddSingleton<IKafkaTopicCreator, TestTopicCreator>();
            services.ReplaceKafkaProducerWithInMemoryQueue<string, MatchStatusMessage>();
        });
    }
}