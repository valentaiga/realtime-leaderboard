using BackOffice.MQ.Messages.MatchStatus;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Tests.Common.Kafka;

namespace Tests.Common.BackOffice.Matchmaker;

public class MatchmakerTestHost : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureServices(services =>
            services.ReplaceKafkaProducerWithInMemoryQueue<Guid, MatchStatusMessage>());
    }
}