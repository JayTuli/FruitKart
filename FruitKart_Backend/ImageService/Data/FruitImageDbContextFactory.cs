using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ImageService.Data
{
    public class FruitImageDbContextFactory : IDesignTimeDbContextFactory<FruitImageDbContext>
    {
        public FruitImageDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FruitImageDbContext>();
            optionsBuilder.UseNpgsql(
                "Host=aws-1-ap-northeast-1.pooler.supabase.com;Database=postgres;Username=postgres.msaljdctgddskvihrkdx;Password=DB#Tuli69aarav;SSL Mode=Require;Trust Server Certificate=true");
            return new FruitImageDbContext(optionsBuilder.Options);
        }
    }
}