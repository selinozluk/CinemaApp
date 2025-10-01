using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using CinemaApp.Data.Entities;
using CinemaApp.Data.Context;
using CinemaApp.Data.Repositories;
using CinemaApp.Data.UnitOfWork;
using CinemaApp.Business.Auth;
using CinemaApp.WebApi.Filters;
using CinemaApp.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

/* Controllers + NewtonsoftJson (PATCH desteði) */
builder.Services
    .AddControllers(opts =>
    {
        // Global model validation
        opts.Filters.Add<ValidationFilterAttribute>();
    })
    .AddNewtonsoftJson();

// [ServiceFilter] ile kullanýlma durumuna karþý DI
builder.Services.AddScoped<ValidationFilterAttribute>();

/* Swagger (+ JWT) */
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CinemaApp API", Version = "v1" });

    var bearer = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Bearer {token}",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };

    c.AddSecurityDefinition("Bearer", bearer);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { { bearer, Array.Empty<string>() } });
});

/* EF Core – SQL Server */
var cs = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<CinemaDbContext>(opt =>
    opt.UseSqlServer(cs, b => b.MigrationsAssembly("CinemaApp.Data")));

/* DI – Repository & UnitOfWork */
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

/* Data Protection */
builder.Services.AddDataProtection().SetApplicationName("CinemaApp");

/* Authentication – JWT */
var jwt = builder.Configuration.GetSection("Jwt");
var secret = jwt["SecretKey"] ?? jwt["Key"]
             ?? throw new InvalidOperationException("Jwt:SecretKey/Key bulunamadý.");

byte[] keyBytes;
try { keyBytes = Convert.FromBase64String(secret); }    // Base64 ise
catch { keyBytes = Encoding.UTF8.GetBytes(secret); }    // düz string ise

if (keyBytes.Length < 32)
    throw new InvalidOperationException("JWT secret minimum 32 byte (256 bit) olmalý.");

var signingKey = new SymmetricSecurityKey(keyBytes);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.IncludeErrorDetails = true; // 401 sebebini logla
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = signingKey,
            ClockSkew = TimeSpan.Zero
        };
    });

/* Authorization */
builder.Services.AddAuthorization();

/* Business servisleri */
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IPasswordHasher<UserEntity>, PasswordHasher<UserEntity>>(); // <<< eklendi

var app = builder.Build();

/* Global exception handling */
app.UseMiddleware<GlobalExceptionMiddleware>();

/* Swagger UI */
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

/* Pipeline */
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
