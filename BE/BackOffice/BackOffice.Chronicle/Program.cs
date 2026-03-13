using System.Net;
using BackOffice.Chronicle;
using BackOffice.Chronicle.Database;
using BackOffice.Chronicle.Database.Pgsql;
using BackOffice.MQ.Messages;
using BackOffice.MQ.Messages.MatchStatus;
using BackOffice.MQ.Messages.Player;
using Common.Grpc.Server;
using Common.MQ.Kafka;
using Common.MQ.Kafka.Serializer.MessagePack;
using Common.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpcServices();
builder.Services.AddSingleton<MatchService>();
builder.Services
    .AddSingleton<IMatchRepository, PgsqlMatchRepository>()
    .AddSingleton<DbConnectionFactory>();
builder.Services
    .AddKafkaConsumer<string, MatchStatusMessage>(builder.Configuration, "Kafka:Consumer:MatchStatusMessage", config => config.ClientId = Dns.GetHostName())
    .AddHostedService<MatchStatusHandler>()
    .AddKafkaConsumer<long, PlayerMessage>(builder.Configuration, "Kafka:Consumer:PlayerMessage", config => config.ClientId = Dns.GetHostName())
    .AddHostedService<PlayerUpdateHandler>()
    .AddMemoryPackKafkaDeserializer(MessagesMessagePackResolver.Instance)
    .OverrideKafkaDeserializer<KafkaMemoryPackDeserializer<long>, long>();

if (builder.Configuration["EnableOpenTelemetry"] == "true")
{
    builder.Logging.AddOpenTelemetryLogger();
    builder.Services.AddOpenTelemetry("BackOffice.Chronicle");
}

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<ChronicleApiService>();
app.MapGet("/",
    () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();