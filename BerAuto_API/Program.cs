using BerAuto_API.Lib.Migration;
using CryptoSim_API.Lib.UnitOfWork;
using Scalar.AspNetCore;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);

// Add Mapster
builder.Services.AddMapster(); 
TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

// Add services to the container.

builder.Services.AddControllers();


builder.Services.AddDbContext<API_DbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

builder.Services.AddStackExchangeRedisCache(options => 
    options.Configuration = builder.Configuration.GetConnectionString("Cache"));

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddLocalServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Servers = Array.Empty<ScalarServer>();
    });
    app.ApplyMigrations();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();