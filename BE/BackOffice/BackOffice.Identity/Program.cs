using BackOffice.Identity;
using BackOffice.Identity.Data;
using BackOffice.Identity.Database;
using BackOffice.Identity.Database.Pgsql;
using BackOffice.Identity.Identity;
using BackOffice.MQ.Messages;
using BackOffice.MQ.Messages.MatchStatus;
using BackOffice.MQ.Messages.Player;
using Common.Grpc.Server;
using Common.MQ;
using Common.MQ.Kafka;
using Common.MQ.Kafka.Configurator;
using Common.MQ.Kafka.Serializer.MessagePack;
using Common.OpenTelemetry;
using Common.Primitives;
using Microsoft.AspNetCore.Identity;

// todo vm: move migrations apply from service docker to different docker service
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpcServices();

builder.Services
    .AddSingleton<UserService>()
    .AddSingleton<IUserRepository, PgsqlUserRepository>()
    .AddSingleton<IPasswordHasher<UserDto>, PasswordHasher<UserDto>>()
    .AddSingleton<DbConnectionFactory>();

builder.Services
    .AddUnboundedChannel<PlayerMessage>()
    .AddMessageSender<PlayerRegisteredEventSender, PlayerMessage>()
    .AddKafkaProducer<long, PlayerMessage>(builder.Configuration, "Kafka:Producer:PlayerMessage")
    .AddMemoryPackKafkaSerializer(MessagesMessagePackResolver.Instance)
    .ConfigureKafka(builder.Configuration, configurator =>
    {
        configurator.CreateTopic("Kafka:Configuration:Player");
    });

if (builder.Configuration["EnableOpenTelemetry"] == "true")
{
    builder.Logging.AddOpenTelemetryLogger();
    builder.Services.AddOpenTelemetry("BackOffice.Identity");
}

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<IdentityApiService>();
app.MapGet("/",
    () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();