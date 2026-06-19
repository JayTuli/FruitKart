using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MenuServiceAPI.Data
{
    public class MenuDbContextFactory : IDesignTimeDbContextFactory<MenuDbContext>
    {
        public MenuDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

            var optionsBuilder = new DbContextOptionsBuilder<MenuDbContext>();
            optionsBuilder.UseNpgsql(config.GetConnectionString("MenuServiceDb"));

            return new MenuDbContext(optionsBuilder.Options);
        }
    }
}