using JWTAuth;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace FruitKartGateway
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Load ocelot.json
            builder.Configuration
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

            builder.Services.AddJwtAuthentication();

            builder.Services.AddOcelot();

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

            app.UseHttpsRedirection();
            //app.UseRouting();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();

            await app.UseOcelot();

            app.Run();
        }
    }
}