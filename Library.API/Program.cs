using Library.API.Contexts;
using Library.API.Endpoints;
using Library.API.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddXmlDataContractSerializerFormatters();

builder.Services.AddProblemDetails();

// builder.Services.AddOpenApi("library-api");
builder.Services.AddOpenApi(options =>
{
	options.AddDocumentTransformer((document, context, cancellationToken) =>
	{
		document.Info = new()
		{
			Title = "Library API",
			Version = "v1",
			Description = "Through this API you can access authors and their books.",
			Contact = new()
			{
				Name = "Test Author",
				Email = "test@test.com"
			},
			License = new()
			{
				Name = "MIT License",
				Url = new Uri("https://opensource.org/licenses/MIT")
			}
		};
		return Task.CompletedTask;
	});
});

builder.Services.AddDbContext<LibraryContext>(
    dbContextOptions => dbContextOptions.UseSqlite(
        builder.Configuration["ConnectionStrings:LibraryDBConnectionString"]));

builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();

builder.Services.AddAutoMapper(config => { },
    AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

// app.MapOpenApi("/openapi/{documentName}.json");
app.MapOpenApi();

app.MapScalarApiReference(options =>
{
	options
		.WithTitle("Library API")
		.WithTheme(ScalarTheme.DeepSpace)
		.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});

app.MapControllers();

app.MapBookEndpoints();

app.Run();
