using AutoMapper;
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
		public async Task<IActionResult> GetBooks()
		{
			var bookEntity = await _booksRepository.GetBooksAsync();
			return Ok(_mapper.Map<IEnumerable<Models.BookDto>>(bookEntity));
		}


		[HttpGet("{id:guid}", Name = "GetBook")]
		public async Task<IActionResult> GetBook(Guid id)
		{
			var bookEntity = await _booksRepository.GetBookAsync(id);
			if (bookEntity == null)
			{
				return NotFound();
			}
			return Ok(_mapper.Map<Models.BookDto>(bookEntity));
		}
	}
}
