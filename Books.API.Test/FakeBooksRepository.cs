using Books.API.Entities;
using Books.API.Models.External;
using Books.API.Services;

namespace Books.API.Test
{
	public class FakeBooksRepository : IBooksRepository
	{
		private readonly List<Book> _books = [
			new(
				// Guid.Parse("5b1c2b4d"), 
				Guid.Parse("2A9E0CD7-8214-4B44-A443-A5CD5E561FA5"),
				Guid.Parse("45F128D0-A460-49D8-AC83-3D009EF51C22"),
				"The Winds of Winter",
				"The book that seems impossible to write."
				),
			new (
				// Guid.Parse("d8663e5e"),
				Guid.Parse("859ACB2E-F79F-4B62-9F54-1D14B111ABC7"),
				Guid.Parse("45F128D0-A460-49D8-AC83-3D009EF51C22"),
				"A Game of Thrones",
				"A Game of Thrones is the first novel of A Song of Ice and Fire."
				)
			];

		public void AddBook(Book bookToAdd)
		{
			_books.Add(bookToAdd);
		}

		public Task<Book?> GetBookAsync(Guid id, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return Task.FromResult(_books.FirstOrDefault(b => b.Id == id));
		}

		public Task<BookCoverDto?> GetBookCoverAsync(string id, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<BookCoverDto>> GetBookCoverOneByOneAsync(Guid bookId, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<BookCoverDto>> GetBookCoversAfterWaitForAllAsync(Guid id, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<Book>> GetBooksAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return Task.FromResult<IEnumerable<Book>>(_books);
		}

		public IAsyncEnumerable<Book> GetBooksAsyncEnumerable()
		{
			throw new NotImplementedException();
		}

		public Task<bool> SaveChangesAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return Task.FromResult(true);
		}
	}
}
