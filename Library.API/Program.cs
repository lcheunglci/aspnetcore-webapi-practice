using Library.API.Contexts;
using Library.API.Endpoints;
using Library.API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddXmlDataContractSerializerFormatters();

builder.Services.AddProblemDetails();

// builder.Services.AddOpenApi("library-api");
builder.Services.AddOpenApi();

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

app.MapControllers();

app.MapBookEndpoints();

app.Run();
