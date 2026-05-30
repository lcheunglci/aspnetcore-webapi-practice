using Books.API.Models;

namespace Books.API.ExportWriters
{
	public class BookExportWriter : IAsyncDisposable
	{
		private readonly StreamWriter _writer;
		private bool _disposed;

		public BookExportWriter(string filePath)
		{
			_writer = new StreamWriter(filePath, append: false);
		}

		public async Task WriteBookAsync(BookDto book, CancellationToken cancellationToken)
		{
			ObjectDisposedException.ThrowIf(_disposed, this);
			await _writer.WriteLineAsync(
				$"{book.Id},{book.AuthorName},{book.Title}".AsMemory(), cancellationToken);
		}

		public async ValueTask DisposeAsync()
		{
			if (_disposed) return;
			_disposed = true;
			await _writer.DisposeAsync();
			GC.SuppressFinalize(this);
		}
	}
}
