using AutoMapper;
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
			var bookEntity = await _booksRepository.GetBooksAsync(cancellationToken);
			return Ok(_mapper.Map<IEnumerable<BookDto>>(bookEntity));
		}


		[HttpGet("{id:guid}", Name = "GetBook")]
		public async Task<IActionResult> GetBook(Guid id, CancellationToken cancellationToken)
		{
			_logger.LogInformation(
				"[{Timestamp}] GetBook entered - ThreadId {ThreadId}", DateTime.UtcNow.ToString("HH:mm:ss.fff"), Environment.CurrentManagedThreadId);

			Thread.Sleep(5000); // simulate a long-running operation

			var bookEntity = await _booksRepository.GetBookAsync(id, cancellationToken);
			if (bookEntity == null)
			{
				return NotFound();
			}
			return Ok(_mapper.Map<BookDto>(bookEntity));
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
	}
}
