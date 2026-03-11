using BackOffice.Matchmaker.Fake;
using BackOffice.Matchmaker.Services;
using BackOffice.MQ.Messages;
using Common.MQ;
using Common.MQ.Kafka;
using Common.MQ.Kafka.Serializer.MessagePack;
using BackOffice.MQ.Messages.MatchStatus;
using Common.MQ.Kafka.Configurator;
using Common.OpenTelemetry;
using Common.Primitives;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOptions<FakePlayerActivityOptions>()
    .Configure(options => builder.Configuration.GetSection("FakeActivity").Bind(options)).Services
    .AddHostedService<FakePlayerActivityService>();

builder.Services
    .AddSingleton<MatchService>()
    .AddSingleton(typeof(ObjectRingBuffer<>))
    .AddUnboundedChannel<MatchStatusMessage>()
    .AddMessageSender<MatchStatusMessageSender, MatchStatusMessage>()
    .AddKafkaProducer<string, MatchStatusMessage>(builder.Configuration, "Kafka:Producer:MatchStatusMessage")
    .AddMemoryPackKafkaSerializer(MessagesMessagePackResolver.Instance)
    .ConfigureKafka(builder.Configuration, configurator =>
    {
        configurator.CreateTopic("Kafka:Configuration:MatchStatus");
    });

if (builder.Configuration["EnableOpenTelemetry"] == "true")
{
    builder.Logging.AddOpenTelemetryLogger();
    builder.Services.AddOpenTelemetry("BackOffice.Matchmaker");
}

var app = builder.Build();

app.Run();