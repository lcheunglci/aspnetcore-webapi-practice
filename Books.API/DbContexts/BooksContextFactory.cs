using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Books.API.DbContexts;

public class BooksContextFactory : IDesignTimeDbContextFactory<BooksContext>
{
    public BooksContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BooksContext>();
        optionsBuilder.UseSqlite("Data Source=books.db"); // Update with your connection string if needed
        return new BooksContext(optionsBuilder.Options);
    }
}
