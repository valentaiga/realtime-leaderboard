using System.Net;
using BackOffice.Chronicle;
using BackOffice.Chronicle.Database;
using BackOffice.Chronicle.Database.Pgsql;
using BackOffice.MQ.Messages;
using BackOffice.MQ.Messages.MatchStatus;
using Common.Grpc.Server;
using Common.MQ.Kafka;
using Common.MQ.Kafka.Serializer.MessagePack;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpcServices();
builder.Services.AddSingleton<MatchService>();
builder.Services
    .AddSingleton<IMatchRepository, PgsqlMatchRepository>()
    .AddHostedService<MatchStatusHandler>()
    .AddSingleton<DbConnectionFactory>();
builder.Services
    .AddKafkaConsumer<string, MatchStatusMessage>(builder.Configuration, "Kafka:Consumer:MatchStatusMessage", config => config.ClientId = Dns.GetHostName())
    .AddMemoryPackKafkaDeserializer(MessagesMessagePackResolver.Instance);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<ChronicleApiService>();
app.MapGet("/",
    () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();