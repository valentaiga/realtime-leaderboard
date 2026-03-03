using System.Net;
using BackOffice.Chronicle;
using BackOffice.MQ.Messages;
using BackOffice.MQ.Messages.MatchStatus;
using Common.Grpc.Server;
using Common.MQ.Kafka;
using Common.MQ.Kafka.Serializer.MessagePack;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpcServices();
builder.Services.AddSingleton<MatchService>();
builder.Services
    .AddKafkaConsumer<MatchStatusConsumer, Guid, MatchStatusMessage>(builder.Configuration, "Kafka:Consumer:MatchStatusMessage", config => config.ClientId = Dns.GetHostName())
    .AddMemoryPackKafkaDeserializer(MessagesMessagePackResolver.Instance)
    .OverrideKafkaDeserializer<KafkaMemoryPackDeserializer<Guid>, Guid>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<ChronicleApiService>();
app.MapGet("/",
    () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();