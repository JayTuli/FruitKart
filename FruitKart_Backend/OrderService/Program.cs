using JWTAuth;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Repository;
using OrderService.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// JSON cycle fix
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseNpgsql(builder.Configuration
        .GetConnectionString("OrderConnection")));

builder.Services.AddHttpClient("MenuService", client =>
{
    var url = builder.Configuration["ServiceUrls:MenuService"]
        ?? "http://localhost:5142";
    client.BaseAddress = new Uri(url);
});

builder.Services.AddJwtAuthentication();
builder.Services.AddOpenApi();

builder.Services.AddCors(opt =>
    opt.AddPolicy("AllowAll", p =>
        p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddLogging();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IMenuServiceClient, MenuServiceClient>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();