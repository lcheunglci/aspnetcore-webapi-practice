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
        private readonly IBooksRepository _bookRepository;
        private readonly IMapper _mapper;

        public BooksController(IBooksRepository booksRepository, IMapper mapper)
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
            var bookEntity = await _bookRepository.GetBookAsync(id);
            if (bookEntity == null)
            {
                return NotFound();
            }
            var bookCovers = await _bookRepository.GetBookCoversAsync(id);

            //var propertyBag = new Tuple<Entities.Book, IEnumerable<ExternalModels.BookCover>>(bookEntity, bookCover);

            //(Entities.Book book, IEnumerable<ExternalModels.BookCover> bookCovers) propertyBag = (bookEntity, bookCovers);

            //return Ok((book: bookEntity, bookCovers: bookCovers));

            return Ok((bookEntity, bookCovers));
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
