using DataAccess.Cassandra;
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
builder.Services.AddDbContext<SocialNetworkDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDb"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(options => options
    .WithOrigins("http://fricon.online:3000")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
);

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SocialNetworkDbContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Xác minh user hợp lệ không
app.UseAuthorization(); // Phân quyền

app.UseMiddleware<ValidationErrorMiddleware>();

app.MapControllers();

app.MapHub<ChatHub>("/chathub");

app.Run();
