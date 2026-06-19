using AccountService.Data;
using AccountService.Models;
using AccountService.Repository;
using AccountService.MappingProfile;
using JWTAuth;
using Microsoft.AspNetCore.Identity; 
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add database 
builder.Services.AddDbContext<AccountDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AccountConnection")));

//and identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AccountDbContext>()
    .AddDefaultTokenProviders();

// ── JWT Authentication (using your JWTAuth library) ───────────────────────────
builder.Services.AddJwtAuthentication();

//Adding Mapper
builder.Services.AddAutoMapper(typeof(UserProfile));

// OpenAPI                                              
builder.Services.AddOpenApi();

// Add repository
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Adding CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

//
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();                                  
    app.MapScalarApiReference();
}
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();