using Microsoft.AspNetCore.Mvc.Testing;

namespace Books.API.Test
{
	internal class BooksEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
	{
		private readonly WebApplicationFactory<Program> _factory;
		public BooksEndpointsTests(
			WebApplicationFactory<Program> factory)
		{
			_factory = factory;
		}

		[Fact]
		public async Task GetBooks_ReturnsSuccessStatusCode()
		{
			// Arrange
			var client = _factory.CreateClient();
			// Act
			var response = await client.GetAsync("/api/books");
			// Assert
			response.EnsureSuccessStatusCode();
		}

		[Fact]
		public async Task StreamBooks_ReturnsSuccessAndStreamedContent()
		{
			// Arrange
			var client = _factory.CreateClient();
			// Act - ResponseHeadersRead means "don't wait for the full body"
			var response = await client.GetAsync(
				"/api/books/booksstream",
				HttpCompletionOption.ResponseHeadersRead);

			// Assert
			response.EnsureSuccessStatusCode();
			await using var stream =
				await response.Content.ReadAsStreamAsync();
			using var reader = new StreamReader(stream);
			var content = await reader.ReadToEndAsync();
			Assert.NotEmpty(content);

		}
	}
}
