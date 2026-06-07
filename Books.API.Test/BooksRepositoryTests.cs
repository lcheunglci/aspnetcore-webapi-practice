namespace Books.API.Test
{
	public class BooksRepositoryTests
	{
		// sync test: public void TestName() { ... }

		[Fact]
		public async Task GetBooksAsync_ReturnsBooks()
		{
			// Arrange
			var repository = new FakeBooksRepository();
			// Act
			var books = await repository.GetBooksAsync(CancellationToken.None);
			// Assert
			Assert.NotNull(books);
			Assert.NotEmpty(books);
		}

		[Fact]
		public async Task GetBookAsync_WithValidId_ReturnsCorrectBook()
		{
			// Arrange
			var repo = new FakeBooksRepository();
			var bookId = Guid.Parse("2A9E0CD7-8214-4B44-A443-A5CD5E561FA5");
			// Act 
			var book = await repo.GetBookAsync(bookId, CancellationToken.None);
			// Assert
			Assert.NotNull(book);
			Assert.Equal("The Winds of Winter", book.Title);
		}

		// Sync assertion: Assert.Throws<T>(() => Method());
		// Async assertion: await Assert.ThrowsAsync<T>(() => MthodAsync());
		[Fact]
		public async Task GetBooksAsync_WithCancellationToken_ThrowsOperationCanceledException()
		{
			// Arrange
			var repo = new FakeBooksRepository();
			var cancelledToken = new CancellationToken(canceled: true);
			// Act & Assert
			await Assert.ThrowsAnyAsync<OperationCanceledException>(
				() => repo.GetBooksAsync(cancelledToken));
		}
	}
}
