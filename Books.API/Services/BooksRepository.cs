using Books.API.DbContexts;
using Books.API.Entities;
using Books.API.Models.External;
using Microsoft.EntityFrameworkCore;

namespace Books.API.Services
{
	public class BooksRepository(BooksContext context, IHttpClientFactory httpClientFactory) : IBooksRepository
	{
		private readonly BooksContext _context = context ?? throw new ArgumentNullException(nameof(context));
		private readonly IHttpClientFactory _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

		public void AddBook(Book bookToAdd)
		{
			ArgumentNullException.ThrowIfNull(bookToAdd, nameof(bookToAdd));
			_context.Add(bookToAdd);
		}

		public async Task<Book?> GetBookAsync(Guid id, CancellationToken cancellationToken)
		{
			return await _context.Books
				.Include(b => b.Author)
				.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
		}

		public async Task<IEnumerable<Book>> GetBooksAsync(CancellationToken cancellationToken)
		{
			return await _context.Books
				.Include(b => b.Author)
				.ToListAsync(cancellationToken);

		}

		public IAsyncEnumerable<Book> GetBooksAsyncEnumerable()
		{
			return _context.Books
				.Include(b => b.Author)
				.AsAsyncEnumerable();
		}

		public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken)
		{
			return (await _context.SaveChangesAsync(cancellationToken)) > 0;
		}

		public async Task<BookCoverDto?> GetBookCoverAsync(string id, CancellationToken cancellationToken)
		{
			var client = _httpClientFactory.CreateClient("BookCoversAPI");
			try
			{
				var response = await client.GetAsync($"api/bookcovers/{id}", cancellationToken);
				if (response.IsSuccessStatusCode)
				{
					var bookCover = await response.Content.ReadFromJsonAsync<BookCoverDto>(cancellationToken: cancellationToken);
					return bookCover;
				}
				else
				{
					return null;
				}
			}
			catch (HttpRequestException)
			{
				return null;
			}
		}
	}
}
