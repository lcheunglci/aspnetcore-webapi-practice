using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DishesAPI.DbContexts;

public class DishesDbContextFactory : IDesignTimeDbContextFactory<DishesDbContext>
{
    public DishesDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DishesDbContext>();
        optionsBuilder.UseSqlite("Data Source=dishes.db"); // Update with your connection string if needed
        return new DishesDbContext(optionsBuilder.Options);
    }
}
