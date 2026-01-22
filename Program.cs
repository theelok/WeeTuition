using Microsoft.EntityFrameworkCore;
using System;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using TimetableSystem.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Handle circular references in JSON serialization
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = false;
        // Set max depth to prevent issues
        options.JsonSerializerOptions.MaxDepth = 64;
        // Ignore null values for navigation properties
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".TimetableSystem.Session";
    options.Cookie.Path = "/";
    
    // Production: Use Secure cookies with SameSite=None for cross-origin
    // Development: Use Lax for same-site
    if (builder.Environment.IsDevelopment())
    {
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.None;
    }
    else
    {
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // HTTPS required
    }
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // Development origins
            policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
        else
        {
            // Production: Get frontend URL from configuration
            var frontendUrl = builder.Configuration["FrontendUrl"] 
                ?? throw new InvalidOperationException("FrontendUrl not configured");
            
            policy.WithOrigins(frontendUrl)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseRouting();

// CORS must be after UseRouting and before UseSession
app.UseCors("AllowReactApp");

app.UseSession();
app.UseAuthorization();

// Serve static files from wwwroot (contains frontend/dist after build)
app.UseStaticFiles();

// API routes
app.MapControllers();

// Fallback to index.html for SPA routing (must be last, after all other routes)
if (!app.Environment.IsDevelopment())
{
    app.MapFallbackToFile("index.html");
}

app.Run();