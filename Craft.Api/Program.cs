using Craft.Application;
using Craft.Application.Common.Interface;
using Craft.Application.Common.Interfaces;
using Craft.Infrastucture.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddSerilog(dispose: true);
});

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("ApplicationName", "Craft")
    .Enrich.WithProperty("ApplicationVersion", "0.1")
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Hour)
    .CreateLogger();
        
builder.Host.UseSerilog();

builder.Services.AddApplicationLayer();

builder.Services.AddHttpContextAccessor();


try
{
    builder.Services.AddDbContext<ApplicationContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationContext"), opt => opt.EnableRetryOnFailure()));
}
catch (SqlException)
{
    Log.Warning("Failed to connect to SQL Server. Falling back to in-memory database.");
    builder.Services.AddDbContext<ApplicationContext>(options =>
        options.UseInMemoryDatabase("CraftDb"));
}

builder.Services.AddDbContext<ApplicationContext>(options =>
        options.UseInMemoryDatabase("CraftDb"));

builder.Services.AddScoped<IApplicationContext, ApplicationContext>();

builder.Services.AddScoped<IEmailService, EmailService>(); // Register EmailService

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuration to use HTTPS in production
if (!builder.Environment.IsDevelopment())
{
    builder.WebHost.UseKestrel(options =>
    {
        options.ListenAnyIP(5001, listenOptions =>
        {
            listenOptions.UseHttps("certificate.pfx", "password");
        });
    });
}

// Configure CORS.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder => builder
            .WithOrigins("https://example.com", "https://www.example.com")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS middleware
app.UseCors("AllowSpecificOrigins");

app.UseAuthorization();

app.MapControllers();

app.Run();
