using AutoMapper;
using Books.API.DbContexts;
using Books.API.Models;
using Books.API.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// register the DbContext on the container 
builder.Services.AddDbContext<BooksContext>(options =>
	options.UseSqlite(
		builder.Configuration["ConnectionStrings:BooksDBConnectionString"]));

builder.Services.AddScoped<IBooksRepository, BooksRepository>();

builder.Services.AddAutoMapper(config => { },
	AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthorization();

app.MapControllers();

app.MapGet("api/minimal/books", async (IBooksRepository repo, IMapper mapper) =>
{
	var bookEntities = await repo.GetBooksAsync();
	return Results.Ok(mapper.Map<IEnumerable<BookDto>>(bookEntities));
});

app.MapGet("api/minimal/books/{id:guid}", async Task<Results<Ok<BookDto>, NotFound>> (Guid id, IBooksRepository repo, IMapper mapper) =>
{
	var bookEntity = await repo.GetBookAsync(id);
	if (bookEntity == null)
		return TypedResults.NotFound();
	return TypedResults.Ok(mapper.Map<BookDto>(bookEntity));
});

app.Run();
