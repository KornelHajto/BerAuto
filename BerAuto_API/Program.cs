using BerAuto_API.Lib.Migration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Scalar.AspNetCore;
using System.Reflection;
using BerAuto.Lib.ManagerServices;
using BerAuto.Lib.Repositories;
using BerAuto_API.Lib.Repositories;
using BerAuto_API.Lib.Repositories.Interfaces;
using BerAuto_API.Lib.Repositories.Interfaces;


var builder = WebApplication.CreateBuilder(args);

// Add Mapster
builder.Services.AddMapster(); 
TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());


//builder.Services.AddScoped<IAuthManagerService, AuthManagerService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IUnitOfWork, ProductionUnitOfWork>();



// Add services to the container.

builder.Services.AddControllers();


builder.Services.AddDbContext<API_DbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

builder.Services.AddStackExchangeRedisCache(options => 
    options.Configuration = builder.Configuration.GetConnectionString("Cache"));

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
        };
    });

builder.Services.AddAuthorization(options =>
{
    // Basic role policies
    options.AddPolicy("RequireAdministrator", policy => 
        policy.RequireRole(EUserType.Administrator.ToString()));
    
    options.AddPolicy("RequireWorker", policy => 
        policy.RequireRole(EUserType.Administrator.ToString(), EUserType.Worker.ToString()));
    
    options.AddPolicy("RequireUser", policy => 
        policy.RequireRole(EUserType.Administrator.ToString(), EUserType.Worker.ToString(), EUserType.User.ToString()));
    
    // All authenticated users
    options.AddPolicy("RequireAuthenticated", policy => policy.RequireAuthenticatedUser());
});

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();