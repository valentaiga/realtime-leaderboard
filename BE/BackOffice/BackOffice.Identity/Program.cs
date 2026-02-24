using BackOffice.Identity;
using BackOffice.Identity.Identity;
using Common.Grpc.Server.Interceptors;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc(options => 
    options.Interceptors.Add<ServerErrorHandlerInterceptor>());

builder.Services
    .AddSingleton<UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<IdentityApiService>();
app.MapGet("/",
    () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();