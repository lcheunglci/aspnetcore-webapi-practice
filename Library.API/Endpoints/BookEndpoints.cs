using System.ComponentModel;
using AutoMapper;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Endpoints;

public static class BookEndpoints
{
    public static RouteGroupBuilder MapBookEndpoints(this IEndpointRouteBuilder app)
    {
		var group = app.MapGroup("api/authors/{authorId:guid}/books")
			.WithTags("Books");

        group.MapGet("", GetBooks);
		group.MapGet("{bookId:guid}", GetBook)
			.WithName("GetBook")
			.WithSummary("Get a book by id")
			.WithDescription("Returns a single book with title and description for a specific author")
			.Produces<Book>(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status400BadRequest);

		group.MapPost("", CreateBook)
			.WithSummary("Create a book for an author")
			.WithDescription("Creates a new book for a specific author")
			//.Accepts<BookForCreation>("application/json")
			//.Accepts<BookForCreation>("application/xml")
			.Produces<Book>(StatusCodes.Status201Created)
			.Produces(StatusCodes.Status404NotFound)
			.ProducesValidationProblem();

        return group;
    }

    private static async Task<IResult> GetBooks(
        Guid authorId,
        IAuthorRepository authorRepository,
        IBookRepository bookRepository,
        IMapper mapper)
    {
        if (!await authorRepository.AuthorExistsAsync(authorId))
        {
            return Results.NotFound();
        }

        var booksFromRepo = await bookRepository.GetBooksAsync(authorId);
        return Results.Ok(mapper.Map<IEnumerable<Book>>(booksFromRepo));
    }

    private static async Task<IResult> GetBook(
		[Description("The id of the author")] Guid authorId,
        [Description("The id of the book to get")] Guid bookId,
        IAuthorRepository authorRepository,
        IBookRepository bookRepository,
        IMapper mapper)
    {
        if (!await authorRepository.AuthorExistsAsync(authorId))
        {
            return Results.NotFound();
        }

        var bookFromRepo = await bookRepository.GetBookAsync(authorId, bookId);
        if (bookFromRepo == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(mapper.Map<Book>(bookFromRepo));
    }

    private static async Task<IResult> CreateBook(
        Guid authorId,
        [FromBody] BookForCreation bookForCreation,
        IAuthorRepository authorRepository,
        IBookRepository bookRepository,
        IMapper mapper)
    {
        if (!await authorRepository.AuthorExistsAsync(authorId))
        {
            return Results.NotFound();
        }

        var bookToAdd = mapper.Map<Entities.Book>(bookForCreation);
        bookRepository.AddBook(bookToAdd);
        await bookRepository.SaveChangesAsync();

        return Results.CreatedAtRoute(
            "GetBook",
            new { authorId, bookId = bookToAdd.Id },
            mapper.Map<Book>(bookToAdd));
    }
}
