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
        private readonly ILogger<BooksRepository> _logger;
        private CancellationTokenSource _cancellationTokenSource;

        public BooksRepository(BookContext context, IHttpClientFactory httpClientFactory, ILogger<BooksRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Pitfall: Using Task.Run() on the server
        // ASP.NET Core is not optimized for Task.Run() and it created extra overhead
        // also decreases scalability, and is mainly intended for client side apps for responsiveness
        private Task<int> GetBookPages()
        {
            return Task.Run(() =>
            {
                var pageCalculator = new Books.Legacy.ComplicatedPageCalculator();
                _logger.LogInformation($"ThreadId when calculating the amount of pages: "
                    + $"{System.Threading.Thread.CurrentThread.ManagedThreadId}");
                return pageCalculator.CalculateBookPage();
            });
        }

        public async Task<Book> GetBookAsync(Guid id)
        {
            _context.Database.ExecuteSqlRaw("WAITFOR DELAY '00:00:02';");

            //var pageCalculator = new Books.Legacy.ComplicatedPageCalculator();
            //var amountOfPages = pageCalculator.CalculateBookPage();

            //_logger.LogInformation($"ThreadId when entering GetBookAsync: " +
            //    $"{System.Threading.Thread.CurrentThread.ManagedThreadId}");

            //var bookPages = await GetBookPages();

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
            _cancellationTokenSource = new CancellationTokenSource();


            // create a list of fake bookcovers
            var bookCoverUrls = new[]
            {
                $"https://localhost:52644/api/bookcovers/{bookId}-dummerycover1",
                //$"https://localhost:52644/api/bookcovers/{bookId}-dummerycover2?returnFault=true",
                $"https://localhost:52644/api/bookcovers/{bookId}-dummerycover2",
                $"https://localhost:52644/api/bookcovers/{bookId}-dummerycover3",
                $"https://localhost:52644/api/bookcovers/{bookId}-dummerycover4",
                $"https://localhost:52644/api/bookcovers/{bookId}-dummerycover5",
            };

            // create the tasks
            var downloadBookCoverTasksQuery =
                from bookCoverUrl
                in bookCoverUrls
                select DownloadBookCoverAsync(httpClient, bookCoverUrl, _cancellationTokenSource.Token);


            // start the tasks
            var downloadBookCoverTasks = downloadBookCoverTasksQuery.ToList();

            try
            {
                return await Task.WhenAll(downloadBookCoverTasks);
            }
            catch (OperationCanceledException operationCanceledException)
            {
                _logger.LogInformation($"{operationCanceledException.Message}");
                foreach (var task in downloadBookCoverTasks)
                {
                    _logger.LogInformation($"Task {task.Id} has status {task.Status}");
                }

                return new List<BookCover>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                throw;
            }
            //foreach (var bookCoverUrl in bookCoverUrls)
            //{
            //    var response = await httpClient.GetAsync(bookCoverUrl);

            //    if (response.IsSuccessStatusCode)
            //    {
            //        bookCovers.Add(JsonSerializer.Deserialize<BookCover>(
            //            await response.Content.ReadAsStringAsync(),
            //            new JsonSerializerOptions()
            //            {
            //                PropertyNameCaseInsensitive = true,
            //            }));
            //    }
            //}

            //return bookCovers;
        }

        private async Task<BookCover> DownloadBookCoverAsync(HttpClient httpClient, string bookCoverUrl,
            CancellationToken cancellationToken)
        {
            // throw new Exception("Cannot download book cover. " +
            //    "write isn't finishing book fast enough");

            var response = await httpClient
                .GetAsync(bookCoverUrl, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var bookCover = JsonSerializer.Deserialize<BookCover>(
                    await response.Content.ReadAsStringAsync(),
                    new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true,
                    });
                return bookCover;
            }

            _cancellationTokenSource.Cancel();
            return null;
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

                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Dispose();
                    _cancellationTokenSource = null;
                }
            }
        }
    }
}
