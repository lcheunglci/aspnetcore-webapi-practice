using System.Text.Json;
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

		public async Task<IEnumerable<BookCoverDto>> GetBookCoverOneByOneAsync(Guid bookId, CancellationToken cancellationToken)
		{
			var httpClient = _httpClientFactory.CreateClient("BookCoversAPI");
			var bookCovers = new List<BookCoverDto>();
			var bookCoverUrls = new[]
			{
				$"api/bookcovers/{bookId}-dummycover1",
				$"api/bookcovers/{bookId}-dummycover2",
				$"api/bookcovers/{bookId}-dummycover3",
				$"api/bookcovers/{bookId}-dummycover4",
				$"api/bookcovers/{bookId}-dummycover5"
			};

			foreach (var bookCoverUrl in bookCoverUrls)
			{
				var response = httpClient.GetAsync(bookCoverUrl, cancellationToken).GetAwaiter().GetResult();
				if (response.IsSuccessStatusCode)
				{
					var bookCover = JsonSerializer.Deserialize<BookCoverDto>(
						await response.Content.ReadAsStringAsync(cancellationToken),
						new JsonSerializerOptions
						{
							PropertyNameCaseInsensitive = true
						});

					// var bookCover = response.Content.ReadFromJsonAsync<BookCoverDto>(cancellationToken: cancellationToken).GetAwaiter().GetResult();
					if (bookCover != null)
					{
						bookCovers.Add(bookCover);
					}
				}
			}
			return bookCovers;
		}

		public async Task<IEnumerable<BookCoverDto>> GetBookCoversAfterWaitForAllAsync(Guid bookId, CancellationToken cancellationToken)
		{
			var httpClient = _httpClientFactory.CreateClient("BookCoversAPI");
			var bookCovers = new List<BookCoverDto>();
			var bookCoverUrls = new[]
			{
				$"api/bookcovers/{bookId}-dummycover1",
				$"api/bookcovers/{bookId}-dummycover2",
				$"api/bookcovers/{bookId}-dummycover3",
				$"api/bookcovers/{bookId}-dummycover4",
				$"api/bookcovers/{bookId}-dummycover5"
			};
			var bookCoverTasks = new List<Task<HttpResponseMessage>>();
			foreach (var bookCoverUrl in bookCoverUrls)
			{
				bookCoverTasks.Add(httpClient.GetAsync(bookCoverUrl, cancellationToken));
			}
			var bookCoverTaskResults = await Task.WhenAll(bookCoverTasks);
			foreach (var bookCoverTaskResult in bookCoverTaskResults)
			{
				if (bookCoverTaskResult.IsSuccessStatusCode)
				{
					var bookCover = JsonSerializer.Deserialize<BookCoverDto>(
						await bookCoverTaskResult.Content.ReadAsStringAsync(cancellationToken),
						new JsonSerializerOptions
						{
							PropertyNameCaseInsensitive = true
						});
					// var bookCover = await bookCoverTaskResult.Content.ReadFromJsonAsync<BookCoverDto>(cancellationToken: cancellationToken);
					if (bookCover != null)
					{
						bookCovers.Add(bookCover);
					}
				}
			}
			return bookCovers;
		}
	}
}
