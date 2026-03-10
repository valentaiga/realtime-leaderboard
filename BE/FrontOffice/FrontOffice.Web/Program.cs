using BackOffice.Chronicle.Grpc;
using BackOffice.Identity.Grpc;
using Common.Grpc.Client;
using FrontOffice.Web;
using FrontOffice.Web.Api.Identity;
using FrontOffice.Web.Api.Matches;
using FrontOffice.Web.Authentication;
using FrontOffice.Web.Middleware;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.OpenApi;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

// todo vm: nginx should handle user requests, not frontoffice directly
// todo vm: add service with user search via nickname for FE 'search input'
// todo vm: add remote cache (redis) for idempotent endpoints

builder.Services
    .AddSingleton<ExceptionHandleMiddleware>()
    .AddJwtAuthentication(builder.Configuration)
    .AddCors(options =>
    {
        var policy = new CorsPolicy();
        builder.Configuration.GetSection("Cors").Bind(policy);
        options.DefaultPolicyName = "Frontend";
        options.AddPolicy("Frontend", policy);
    });

builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeScopes = true;
    logging.IncludeFormattedMessage = true;
});

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("FrontOffice.Web"))
    .WithMetrics(metrics =>
        metrics
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation())
    .WithTracing(tracing =>
        tracing
            .AddHttpClientInstrumentation()
            .AddGrpcClientInstrumentation()
            .AddAspNetCoreInstrumentation())
    .UseOtlpExporter();

builder.Services
    .AddGrpcClient(builder.Configuration, "Grpc:Identity", invoker => new IdentityApi.IdentityApiClient(invoker))
    .AddGrpcClient(builder.Configuration, "Grpc:Chronicle", invoker => new ChronicleApi.ChronicleApiClient(invoker));

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options => options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0);

var app = builder.Build();

app.UseAuthorization();
app.UseMiddleware<ExceptionHandleMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseCors(corsBuilder => corsBuilder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
    
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
}
else
{
    app.UseCors();
}

var identityGroup = app.MapGroup("api/identity");
identityGroup.MapPost("login", IdentityController.Login).AllowAnonymous();
identityGroup.MapPost("register", IdentityController.Register).AllowAnonymous();
identityGroup.MapPost("refresh", IdentityController.RefreshToken).AllowAnonymous();
identityGroup.MapPost("logout", IdentityController.Logout).RequireAuthorization();

var matchesGroup = app.MapGroup("api/matches");
matchesGroup.MapPost("/", PlayerController.GetMatches).AllowAnonymous();


app.Run();
