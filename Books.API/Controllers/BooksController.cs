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


		[HttpGet("syncoverasync")]
		public IActionResult GetBooksSynOverAsyncc()
		{
			// bad code result blocks the current thread pool thread while waiting for the async operation to complete, which can lead to thread starvation under load
			// parks a thread, and the pool can only inject new threads slowly 1-2 per second. Queue length rises, latency explodes, and eventually you get timeouts or 503s
			var bookEntities = _booksRepository.GetBooksAsync(CancellationToken.None).Result;
			return Ok(_mapper.Map<IEnumerable<BookDto>>(bookEntities));
		}

		[HttpGet("{id:guid}", Name = "GetBook")]
		public async Task<IActionResult> GetBook(Guid id, CancellationToken cancellationToken)
		{

			// Thread.Sleep(5000); // simulate a long-running operation
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

				var bookCovers = await _booksRepository.GetBookCoversAfterWaitForAllAsync(id, cancellationToken);

				var mappedBook = _mapper.Map<BookWithCoversDto>(bookEntity);
				_mapper.Map(bookCovers, mappedBook);

				return Ok(mappedBook);
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
		public async Task<IActionResult> GetBookCover(Guid id, CancellationToken cancellationToken)
		{
			var bookCover = await _booksRepository.GetBookCoverAsync(id, cancellationToken);
			if (bookCover == null)
			{
				return NotFound();
			}
			// return File(bookCovers.Content, "image/jpeg");
			return Ok(_mapper.Map<BookCoverDto>(bookCover));
		}

		[HttpGet("{id}/bookcovers")]
		public async Task<IActionResult> GetBookCovers(Guid id, CancellationToken cancellationToken)
		{
			var stopwatch = System.Diagnostics.Stopwatch.StartNew();

			// var bookCovers = await _booksRepository.GetBookCoverOneByOneAsync(id, cancellationToken);
			var bookCovers = await _booksRepository.GetBookCoversAfterWaitForAllAsync(id, cancellationToken);
			stopwatch.Stop();
			_logger.LogInformation(
				"[{Timestamp}] GetBookCovers completed in {ElapsedMilliseconds} ms - ThreadId {ThreadId}",
				DateTime.UtcNow.ToString("HH:mm:ss.fff"),
				stopwatch.ElapsedMilliseconds,
				Environment.CurrentManagedThreadId);
			if (bookCovers == null)
			{
				return NotFound();
			}
			
			return Ok(bookCovers);
		}

	}
}
