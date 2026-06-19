using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OrderService.Data
{
    public class OrderDbContextFactory : IDesignTimeDbContextFactory<OrderDbContext>
    {
        public OrderDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<OrderDbContext>();
            optionsBuilder.UseNpgsql(
                "Host=aws-1-ap-northeast-1.pooler.supabase.com;Database=postgres;Username=postgres.msaljdctgddskvihrkdx;Password=DB#Tuli69aarav;SSL Mode=Require;Trust Server Certificate=true");
            return new OrderDbContext(optionsBuilder.Options);
        }
    }
}