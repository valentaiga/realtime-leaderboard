using Amazon.DynamoDBv2;
using Amazon.Runtime;
using BackOffice.PlayerSearch;
using BackOffice.PlayerSearch.DynamoDb;
using Common.Grpc.Server;
using Common.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpcServices();

builder.Services.AddSingleton<PlayerSearchService>();

var awsOptions = builder.Configuration.GetAWSOptions("AWS");

// Valid format AWS credentials (still fake, but correct format)
awsOptions.Credentials = new BasicAWSCredentials(
    "AKIAXXXXXXXXXXXXXXXX", // 20-character access key (starts with AKIA)
    "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx" // 40-character secret key
);

builder.Services
    .AddDefaultAWSOptions(awsOptions)
    .AddAWSService<IAmazonDynamoDB>()
    .AddHostedService<DynamoDbMigrationService>();

if (builder.Configuration["EnableOpenTelemetry"] == "true")
{
    builder.Logging.AddOpenTelemetryLogger();
    builder.Services.AddOpenTelemetry("BackOffice.PlayerSearch");
}

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<PlayerSearchApiService>();
app.MapGet("/",
    () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();