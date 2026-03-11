using BackOffice.Identity;
using BackOffice.Identity.Data;
using BackOffice.Identity.Database;
using BackOffice.Identity.Database.Pgsql;
using BackOffice.Identity.Identity;
using Common.Grpc.Server;
using Common.OpenTelemetry;
using Microsoft.AspNetCore.Identity;

// todo vm: user event on registration
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpcServices();

builder.Services
    .AddSingleton<UserService>()
    .AddSingleton<IUserRepository, PgsqlUserRepository>()
    .AddSingleton<IPasswordHasher<UserDto>, PasswordHasher<UserDto>>()
    .AddSingleton<DbConnectionFactory>();

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