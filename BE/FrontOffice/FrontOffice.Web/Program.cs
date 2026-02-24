using BackOffice.Identity.Grpc;
using Common.Grpc.Client;
using Common.Grpc.Client.Interceptors;
using FrontOffice.Web;
using FrontOffice.Web.Api.Identity;
using FrontOffice.Web.Authentication;
using FrontOffice.Web.Middleware;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;
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
    .AddSingleton<GrpcChannelFactory>() // todo vm: move this dirty code to other place
    .AddOptions<GrpcClientOptions>("Grpc:Identity")
        .Configure(options => builder.Configuration.GetSection("Grpc:Identity").Bind(options)).Services
    .AddSingleton(sp =>
    {
        var optionsMonitor = sp.GetRequiredService<IOptionsMonitor<GrpcClientOptions>>();
        var options = optionsMonitor.Get("Grpc:Identity");
        var channelFactory = sp.GetRequiredService<GrpcChannelFactory>();
        var channel = channelFactory.Get(options.Endpoint);
        var invoker = channel
            .Intercept(new ClientErrorHandlerInterceptor());
        return new IdentityApi.IdentityApiClient(invoker);
    });

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
