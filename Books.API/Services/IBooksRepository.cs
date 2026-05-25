namespace Books.API.Services
{
	public interface IBooksRepository
	{
		Task<IEnumerable<Entities.Book>> GetBooksAsync();
		Task<Entities.Book?> GetBookAsync(Guid id);
		void AddBook(Entities.Book bookToAdd);
		Task<bool> SaveChangesAsync();

	}
}
