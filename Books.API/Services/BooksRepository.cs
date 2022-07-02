using Books.API.Context;
using Books.API.Entities;
using Microsoft.EntityFrameworkCore;



namespace Books.API.Services
{
    public class BooksRepository : IBookRepository, IDisposable
    {
        private BookContext _context;

        public BooksRepository(BookContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
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

        public async Task<IEnumerable<Book>> GetBooksAsync(IEnumerable<Guid> bookIds)
        {
            return await _context.Books.Where(b => bookIds.Contains(b.Id))
                .Include(b => b.Author).ToListAsync();
        }
    }
}
