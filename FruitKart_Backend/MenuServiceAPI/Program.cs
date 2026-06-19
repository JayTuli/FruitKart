using JWTAuth;
using MenuServiceAPI.Data;
using MenuServiceAPI.MappingProfile;
using MenuServiceAPI.Repository;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<MenuDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("MenuServiceDb")));

// JWT
builder.Services.AddJwtAuthentication();                       

// AutoMapper
builder.Services.AddAutoMapper(typeof(MenuItemProfile));       

// Repository
builder.Services.AddScoped<IMenuRepository, MenuRepository>();

// HttpClient for ImageService
builder.Services.AddHttpClient("ImageService", client =>
{
    var url = builder.Configuration["ServiceUrls:ImageService"]
        ?? throw new InvalidOperationException("ServiceUrls:ImageService not configured.");
    client.BaseAddress = new Uri(url);
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Controllers + Swagger
builder.Services.AddControllers();
// OpenAPI                                                  // ✅ replace Swagger
builder.Services.AddOpenApi();

// CORS
builder.Services.AddCors(opt =>
    opt.AddPolicy("AllowAll", p =>
        p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// Auto migrate on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MenuDbContext>();
    await db.Database.MigrateAsync();                           // async
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();                                       // ✅ replace UseSwagger
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();                                  
app.UseCors("AllowAll");
app.UseAuthentication();                                        
app.UseAuthorization();                                         
app.MapControllers();
app.Run();