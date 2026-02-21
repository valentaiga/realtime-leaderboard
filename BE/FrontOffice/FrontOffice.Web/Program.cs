using FrontOffice.Web.Identity;
using BackOffice.Identity.Authentication;
using BackOffice.Identity.Grpc.Client;
using FrontOffice.Web;
using Microsoft.OpenApi;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services
    .AddAuthorization()
    .AddIdentityAuthentication(builder.Configuration);
builder.Services.AddIdentityGrpcClient("Grpc:Identity");
builder.Services.AddHealthChecks()
    .AddCheck<IdentityHealthCheck>("identity");

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options => options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0);

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
}

app.MapHealthChecks("/hc");

var identityGroup = app.MapGroup("/api/identity");
identityGroup.MapPost("login", IdentityController.Login).AllowAnonymous();
identityGroup.MapPost("refresh", IdentityController.RefreshToken).AllowAnonymous();
identityGroup.MapPost("logout", IdentityController.Logout).RequireAuthorization();

app.Run();

