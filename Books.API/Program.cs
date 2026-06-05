using System.Runtime.CompilerServices;
using AutoMapper;
using Books.API.DbContexts;
using Books.API.Models;
using Books.API.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http.Resilience;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// register the DbContext on the container 
//builder.Services.AddDbContext<BooksContext>(options =>
//	options.UseSqlite(
//		builder.Configuration["ConnectionStrings:BooksDBConnectionString"]));

builder.Services.AddDbContext<BooksContext>(options =>
	options.UseSqlServer(
		builder.Configuration["ConnectionStrings:BooksDBConnectionString"]));


builder.Services.AddScoped<IBooksRepository, BooksRepository>();

builder.Services.AddAutoMapper(config => { },
	AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddHttpClient("BookCoversAPI", client =>
{
	// client.BaseAddress = new Uri(builder.Configuration["BookCoversAPI:BaseUrl"]!);
	client.BaseAddress = new Uri("https://localhost:52644");
}).AddStandardResilienceHandler();

//.AddResilienceHandler("custom", pipeline =>
//{
//	pipeline.AddTimeout(TimeSpan.FromSeconds(5));
//	pipeline.AddRetry(new HttpRetryStrategyOptions
//	{
//		MaxRetryAttempts = 3,
//		BackoffType = DelayBackoffType.Exponential,
//	});
//	pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions());
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthorization();

app.MapControllers();

app.MapGet("api/minimal/books", async (IBooksRepository repo, IMapper mapper, CancellationToken cancellationToken) =>
{
	var bookEntities = await repo.GetBooksAsync(cancellationToken);
	return Results.Ok(mapper.Map<IEnumerable<BookDto>>(bookEntities));
});

app.MapGet("api/minimal/books/{id:guid}", async Task<Results<Ok<BookDto>, NotFound>> (Guid id, IBooksRepository repo, IMapper mapper, CancellationToken cancellationToken) =>
{
	var bookEntity = await repo.GetBookAsync(id, cancellationToken);
	if (bookEntity == null)
		return TypedResults.NotFound();
	return TypedResults.Ok(mapper.Map<BookDto>(bookEntity));
});

app.MapGet("api/minimal/booksstream", async (IBooksRepository repo, IMapper mapper, CancellationToken cancellationToken) =>
{
	return StreamBooks(repo, mapper, cancellationToken);
	static async IAsyncEnumerable<BookDto> StreamBooks(IBooksRepository repo, IMapper mapper, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		await foreach (var bookEntity in repo.GetBooksAsyncEnumerable().WithCancellation(cancellationToken))
		{
			yield return mapper.Map<BookDto>(bookEntity);
		}
	}
});

app.Run();
