using Books.API.DbContexts;
using Books.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Books.API.Services
{
	public class BooksRepository(BooksContext context) : IBooksRepository
	{
		private readonly BooksContext _context = context ?? throw new ArgumentNullException(nameof(context));

		public void AddBook(Book bookToAdd)
		{
			ArgumentNullException.ThrowIfNull(bookToAdd, nameof(bookToAdd));
			_context.Add(bookToAdd);
		}

		public async Task<Book?> GetBookAsync(Guid id)
		{
			return await _context.Books
				.Include(b => b.Author)
				.FirstOrDefaultAsync(b => b.Id == id);
		}

		public async Task<IEnumerable<Book>> GetBooksAsync()
		{
			return await _context.Books
				.Include(b => b.Author)
				.ToListAsync();

		}

		public async Task<bool> SaveChangesAsync()
		{
			return (await _context.SaveChangesAsync()) > 0;
		}
	}
}
