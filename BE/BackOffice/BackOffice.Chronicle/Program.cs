using System.Net;
using BackOffice.Chronicle;
using BackOffice.Chronicle.Database;
using BackOffice.Chronicle.Database.Pgsql;
using BackOffice.MQ.Messages;
using BackOffice.MQ.Messages.MatchStatus;
using BackOffice.MQ.Messages.PlayerUpdate;
using Common.Grpc.Server;
using Common.MQ.Kafka;
using Common.MQ.Kafka.Serializer.MessagePack;
using Confluent.Kafka.Extensions.OpenTelemetry;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpcServices();
builder.Services.AddSingleton<MatchService>();
builder.Services
    .AddSingleton<IMatchRepository, PgsqlMatchRepository>()
    .AddSingleton<DbConnectionFactory>();
builder.Services
    .AddKafkaConsumer<string, MatchStatusMessage>(builder.Configuration, "Kafka:Consumer:MatchStatusMessage", config => config.ClientId = Dns.GetHostName())
    .AddHostedService<MatchStatusHandler>()
    .AddKafkaConsumer<long, PlayerUpdateMessage>(builder.Configuration, "Kafka:Consumer:PlayerUpdateMessage", config => config.ClientId = Dns.GetHostName())
    .AddHostedService<PlayerUpdateHandler>()
    .AddMemoryPackKafkaDeserializer(MessagesMessagePackResolver.Instance);

builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeScopes = true;
    logging.IncludeFormattedMessage = true;
});

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("BackOffice.Chronicle"))
    .WithMetrics(metrics =>
        metrics
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddNpgsqlInstrumentation())
    .WithTracing(tracing =>
        tracing
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddConfluentKafkaInstrumentation()
            .AddNpgsql())
    .UseOtlpExporter();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<ChronicleApiService>();
app.MapGet("/",
    () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();