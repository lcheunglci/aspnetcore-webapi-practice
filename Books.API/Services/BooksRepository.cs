using Books.API.Context;
using Books.API.Entities;
using Books.API.ExternalModels;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Books.API.Services
{
    public class BooksRepository : IBooksRepository, IDisposable
    {
        private BookContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public BooksRepository(BookContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }


        public async Task<Book> GetBookAsync(Guid id)
        {
            _context.Database.ExecuteSqlRaw("WAITFOR DELAY '00:00:02';");
            return await _context.Books
                .Include(b => b.Author).FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Book>> GetBooksAsync()
        {
            await _context.Database.ExecuteSqlRawAsync("WAITFOR DELAY '00:00:02';");
            return await _context.Books.Include(b => b.Author).ToListAsync();
        }

        public IEnumerable<Book> GetBooks()
        {
            return _context.Books.Include(b => b.Author).ToList();
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        public void AddBook(Book bookToAdd)
        {
            if (bookToAdd == null)
            {
                throw new ArgumentNullException(nameof(bookToAdd));
            }

            _context.Add(bookToAdd);
        }

        public async Task<bool> SaveChangesAsync()
        {
            // return if 1 or more entities were changed
            return (await _context.SaveChangesAsync() > 0);
        }

        public async Task<IEnumerable<Book>> GetBooksAsync(IEnumerable<Guid> bookIds)
        {
            return await _context.Books.Where(b => bookIds.Contains(b.Id))
                .Include(b => b.Author).ToListAsync();
        }

        public async Task<BookCover> GetBookCoverAsync(string coverId)
        {
            var httpClient = _httpClientFactory.CreateClient();
            // pass through a dummy name
            var response = await httpClient.GetAsync($"http://localhost:52644/api/bookcovers/{coverId}");
            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<BookCover>(await response.Content.ReadAsStringAsync(),
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }

            return null;
        }

        public async Task<IEnumerable<BookCover>> GetBookCoversAsync(Guid bookId)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var bookCovers = new List<BookCover>();

            // create a list of fake bookcovers
            var bookCoverUrls = new[]
            {
                $"https://localhost:52644/api/bookcovers/{bookId}-dummerycover1",
                $"https://localhost:52644/api/bookcovers/{bookId}-dummerycover2",
                $"https://localhost:52644/api/bookcovers/{bookId}-dummerycover3",
                $"https://localhost:52644/api/bookcovers/{bookId}-dummerycover4",
                $"https://localhost:52644/api/bookcovers/{bookId}-dummerycover5",
            };


            foreach (var bookCoverUrl in bookCoverUrls)
            {
                var response = await httpClient.GetAsync(bookCoverUrl);

                if (response.IsSuccessStatusCode)
                {
                    bookCovers.Add(JsonSerializer.Deserialize<BookCover>(
                        await response.Content.ReadAsStringAsync(),
                        new JsonSerializerOptions()
                        {
                            PropertyNameCaseInsensitive = true,
                        }));
                }
            }

            return bookCovers;
        }

        protected virtual void Dispose(bool dispositing)
        {
            if (dispositing)
            {
                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }
            }
        }
    }
}
