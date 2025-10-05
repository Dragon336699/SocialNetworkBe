using DataAccess.DbContext;
using Domain.AddServicesCollection;
using Microsoft.EntityFrameworkCore;
using SocialNetworkBe.AddServicesCollection;

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
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyHeader()
);

app.UseHttpsRedirection();

app.UseAuthentication(); // Xác minh user hợp lệ không
app.UseAuthorization(); // Phân quyền

app.MapControllers();

app.Run();
