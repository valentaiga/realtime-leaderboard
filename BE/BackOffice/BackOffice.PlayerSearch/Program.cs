using BackOffice.PlayerSearch;
using Common.Grpc.Server;
using Common.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpcServices();



if (builder.Configuration["EnableOpenTelemetry"] == "true")
{
    builder.Logging.AddOpenTelemetryLogger();
    builder.Services.AddOpenTelemetry("BackOffice.PlayerSearch");
}

var app = builder.Build();
app.MapGrpcService<AutoCompleteApiService>();

app.Run();