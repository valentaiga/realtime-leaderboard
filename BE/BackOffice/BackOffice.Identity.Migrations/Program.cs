using BackOffice.Identity.Migrations;
using Common.Migrations.Postgres;

var builder = WebApplication.CreateBuilder(args);

// builder.
builder.Services.AddMigrationsDbContext<IdentityDbContext>(builder.Configuration, "IdentityDb");
var app = builder.Build();

await app.Services.EnsureMigrationAsync<IdentityDbContext>();
