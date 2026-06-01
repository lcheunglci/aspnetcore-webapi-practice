using System.Runtime.CompilerServices;
using AutoMapper;
using Books.API.ExportWriters;
using Books.API.Models;
using Books.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Books.API.Controllers
{
	[Route("api/books")]
	[ApiController]
	public class BooksController(IBooksRepository booksRepository,
		IMapper mapper,
		ILogger<BooksController> logger) : ControllerBase
	{
		private readonly IBooksRepository _booksRepository = booksRepository ?? throw new ArgumentNullException(nameof(booksRepository));
		private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		private readonly ILogger<BooksController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

		[HttpGet()]
		public async Task<IActionResult> GetBooks(CancellationToken cancellationToken)
		{
			using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
			using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token, cancellationToken);

			try
			{
				var bookEntity = await _booksRepository.GetBooksAsync(linkedTokenSource.Token);
				return Ok(_mapper.Map<IEnumerable<BookDto>>(bookEntity));
			}
			catch (OperationCanceledException)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					// client disconnect
					throw;
				}
				else if (timeoutCts.IsCancellationRequested)
				{
					// timeout elapsed
					_logger.LogWarning(
						"[{Timestamp}] GetBooks operation timed out - ThreadId {ThreadId}", DateTime.UtcNow.ToString("HH:mm:ss.fff"), Environment.CurrentManagedThreadId);
					// return StatusCode(StatusCodes.Status504GatewayTimeout, "The request timed out.");
					throw;
				}

			}
			return StatusCode(StatusCodes.Status500InternalServerError);
		}


		[HttpGet("{id:guid}", Name = "GetBook")]
		public async Task<IActionResult> GetBook(Guid id, CancellationToken cancellationToken)
		{

			Thread.Sleep(5000); // simulate a long-running operation
			try
			{
				_logger.LogInformation(
					"[{Timestamp}] GetBook entered - ThreadId {ThreadId}",
					DateTime.UtcNow.ToString("HH:mm:ss.fff"),
					Environment.CurrentManagedThreadId);

				var bookEntity = await _booksRepository.GetBookAsync(id, cancellationToken);
				if (bookEntity == null)
				{
					return NotFound();
				}
				return Ok(_mapper.Map<BookDto>(bookEntity));
			}
			catch (OperationCanceledException)
			{
				_logger.LogInformation(
					"GetBook request was cancelled.");
				return StatusCode(499); // ensure the client receives a response indicating the request was cancelled
			}
		}

		[HttpPost]
		public async Task<IActionResult> CreateBook([FromBody] BookForCreationDto book, CancellationToken cancellationToken)
		{
			var bookEntity = _mapper.Map<Entities.Book>(book);
			_booksRepository.AddBook(bookEntity);
			await _booksRepository.SaveChangesAsync(cancellationToken);

			// fetch the book from the data store (including the author)
			var bookFromRepo = _mapper.Map<Models.BookDto>(bookEntity);

			return CreatedAtRoute("GetBook", new { id = bookFromRepo.Id }, _mapper.Map<BookDto>(bookFromRepo));
		}

		[HttpPost("export")]
		public async Task<IActionResult> ExportBooks(CancellationToken cancellationToken)
		{
			var exportFilePath = Path.Combine(Path.GetTempPath(), $"books_export_{DateTime.UtcNow:yyyyMMddHHmmss}.csv");


			await using var writer = new BookExportWriter(exportFilePath);
			var books = await _booksRepository.GetBooksAsync(cancellationToken);
			foreach (var book in books)
			{
				cancellationToken.ThrowIfCancellationRequested();
				var bookDto = _mapper.Map<BookDto>(book);
				await writer.WriteBookAsync(bookDto, cancellationToken);
			}
			//var fileBytes = await System.IO.File.ReadAllBytesAsync(exportFilePath, cancellationToken);
			//System.IO.File.Delete(exportFilePath); // clean up the temporary file
			//return File(fileBytes, "text/csv", "books_export.csv");
			return Ok($"Exported to {exportFilePath}");
		}

		[HttpGet("booksstream")]
		public async IAsyncEnumerable<BookDto> StreamBooks(
			[EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await foreach (var book in _booksRepository.GetBooksAsyncEnumerable().WithCancellation(cancellationToken))
			{
				await Task.Delay(500, cancellationToken); // simulate some processing delay
				yield return _mapper.Map<BookDto>(book);
			}

		}

		[HttpGet("bookcovers/{id}")]
		public async Task<IActionResult> GetBookCover(string id, CancellationToken cancellationToken)
		{
			var bookCover = await _booksRepository.GetBookCoverAsync(id, cancellationToken);
			if (bookCover == null)
			{
				return NotFound();
			}
			// return File(bookCover.Content, "image/jpeg");
			return Ok(_mapper.Map<BookCoverDto>(bookCover));
		}
	}
}
