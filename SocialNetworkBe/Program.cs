using DataAccess.DbContext;
using Domain.AddServicesCollection;
using Microsoft.EntityFrameworkCore;
using SocialNetworkBe.AddServicesCollection;
using SocialNetworkBe.ChatServer;
using SocialNetworkBe.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();
builder.Services.ConfigureServices(builder.Configuration);
builder.Services.ConfigureLifeCycle();

// Support for environment variables (for Docker/Cloud deployment)
var connectionString = builder.Configuration.GetConnectionString("MyDb") 
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__MyDb");

builder.Services.AddDbContext<SocialNetworkDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Dynamic CORS for production
var allowedOrigins = builder.Configuration["AllowedOrigins"]
    ?? Environment.GetEnvironmentVariable("AllowedOrigins")
    ?? "http://localhost:3000";

var originsArray = allowedOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries);

app.UseCors(options => options
    .WithOrigins(originsArray)
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
);

// Only use HTTPS redirection in production with proper SSL
if (!app.Environment.IsDevelopment())
{
    // Comment out if your cloud provider handles SSL termination
    // app.UseHttpsRedirection();
}
else
{
    app.UseHttpsRedirection();
}

app.UseAuthentication(); // Xác minh user hợp lệ không
app.UseAuthorization(); // Phân quyền

app.UseMiddleware<ValidationErrorMiddleware>();

// Health check endpoint for container orchestration
app.MapHealthChecks("/health");

app.MapControllers();

app.MapHub<ChatHub>("/chathub");

app.Run();
