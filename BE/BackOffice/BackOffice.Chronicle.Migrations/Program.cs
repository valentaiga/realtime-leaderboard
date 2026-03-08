using BackOffice.Chronicle.Migrations;
using Common.Migrations.Postgres;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// builder.
builder.Services.AddMigrationsDbContext<ChronicleDbContext>(builder.Configuration, "ChronicleDb");
var app = builder.Build();

await app.Services.EnsureMigrationAsync<ChronicleDbContext>();
