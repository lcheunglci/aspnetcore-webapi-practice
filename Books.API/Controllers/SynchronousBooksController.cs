using Books.API.Filters;
using Books.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Books.API.Controllers
{
    [Route("api/synchronousbooks")]
    public class SynchronousBooksController : ControllerBase
    {
        private IBookRepository _booksRepository;

        public SynchronousBooksController(IBookRepository booksRepository)
        {
            _booksRepository = booksRepository ?? throw new ArgumentNullException(nameof(booksRepository));
        }

        [HttpGet]
        [BookResultFilter]
        public IActionResult GetBooks()
        {
            var bookEntities = _booksRepository.GetBooks();
            return Ok(bookEntities);
        }
    }
}
