using BackOffice.Identity;
using BackOffice.Identity.Data;
using BackOffice.Identity.Database;
using BackOffice.Identity.Database.Pgsql;
using BackOffice.Identity.Identity;
using Common.Grpc.Server;
using Microsoft.AspNetCore.Identity;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

// todo vm: user event on registration
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpcServices();

builder.Services
    .AddSingleton<UserService>()
    .AddSingleton<IUserRepository, PgsqlUserRepository>()
    .AddSingleton<IPasswordHasher<UserDto>, PasswordHasher<UserDto>>()
    .AddSingleton<DbConnectionFactory>();

builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeScopes = true;
    logging.IncludeFormattedMessage = true;
});

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("BackOffice.Identity"))
    .WithMetrics(metrics =>
        metrics
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddNpgsqlInstrumentation())
    .WithTracing(tracing =>
        tracing
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddNpgsql())
    .UseOtlpExporter();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<IdentityApiService>();
app.MapGet("/",
    () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();