using BackOffice.Matchmaker.Fake;
using BackOffice.Matchmaker.Services;
using BackOffice.MQ.Messages;
using Common.MQ;
using Common.MQ.Kafka;
using Common.MQ.Kafka.Serializer.MessagePack;
using BackOffice.MQ.Messages.MatchStatus;
using Common.MQ.Kafka.Configurator;
using Common.Primitives;
using Confluent.Kafka.Extensions.OpenTelemetry;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

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

builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeScopes = true;
    logging.IncludeFormattedMessage = true;
});

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("BackOffice.Matchmaker"))
    .WithMetrics(metrics =>
        metrics
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation())
    .WithTracing(tracing =>
        tracing
            .AddConfluentKafkaInstrumentation()
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation())
    .UseOtlpExporter();

var app = builder.Build();

app.Run();