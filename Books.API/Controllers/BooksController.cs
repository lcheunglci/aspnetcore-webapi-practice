using AutoMapper;
using Books.API.Filters;
using Books.API.Models;
using Books.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Books.API.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public BooksController(IBookRepository booksRepository, IMapper mapper)
        {
            _bookRepository = booksRepository ??
                throw new ArgumentNullException(nameof(booksRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }

        [HttpGet("Get Book")]
        [BookResultFilter]
        public async Task<IActionResult> GetBooks()
        {
            var bookEntities = await _bookRepository.GetBooksAsync();
            return Ok(bookEntities);
        }

        [HttpGet]
        [Route("{id}")]
        [BookResultFilter]
        public async Task<IActionResult> GetBook(Guid id)
        {
            var bookentity = await _bookRepository.GetBookAsync(id);
            if (bookentity == null)
            {
                return NotFound();
            }
            var bookCover = await _bookRepository.GetBookCoverAsync("dummycover");


            return Ok(bookentity);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook(BookForCreation bookForCreation)
        {
            var bookEntity = _mapper.Map<Entities.Book>(bookForCreation);
            _bookRepository.AddBook(bookEntity);

            await _bookRepository.SaveChangesAsync();

            // Fetch (re-fetch) the book from the data store, including the author
            await _bookRepository.GetBookAsync(bookEntity.Id);

            return CreatedAtRoute(
                "GetBook",
                new { id = bookEntity.Id },
                bookEntity);
        }
    }
}
