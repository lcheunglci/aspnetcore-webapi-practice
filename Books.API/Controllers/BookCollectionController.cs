using Books.API.Models;
using Books.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Books.API.Controllers
{
    [Route("api/bookcollection")]
    [ApiController]
    public class BookCollectionController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;

        public BookCollectionController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
        }

        [HttpPost]
        public async Task<IActionResult> CreateBookCollection(
            IEnumerable<BookForCreation> bookCollection)
        {
            return Ok();
        }
    }
}
