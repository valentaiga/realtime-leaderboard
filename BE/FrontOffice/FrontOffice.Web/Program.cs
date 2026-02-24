using BackOffice.Identity.Grpc;
using Common.Grpc.Client;
using FrontOffice.Web;
using FrontOffice.Web.Api.Identity;
using FrontOffice.Web.Authentication;
using FrontOffice.Web.Middleware;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.OpenApi;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

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

builder.Services
    .AddGrpcClient(builder.Configuration, "Grpc:Identity", invoker => new IdentityApi.IdentityApiClient(invoker));

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

var identityGroup = app.MapGroup("/api/identity");
identityGroup.MapPost("login", IdentityController.Login).AllowAnonymous();
identityGroup.MapPost("refresh", IdentityController.RefreshToken).AllowAnonymous();
identityGroup.MapPost("logout", IdentityController.Logout).RequireAuthorization();

app.Run();
