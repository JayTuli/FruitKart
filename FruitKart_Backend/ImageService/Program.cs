using CloudinaryDotNet;
using ImageService.Data;
using ImageService.MappingProfile;
using ImageService.Repository;
using JWTAuth;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace ImageService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            // ── Database ──────────────────────────────────────────────────────
            builder.Services.AddDbContext<FruitImageDbContext>(options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("ImageServiceDb")));

            // ── Cloudinary (singleton — one instance is enough) ───────────────
            builder.Services.AddSingleton<ICloudinaryService, CloudinaryService>();

            // ── Repository ────────────────────────────────────────────────────
            builder.Services.AddScoped<IImageRepository, ImageRepository>();

            // ── AutoMapper ────────────────────────────────────────────────────
            builder.Services.AddAutoMapper(typeof(FruitImageMap));

            // ── OpenAPI ───────────────────────────────────────────────────────
            builder.Services.AddOpenApi();

            // ── JWT ───────────────────────────────────────────────────────────
            builder.Services.AddJwtAuthentication();

            // ── CORS ──────────────────────────────────────────────────────────
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
                scope.ServiceProvider.GetRequiredService<FruitImageDbContext>().Database.Migrate();

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
        }
    }
}