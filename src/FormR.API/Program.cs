using System.Text;
using AspNetCoreRateLimit;
using FluentValidation;
using FormR.API.Middleware;
using FormR.API.Services;
using FormR.Core.Interfaces;
using FormR.Core.Models;
using FormR.Core.Validation;
using FormR.Data;
using FormR.Data.Providers;
using FormR.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Register validators
builder.Services.AddScoped<IValidator<FormTemplate>, TemplateValidator>();
builder.Services.AddScoped<IValidator<FormControl>, ControlValidator>();

// Register repositories and services
builder.Services.AddScoped<IFormRepository, FormRepository>();
builder.Services.AddScoped<ITemplateService, TemplateService>();

// Configure database provider pattern
builder.Services.AddSingleton<IDataProvider, PostgreSqlProvider>();
builder.Services.AddDbContext<FormBuilderContext>((sp, options) =>
{
    var provider = sp.GetRequiredService<IDataProvider>();
    provider.ConfigureMigrations(options);
});

// Configure JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Configure rate limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Ensure database is created and apply migrations
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FormBuilderContext>();
    context.Database.Migrate();
}

// Configure the HTTP request pipeline
app.UseErrorHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FormR API v1");
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseIpRateLimiting();

app.UseAuthentication();
app.UseAuthorization();

app.UseTenantResolution();

app.MapControllers();

app.Run();
