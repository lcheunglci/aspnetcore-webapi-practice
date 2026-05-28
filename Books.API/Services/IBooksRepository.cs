namespace Books.API.Services
{
	public interface IBooksRepository
	{
		Task<IEnumerable<Entities.Book>> GetBooksAsync(CancellationToken cancellationToken);
		Task<Entities.Book?> GetBookAsync(Guid id, CancellationToken cancellationToken);
		void AddBook(Entities.Book bookToAdd);
		Task<bool> SaveChangesAsync(CancellationToken cancellationToken);

	}
}
